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
        public DataLoaderViewModel LoaderVm { get; set; }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => Set(ref _filePath, value);
        }

        public WizardViewModel(DataLoaderViewModel loaderVm)
        {
            LoaderVm = loaderVm;
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

        public async void RunLoader()
        {
            if (Path.GetExtension(FilePath) == ".csv")
                await LoaderVm.RecreateGridAsync(FilePath);
        }
    }
}
