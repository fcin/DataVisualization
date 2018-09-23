using Caliburn.Micro;
using DataVisualization.Core.Events;
using DataVisualization.Core.Translations;
using DataVisualization.Core.Views;
using DataVisualization.Models;
using DataVisualization.Services;
using DataVisualization.Services.Transform;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DataColumn = System.Data.DataColumn;

namespace DataVisualization.Core.ViewModels
{
    public class SelectableTypes : PropertyChangedBase
    {
        public List<ColumnTypes> MyColumnTypes => ColumnTypeDef.AllTypes.Select(t => t.PrettyType).ToList();

        public ColumnTypes SelectedType { get; set; }

        public SelectableTypes()
        {
            SelectedType = MyColumnTypes[0];
        }
    }

    public class SelectableAxes : PropertyChangedBase
    {
        public List<Axes> MyAxisTypes => new List<Axes> { Axes.None, Axes.X1, Axes.X2, Axes.Y1, Axes.Y2 };

        public Axes SelectedAxis { get; set; }

        public SelectableAxes()
        {
            SelectedAxis = MyAxisTypes[0];
        }
    }

    public class DataLoaderViewModel : Screen, IHandle<LoadingBarOpenedEventArgs>, IHandle<LoadingBarClosedEventArgs>
    {
        private bool _isLoaderWindowEnabled;
        public bool IsLoaderWindowEnabled
        {
            get => _isLoaderWindowEnabled;
            set => SetValue(ref _isLoaderWindowEnabled, value);
        }

        private List<SelectableTypes> _allSelectableTypes = new List<SelectableTypes>();
        public List<SelectableTypes> AllSelectableTypes
        {
            get => _allSelectableTypes;
            set => SetValue(ref _allSelectableTypes, value);
        }

        private List<SelectableAxes> _allSelectableAxes = new List<SelectableAxes>();
        public List<SelectableAxes> AllSelectableAxes
        {
            get => _allSelectableAxes;
            set => SetValue(ref _allSelectableAxes, value);
        }

