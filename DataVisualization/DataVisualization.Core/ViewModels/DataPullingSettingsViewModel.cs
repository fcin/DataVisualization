using Caliburn.Micro;
using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Core.ViewModels
{
    public class DataPullingSettingsViewModel : Screen
    {
        public IEnumerable<string> PullingMethods => _pullingMethods.Select(m => m.Key);
        private string _selectedPullingMethod;
        public string SelectedPullingMethod
        {
            get => _selectedPullingMethod;
            set
            {
                Set(ref _selectedPullingMethod, value);
                NotifyOfPropertyChange(() => IsChangeEndpointUrlEnabled);
                NotifyOfPropertyChange(() => EndpointUrl);
            }
        }
        private bool _isChangeEndpointUrlEnabled;
        public bool IsChangeEndpointUrlEnabled
        {
            get => _pullingMethods[SelectedPullingMethod].CanChangeName;
            set => Set(ref _isChangeEndpointUrlEnabled, value);
        }
        
        public string EndpointUrl
        {
            get => _pullingMethods[SelectedPullingMethod].EndpointUrl;
            set
            {
                _pullingMethods[SelectedPullingMethod].EndpointUrl = value;
                NotifyOfPropertyChange(() => EndpointUrl);
            }
        }

        public event EventHandler<PullingMethodProperties> OnSubmit;

        private Dictionary<string, PullingMethodModel> _pullingMethods;

        public DataPullingSettingsViewModel(string defaultEndpointUrl)
        {
            _pullingMethods = new Dictionary<string, PullingMethodModel>
            {
                { "From local file", new PullingMethodModel {
                    Method = Models.PullingMethods.LocalFile,
                    CanChangeName = false,
                    EndpointUrl = defaultEndpointUrl
                } },
                { "From JSON request", new PullingMethodModel {
                    Method = Models.PullingMethods.HttpJson,
                    CanChangeName = true,
                    EndpointUrl = string.Empty
                } }
            };
            SelectedPullingMethod = "From local file";
        }

        public void Submit()
        {
            var method = new PullingMethodProperties
            {
                EndpointUrl = EndpointUrl,
                Method = _pullingMethods[SelectedPullingMethod].Method
            };
            OnSubmit?.Invoke(this, method);

            TryClose();
        }

        private class PullingMethodModel
        {
            public PullingMethods Method { get; set; }
            public bool CanChangeName { get; set; }
            public string EndpointUrl { get; set; }
        }  
    }
}
