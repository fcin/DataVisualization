using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using DataVisualization.Services.Transform;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataColumn = System.Data.DataColumn;

namespace DataVisualization.Core.ViewModels.DataLoading
{
    public class AxisTypes : BindableCollection<Axes>
    {
        private readonly List<Axes> _myColumnTypes = new List<Axes>
        {
            Axes.None, Axes.X1, Axes.X2, Axes.Y1, Axes.Y2
        };

        public AxisTypes()
        {
            if (base.Count == 0)
                base.AddRange(_myColumnTypes);
        }
    }

    public class CustomColumnTypes : BindableCollection<Type>
    {
        private readonly List<Type> _myColumnTypes = new List<Type>
        {
            typeof(string), typeof(double), typeof(DateTime)
        };

        public CustomColumnTypes()
        {
            if (base.Count == 0)
                base.AddRange(_myColumnTypes);
        }
    }

    /// <summary>
    /// Temporary solution for loading data. It will be replaced soon.
    /// </summary>
    public class DataLoaderViewModel : Screen
    {
        private AxisTypes _myAxisTypes = new AxisTypes();
        public AxisTypes MyAxisTypes
        {
            get => _myAxisTypes;
            set => SetValue(ref _myAxisTypes, value);
        }

        private CustomColumnTypes _myColumnTypes = new CustomColumnTypes();
        public CustomColumnTypes MyColumnTypes
        {
            get => _myColumnTypes;
            set => SetValue(ref _myColumnTypes, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetValue(ref _filePath, value);
        }

        private string _thousandsSeparator = ".";
        public string ThousandsSeparator
        {
            get => _thousandsSeparator;
            set => SetValue(ref _thousandsSeparator, value);
        }

        private string _decimalSeparator = ",";
        public string DecimalSeparator
        {
            get => _decimalSeparator;
            set => SetValue(ref _decimalSeparator, value);
        }

        private ObservableCollection<object> _dataGridCollection;
        public ObservableCollection<object> DataGridCollection
        {
            get => _dataGridCollection;
            set => SetValue(ref _dataGridCollection, value);
        }

        private readonly DataFileReader _dataFileReader;
        private readonly DataConfigurationService _dataConfigurationService;

        public DataLoaderViewModel()
        {
            DataGridCollection = new ObservableCollection<object>();
            _dataFileReader = new DataFileReader();
            _dataConfigurationService = new DataConfigurationService();
        }

        private DataGridColumnsModel _dataGridColumnsModel = new DataGridColumnsModel();
        public DataGridColumnsModel DataGridColumnsModel
        {
            get => _dataGridColumnsModel;
            set => SetValue(ref _dataGridColumnsModel, value);
        }

        public void OnColumnTypeChanged(SelectionChangedEventArgs args, string columnName, ComboBox comboBox)
        {
            var newType = args.AddedItems[0].ToString();
            var index = DataGridColumnsModel.GetColumnIndex(columnName);
            var newValues = new List<string>();

            var parser = new ValueParser(ThousandsSeparator, DecimalSeparator);

            try
            {
                // Try to convert Rows.
                foreach (var item in DataGridCollection)
                {
                    var prop = item.GetType().GetProperty(columnName);
                    var oldValue = prop.GetValue(item).ToString();
                    var newValue = parser.Parse(oldValue, newType).ToString();
                    newValues.Add(newValue);
                }
            }
            catch
            {
                comboBox.SelectedIndex = 0;
                var safeValue = comboBox.SelectedItem.ToString();
                DataGridColumnsModel.Columns[index] = new GridColumn(columnName, safeValue, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
                MessageBox.Show("Cannot convert data to this type. Please select a different one.");
                ValidateSubmit();
                return;
            }

            // If all rows were converted then assign all values.
            for (var propIndex = 0; propIndex < DataGridCollection.Count; propIndex++)
            {
                var item = DataGridCollection[propIndex];
                var prop = item.GetType().GetProperty(columnName);
                prop.SetValue(item, newValues[propIndex]);
            }

            // Assing new type to header.
            DataGridColumnsModel.Columns[index] = new GridColumn(columnName, newType, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
            NotifyOfPropertyChange(() => MyColumnTypes);
            ValidateSubmit();
        }

        public async Task RecreateGrid()
        {
            DataGridCollection.Clear();

            var data = await _dataFileReader.ReadSampleAsync(FilePath, 30);

            var properties = (from DataColumn column in data.Columns
                              select new Tuple<string, Type>(column.ColumnName, typeof(string))).ToList();

            // Add columns.
            foreach (var property in properties)
            {
                var column = new GridColumn(property.Item1, property.Item2.ToString(), false, MyAxisTypes[0]);
                if (!DataGridColumnsModel.Columns.Contains(column))
                    DataGridColumnsModel.Columns.Add(column);
            }

            // Add rows.
            for (var rowIndex = 0; rowIndex < data.Rows.Count; rowIndex++)
            {
                var columnModel = ModelCreator.Create(properties.ToArray());
                var row = data.Rows[rowIndex];

                for (var columnIndex = 0; columnIndex < row.ItemArray.Length; columnIndex++)
                {
                    var cell = row[columnIndex];
                    columnModel.GetType().InvokeMember(properties[columnIndex].Item1, BindingFlags.SetProperty, null, columnModel, new[] { cell });
                }

                DataGridCollection.Add(columnModel);
            }

            ValidateSubmit();
        }

        public void OnColumnNameChanged(string oldName, string newName, TextBox textBox)
        {
            if (oldName.Equals(newName))
                return;

            var index = DataGridColumnsModel.GetColumnIndex(oldName);
            // TODO: INDEX SOMETIMES RETURNS -1 0.o
            var type = DataGridColumnsModel.Columns[index].ColumnType;
            var isNameValid = newName.All(character => char.IsLetterOrDigit(character) || character.Equals('_'));

            if (!isNameValid || DataGridColumnsModel.Columns.Any(col => col.Name.Equals(newName)))
            {
                DataGridColumnsModel.Columns[index] = new GridColumn(oldName, type, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
                textBox.Text = oldName;
                MessageBox.Show($"Column with name {newName} already exists or contains illegal character.");
            }
            else
            {
                DataGridColumnsModel.Columns[index] = new GridColumn(newName, type, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
                textBox.Text = newName;
            }
        }

        public void OnIgnoreSelected(string columnName)
        {
            var index = DataGridColumnsModel.GetColumnIndex(columnName);
            DataGridColumnsModel.Columns[index] = new GridColumn(DataGridColumnsModel.Columns[index].Name,
                                                                                  DataGridColumnsModel.Columns[index].ColumnType,
                                                                                  !DataGridColumnsModel.Columns[index].IsIgnored,
                                                                                  DataGridColumnsModel.Columns[index].Axis);

            ValidateSubmit();
        }

        public void OnAxisTypeChanged(SelectionChangedEventArgs args, string columnName, ComboBox comboBox)
        {
            var index = DataGridColumnsModel.GetColumnIndex(columnName);
            DataGridColumnsModel.Columns[index].Axis = (Axes)args.AddedItems[0];
            ValidateSubmit();
        }

        public void OnFileSelectionClicked()
        {
            var fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "CSV Files (.csv) |*.csv"
            };
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                FilePath = fileDialog.FileName;
            }
        }

        public bool CanOnDataLoad => !string.IsNullOrEmpty(FilePath) &&
                                     !_dataConfigurationService.Exists(Path.GetFileNameWithoutExtension(FilePath)) &&
                                     !DataGridColumnsModel.Columns.Any(col => col.ColumnType.Equals($"{typeof(string)}") && !col.IsIgnored) &&
                                      DataGridColumnsModel.Columns.Count(col => !col.IsIgnored) > 1 &&
                                      DataGridColumnsModel.Columns.Any(col => col.Axis == Axes.X1) &&
                                      DataGridColumnsModel.Columns.Any(col => col.Axis == Axes.Y1) &&
                                     !DataGridColumnsModel.Columns.Any(col => !col.IsIgnored && col.Axis == Axes.None);

        public void OnDataLoad()
        {
            var config = new DataConfiguration
            {
                DataName = Path.GetFileNameWithoutExtension(FilePath),
                FilePath = FilePath,
                ThousandsSeparator = ThousandsSeparator,
                DecimalSeparator = DecimalSeparator,
                Columns = DataGridColumnsModel.Columns.Select((col, index) => new
                {
                    Column = new Models.DataColumn
                    {
                        Index = index,
                        Name = col.Name,
                        ColumnType = col.ColumnType,
                        Axis = col.Axis
                    },
                    Ignore = col.IsIgnored
                }).Where(column => !column.Ignore).Select(col => col.Column).ToList()
            };

            try
            {
                _dataConfigurationService.Add(config);
                TryClose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ValidateSubmit()
        {
            NotifyOfPropertyChange(() => CanOnDataLoad);
        }

        private void SetValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            NotifyOfPropertyChange(propertyName);
        }
    }
}
