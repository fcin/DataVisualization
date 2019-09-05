using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Win32;

namespace DataVisualization.Core.ViewModels
{
    public class WizardViewModel : Screen
    {
        private LoaderViewModelBase _loaderVm;

        public LoaderViewModelBase LoaderVm
        {
            get => _loaderVm;
            set => Set(ref _loaderVm, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        private DataLoaderViewModel _dataLoaderVm;
        private readonly DvFileLoaderViewModel _dvFileLoaderVm;

        public WizardViewModel(DataLoaderViewModel loaderVm, DvFileLoaderViewModel dvFileLoaderVm)
        {
            _dataLoaderVm = loaderVm;
            _dvFileLoaderVm = dvFileLoaderVm;
        }

        public void OnFileSelectionClicked()
        {
            var fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "CSV Files (.csv)|*.csv|DV Files (.dv)|*.dv"
            };
            var dialogResult = fileDialog.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                FilePath = fileDialog.FileName;
            }
        }

        public async void RunLoader()
        {
            switch (Path.GetExtension(FilePath))
            {
                case ".csv":
                    LoaderVm = _dataLoaderVm;
                    break;
                case ".dv":
                    LoaderVm = _dvFileLoaderVm;
                    break;
                default:
                    throw new ArgumentException(nameof(FilePath));
            }

            await LoaderVm.InitializeAsync(FilePath);
        }
    }
}
