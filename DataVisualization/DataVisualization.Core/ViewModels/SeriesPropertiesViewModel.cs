using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Models.Transformations;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DataVisualization.Core.ViewModels
{
    public class SeriesPropertiesViewModel : Screen
    {
        public string SeriesName
        {
            get => _series.Name;
            set
            {
                _series.Name = value;
                NotifyOfPropertyChange(() => SeriesName);
            }
        }

        public Color SeriesColor
        {
            get => (Color)ColorConverter.ConvertFromString(_series.ColorHex);
            set
            {
                _series.ColorHex = value.ToString();
                NotifyOfPropertyChange(() => SeriesColor);
            }
        }

        public int Thickness
        {
            get => _series.Thickness;
            set
            {
                _series.Thickness = value;
                NotifyOfPropertyChange(() => Thickness);
            }
        }

        private double _sampleValue;
        public double SampleValue
        {
            get => _sampleValue;
            set => Set(ref _sampleValue, value);
        }


        private BindableCollection<ITransformationViewModel> _transformations;
        public BindableCollection<ITransformationViewModel> TransformationVms
        {
            get => _transformations;
            set => Set(ref _transformations, value);
        }

        public IEnumerable<string> AllTransformationDefinitionNames => _availableTransformations.Select(trans => trans.Name);

        private IEnumerable<ITransformationViewModel> _availableTransformations =
            TransformationViewModelFactory.GetAllTransformationVms();

        private readonly string _oldName;
        private readonly Series _series;
        private readonly DataService _dataService;

        public SeriesPropertiesViewModel(Series series, DataService dataService)
        {
            _series = series;
            _dataService = dataService;
            _oldName = series.Name;
            SampleValue = 0;

            var transformations = series.Transformations.Select(t => TransformationViewModelFactory.Create(t));
            TransformationVms = new BindableCollection<ITransformationViewModel>(transformations);

            RecalculateAggregate();
        }

        public void OnSave()
        {
            if (SeriesName != _oldName && _dataService.SeriesWithNameExists(SeriesName))
            {
                MessageBox.Show($"Series with name {SeriesName} already exists", "Series name already exists", MessageBoxButton.OK);
                return;
            }

            _series.SetTransformations(TransformationVms.Select(t => t.Transformation).ToList());
            _dataService.UpdateSeries(_series);

            TryClose(true);
        }

        public void OnCancel()
        {
            TryClose(false);
        }

        public void AddTransformation()
        {
            TransformationVms.Add(TransformationViewModelFactory.Create("Add"));


        }

        public void TransformationChanged(ITransformationViewModel item, SelectionChangedEventArgs args)
        {
            if (item == null)
                return;

            if (args != null && args.AddedItems.Count != 0)
            {
                var index = TransformationVms.IndexOf(item);
                TransformationVms[index] = TransformationViewModelFactory.Create(args.AddedItems[0].ToString());
            }

            RecalculateAggregate();
            
            TransformationVms.Refresh();
        }

        public void TransformationValueChanged()
        {
            RecalculateAggregate();

            TransformationVms.Refresh();
        }

        public void RemoveTransformation(ITransformationViewModel transformationVm)
        {
            var index = TransformationVms.ToList().FindIndex(t => t.Id == transformationVm.Id);
            TransformationVms.RemoveAt(index);

            RecalculateAggregate();

            TransformationVms.Refresh();
        }

        public void SampleValueUpdated()
        {
            RecalculateAggregate();

            TransformationVms.Refresh();
        }

        private void RecalculateAggregate()
        {
            var globalAggregate = SampleValue;

            foreach (var transformation in TransformationVms)
            {
                globalAggregate = transformation.ApplyTransformation(globalAggregate);
            }
        }
    }
}