        public List<bool> _isIgnoredList = new List<bool>();
        public List<bool> IsIgnoredList
        {
            get => _isIgnoredList;
            set => SetValue(ref _isIgnoredList, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetValue(ref _filePath, value);
        }

        public IEnumerable<char> AllThousandsSeparators => new List<char> { '.', ',' };

        private char _selectedThousandsSeparator = '.';
        public char SelectedThousandsSeparator
        {
            get => _selectedThousandsSeparator;
            set => SetValue(ref _selectedThousandsSeparator, value);
        }

        public IEnumerable<char> AllDecimalSeparators => new List<char> { ',', '.' };

        private char _selectedDecimalSeparator = ',';
        public char SelectedDecimalSeparator
        {
            get => _selectedDecimalSeparator;
            set => SetValue(ref _selectedDecimalSeparator, value);
        }

        private BindableCollection<object> _dataGridCollection;
        public BindableCollection<object> DataGridCollection
        {
            get => _dataGridCollection;
            set => SetValue(ref _dataGridCollection, value);
        }

        private DataGridColumnsModel _dataGridColumnsModel = new DataGridColumnsModel();
        public DataGridColumnsModel DataGridColumnsModel
        {
            get => _dataGridColumnsModel;
            set => SetValue(ref _dataGridColumnsModel, value);
        }

        public List<KeyValuePair<string, int>> RefreshTimes { get; } = new List<KeyValuePair<string, int>> {
            new KeyValuePair<string, int>("Don't watch", 0),
            new KeyValuePair<string, int>("10 seconds", 10),
            new KeyValuePair<string, int>("1 minute", 1 * 60),
            new KeyValuePair<string, int>("5 minutes", 5 * 60),
            new KeyValuePair<string, int>("30 minutes", 30 * 60),
            new KeyValuePair<string, int>("60 minutes", 60 * 60)
        };

        public KeyValuePair<string, int> SelectedRefreshTime { get; set; }

        private IEnumerable<string> _errors;
        public IEnumerable<string> Errors
        {
            get => _errors;
            set => Set(ref _errors, value);
        }

        private readonly IEventAggregator _eventAggregator;
        private readonly LoadingBarManager _loadingBarManager;
        private readonly DataFileReader _dataFileReader;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataService _dataService;
        private DataTable _sampleData;

        public DataLoaderViewModel(IEventAggregator eventAggregator, LoadingBarManager loadingBarManager,
            DataConfigurationService dataConfigurationService, DataService dataService)
        {
            _eventAggregator = eventAggregator;
            _loadingBarManager = loadingBarManager;

            DataGridCollection = new BindableCollection<object>();
            _dataFileReader = new DataFileReader();
            _dataConfigurationService = dataConfigurationService;
            _dataService = dataService;

            eventAggregator.Subscribe(this);

            SelectedRefreshTime = RefreshTimes[0];
            IsLoaderWindowEnabled = true;
        }

        public async void OnColumnTypeChanged(SelectionChangedEventArgs args, string columnName, ComboBox comboBox)
        {
            var newType = ColumnTypeDef.AllTypes.First(t => t.PrettyType == (ColumnTypes)args.AddedItems[0]);
            var index = DataGridColumnsModel.GetColumnIndex(columnName);

            var parser = new ValueParser(SelectedThousandsSeparator.ToString(), SelectedDecimalSeparator.ToString());
            var valuesToParse = _sampleData.AsEnumerable().Select(s => s.Field<string>(index));
            (bool isParsed, List<object> parsedValues) = parser.TryParseAll(valuesToParse, newType);

            if (!isParsed)
            {
                comboBox.SelectedIndex = 0;
                var safeValue = ColumnTypeDef.AllTypes.First(t => t.PrettyType == (ColumnTypes)comboBox.SelectedItem);
                DataGridColumnsModel.Columns[index] = new GridColumn(columnName, safeValue, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
                ValidateSubmit();

                var popup = new PopupBoxView
                {
                    DataContext = new PopupBoxViewModel(PopupBoxType.Ok, Translation.ConvertToTypeFailErrorMessage, true)
                };
                await DialogHost.Show(popup, "DataLoaderHost");

                return;
            }

            // If all rows were converted then assign all values.
            for (var propIndex = 0; propIndex < DataGridCollection.Count; propIndex++)
            {
                var item = DataGridCollection[propIndex];
                var prop = item.GetType().GetProperty(columnName);
                prop.SetValue(item, parsedValues[propIndex].ToString());
            }

            // Assing new type to header.
            DataGridColumnsModel.Columns[index] = new GridColumn(columnName, newType, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
            DataGridCollection.Refresh();

            ValidateSubmit();
        }

        public async Task RecreateGrid()
        {
            DataGridCollection.Clear();

            _sampleData = await _dataFileReader.ReadSampleAsync(FilePath, 30);

            var properties = (from DataColumn column in _sampleData.Columns
                              select new Tuple<string, ColumnTypeDef>(column.ColumnName, ColumnTypeDef.Unknown)).ToList();

            // Add columns.
            foreach (var property in properties)
            {
                var column = new GridColumn(property.Item1, property.Item2, false, Axes.None);
                if (!DataGridColumnsModel.Columns.Contains(column))
                    DataGridColumnsModel.Columns.Add(column);
            }

            // Add rows.
            for (var rowIndex = 0; rowIndex < _sampleData.Rows.Count; rowIndex++)
            {
                var columnModel = ModelCreator.Create(properties.Select(prop => Tuple.Create(prop.Item1, Type.GetType(prop.Item2.InternalType))).ToArray());
                var row = _sampleData.Rows[rowIndex];

                for (var columnIndex = 0; columnIndex < row.ItemArray.Length; columnIndex++)
                {
                    var cell = row[columnIndex];
                    columnModel.GetType().InvokeMember(properties[columnIndex].Item1, BindingFlags.SetProperty, null, columnModel, new[] { cell });
                }

                DataGridCollection.Add(columnModel);
            }

            var columnsCount = DataGridCollection[0].GetType().GetProperties().Count();
            for (var i = 0; i < columnsCount; i++)
            {
                AllSelectableTypes.Add(new SelectableTypes());
                AllSelectableAxes.Add(new SelectableAxes());
                IsIgnoredList.Add(false);
            }

            GuessColumnTypes();
            SetAxes();

            ValidateSubmit();
        }

        private void GuessColumnTypes()
        {
            for (int index = 0; index < DataGridColumnsModel.Columns.Count; index++)
            {
                var column = DataGridColumnsModel.Columns[index];
                foreach (var type in ColumnTypeDef.AllTypes.Where(t => t != ColumnTypeDef.Unknown))
                {
                    var parser = new ValueParser(SelectedThousandsSeparator.ToString(), SelectedDecimalSeparator.ToString());
                    var valuesToParse = _sampleData.AsEnumerable().Select(s => s.Field<string>(index));
                    (bool isParsed, List<object> parsedValues) = parser.TryParseAll(valuesToParse, type);

                    if (!isParsed)
                    {
                        continue;
                    }

                    for (var propIndex = 0; propIndex < DataGridCollection.Count; propIndex++)
                    {
                        var item = DataGridCollection[propIndex];
                        var prop = item.GetType().GetProperty(column.Name);
                        prop.SetValue(item, parsedValues[propIndex].ToString());
                    }

                    DataGridColumnsModel.Columns[index] = new GridColumn(column.Name, type, DataGridColumnsModel.Columns[index].IsIgnored, DataGridColumnsModel.Columns[index].Axis);
                    AllSelectableTypes[index].SelectedType = type.PrettyType;

                    break;
                }

                if (DataGridColumnsModel.Columns[index].ColumnType == ColumnTypeDef.Unknown)
                {
                    IsIgnoredList[index] = true;
                    DataGridColumnsModel.Columns[index].IsIgnored = true;
                }
            }
        }

        private void SetAxes()
        {
            var column = DataGridColumnsModel.Columns.FirstOrDefault(d => !d.IsIgnored);
            if (column == null)
                return;
            var columnIndex = DataGridColumnsModel.Columns.IndexOf(column);
            DataGridColumnsModel.Columns[columnIndex].Axis = Axes.X1;
            AllSelectableAxes[columnIndex].SelectedAxis = Axes.X1;
            for (int index = columnIndex + 1; index < DataGridColumnsModel.Columns.Count; index++)
            {
                if (DataGridColumnsModel.Columns[index].IsIgnored)
                    continue;

                DataGridColumnsModel.Columns[index].Axis = Axes.Y1;
                AllSelectableAxes[index].SelectedAxis = Axes.Y1;
            }
        }

        public async void OnColumnNameChanged(string oldName, string newName, TextBox textBox)
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
                var message = Translation.ColumnAlreadyExistsOrillegalCharacter.Replace("{newName}", newName);
                var popup = new PopupBoxView
                {
                    DataContext = new PopupBoxViewModel(PopupBoxType.Ok, message, true)
                };
                await DialogHost.Show(popup, "DataLoaderHost");
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

        public bool CanOnDataLoad
        {
            get
            {
                UpdateAllErrors();
                return !Errors.Any();
            }
        }

        private void UpdateAllErrors()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(FilePath))
                errors.Add(Translation.LoadError_SelectFile);
            if (_dataConfigurationService.Exists(Path.GetFileNameWithoutExtension(FilePath)))
                errors.Add(Translation.LoadError_FileAlreadyLoaded.Replace("{fileName}", Path.GetFileName(FilePath)));
            var columnsWithoutTypes = DataGridColumnsModel.Columns.Where(col => col.ColumnType.Equals(ColumnTypeDef.Unknown) && !col.IsIgnored);
            foreach (var columnWithoutType in columnsWithoutTypes)
                errors.Add(Translation.LoadError_ColumnWithoutType.Replace("{columnName}", columnWithoutType.Name));
            if (DataGridColumnsModel.Columns.Count(col => !col.IsIgnored) < 2)
                errors.Add(Translation.LoadError_NotIgnoredColumnsMissing);
            if (!DataGridColumnsModel.Columns.Any(col => col.Axis == Axes.X1))
                errors.Add(Translation.LoadError_ColumnWithAxisX1Missing);
            if (!DataGridColumnsModel.Columns.Any(col => col.Axis == Axes.Y1))
                errors.Add(Translation.LoadError_ColumnWithAxisY1Missing);
            var columnsWithoutAxes = DataGridColumnsModel.Columns.Where(col => !col.IsIgnored && col.Axis == Axes.None);
            foreach (var columnWithoutAxis in columnsWithoutAxes)
                errors.Add(Translation.LoadError_ColumnWithoutAxis.Replace("{columnName}", columnWithoutAxis.Name));

            Errors = errors;
        }

        public async void OnDataLoad()
        {
            var config = new DataConfiguration
            {
                DataName = Path.GetFileNameWithoutExtension(FilePath),
                FilePath = FilePath,
                ThousandsSeparator = SelectedThousandsSeparator.ToString(),
                DecimalSeparator = SelectedDecimalSeparator.ToString(),
                RefreshRate = TimeSpan.FromSeconds(SelectedRefreshTime.Value),
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

                if (!_dataService.Exists(config.DataName))
                {
                    var loadingBarWindow = _loadingBarManager.ShowLoadingBar();
                    var readDataProgress = new Progress<LoadingBarStatus>(result =>
                    {
                        loadingBarWindow.PercentFinished = result.PercentFinished;
                        loadingBarWindow.Message = result.Message;
                    });

                    var loadedData = await _dataFileReader.ReadDataAsync(config, readDataProgress);
                    _dataService.AddData(loadedData);

                    _loadingBarManager.CloseLoadingBar();
                }

                _eventAggregator.PublishOnUIThread(new NewDataAddedEventArgs());
                TryClose();
            }
            catch (Exception ex)
            {
                var message = $"{Translation.InternalError}: {ex.Message}";
                var popup = new PopupBoxView
                {
                    DataContext = new PopupBoxViewModel(PopupBoxType.Ok, message, true)
                };
                await DialogHost.Show(popup, "DataLoaderHost");
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

        public void Handle(LoadingBarOpenedEventArgs message)
        {
            IsLoaderWindowEnabled = false;
        }

        public void Handle(LoadingBarClosedEventArgs message)
        {
            IsLoaderWindowEnabled = true;
        }
    }
}
