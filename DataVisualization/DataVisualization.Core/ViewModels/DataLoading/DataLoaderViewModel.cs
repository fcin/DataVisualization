using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using DataVisualization.Core.Annotations;
using DataVisualization.Services;
using Microsoft.Win32;
using Action = System.Action;

namespace DataVisualization.Core.ViewModels.DataLoading
{
    /// <summary>
    /// Temporary solution for loading data. It will be replaced soon.
    /// </summary>
    public class DataLoaderViewModel : PropertyChangedBase
    {
        private string _filePath = "Path to a file...";

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath);
                PreviewSample();
            }
        }

        private DataTable _sampledData = new DataTable();

        public DataTable SampledData
        {
            get => _sampledData;
            set { _sampledData = value; NotifyOfPropertyChange(() => SampledData); }
        }

        private readonly IDataService _dataService;
        private readonly string[] _types = { "Not Set", "Number", "Date" };

        public DataLoaderViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Mappings = new List<DataListBoxItem>();
        }

        private List<DataListBoxItem> _mappings;

        public List<DataListBoxItem> Mappings
        {
            get => _mappings;
            set { _mappings = value; NotifyOfPropertyChange(() => Mappings); }
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

        public void OnDataLoad()
        {
            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            {
                MessageBox.Show("You have to provide a valid path.");
                return;
            }
        }

        private async void PreviewSample()
        {
            SampledData = await _dataService.GetSampleDataAsync(FilePath, 30);
            Mappings = new List<DataListBoxItem>();
            foreach (DataColumn column in SampledData.Columns)
            {
                var item = new DataListBoxItem(_types, column.ColumnName);
                item.CanChangeName += newName => SampledData.Columns.IndexOf(newName) == -1;
                // check if every item is valid
                item.ColumnNameChanged += () =>
                {
                    if (Mappings.Any(c => !c.IsValid))
                        return;
                    var copy = SampledData.Clone();
                    foreach (var configItem in Mappings)
                    {
                        var colId = SampledData.Columns.IndexOf(configItem.IdentifiableColumnName);
                        copy.Columns[colId].ColumnName = configItem.CustomColumnName;
                        configItem.Reset();
                    }
                    foreach (DataRow row in SampledData.Rows)
                    {
                        copy.Rows.Add(row.ItemArray);
                    }
                    SampledData = copy;
                };
                Mappings.Add(item);
            }
        }

        public void OnColumnTypeSelected(SelectionChangedEventArgs eventArgs, DataListBoxItem item)
        {
            if (eventArgs.AddedItems.Count <= 0)
                return;

            switch (eventArgs.AddedItems[0].ToString())
            {
                case "Not Set":
                    item.CurrentType = typeof(string);
                    break;
                case "Number":
                    item.CurrentType = typeof(double);
                    break;
                case "Date":
                    item.CurrentType = typeof(DateTime);
                    break;
                default: throw new ArgumentException("Invalid enum value");
            }

            var copy = SampledData.Clone();
            var column = copy.Columns[SampledData.Columns.IndexOf(item.CustomColumnName)];
            column.DataType = item.CurrentType;

            try
            {
                foreach (DataRow row in SampledData.Rows)
                {
                    copy.ImportRow(row);
                }
            }
            catch
            {
                MessageBox.Show("Cannot convert to selected type. No conversion found");
                item.SelectedTypeIndex = Array.IndexOf(item.ColumnTypes, eventArgs.RemovedItems[0].ToString());
                return;
            }

            SampledData = copy;
        }
    }

    public class DataListBoxItem : ListBoxItem, INotifyPropertyChanged, IDataErrorInfo
    {
        public event Func<string, bool> CanChangeName;
        public event Action ColumnNameChanged;
        public string[] ColumnTypes { get; set; }

        public Type CurrentType { get; set; } = typeof(string);

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }

        private bool _changed;
        private string _identifiableColumnName;
        public string IdentifiableColumnName
        {
            get => _identifiableColumnName;
            set
            {
                if (!_changed)
                    _identifiableColumnName = value;
            }
        }

        public void Reset()
        {
            _changed = false;
            IdentifiableColumnName = CustomColumnName;
        }

        private string _customColumnName;
        public string CustomColumnName
        {
            get => _customColumnName;
            set { _customColumnName = value; OnPropertyChanged(); }
        }

        private int _selectedTypeIndex;
        public int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set { _selectedTypeIndex = value; OnPropertyChanged(); }
        }

        private bool _isDisabled;
        public bool IsDisabled
        {
            get => _isDisabled;
            set { _isDisabled = value; OnPropertyChanged(); }
        }

        public DataListBoxItem(string[] columnTypes, string text)
        {
            ColumnTypes = columnTypes;
            CustomColumnName = text;
            SelectedTypeIndex = 0;
            IdentifiableColumnName = CustomColumnName;
        }

        public void OnColumnNameChanged(TextChangedEventArgs eventArgs)
        {
            var newText = ((TextBox)eventArgs.Source).Text;
            if (CanChangeName?.Invoke(newText) == true)
            {
                IsValid = true;
                if (!_changed)
                {
                    IdentifiableColumnName = CustomColumnName;
                    _changed = true;
                }
                CustomColumnName = newText;
            }
            else
            {
                IsValid = false;
            }
        }

        public void OnLostFocus()
        {
            if(IsValid)
                ColumnNameChanged?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                string errorMsg = null;

                if (columnName == nameof(CustomColumnName) && !IsValid)
                    errorMsg = "Invalid column name!";

                return errorMsg;
            }
        }

        public string Error => null;
    }
}
