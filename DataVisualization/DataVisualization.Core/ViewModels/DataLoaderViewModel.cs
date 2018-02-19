using Caliburn.Micro;
using DataVisualization.Services;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DataVisualization.Core.ViewModels
{
    public class DataLoaderViewModel : PropertyChangedBase
    {
        private string _filePath = "Path to a file...";

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                NotifyOfPropertyChange(() => FilePath );
                PreviewSample();
            }
        }

        private IEnumerable<object> _sampledData;

        public IEnumerable<object> SampledData
        {
            get => _sampledData;
            set { _sampledData = value; NotifyOfPropertyChange(() => SampledData); }
        }



        private readonly IDataService _dataService;

        public DataLoaderViewModel(IDataService dataService)
        {
            _dataService = dataService;
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
        }
    }
}
