﻿using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
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
            get => (Color)(ColorConverter.ConvertFromString(_series.ColorHex) ?? Colors.Black);
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

        public IEnumerable<TransformationVisualName> AllTransformationDefinitionNames 
            => _availableTransformations.Select(trans => new TransformationVisualName(trans.Name, trans.PrettyName));

        private readonly IEnumerable<ITransformationViewModel> _availableTransformations =
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

            var transformations = series.Transformations.Select(TransformationViewModelFactory.Create);
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

            RecalculateAggregate();

            TransformationVms.Refresh();
        }

        public void TransformationChanged(ITransformationViewModel item, SelectionChangedEventArgs args)
        {
            if (item == null)
                return;

            if (args != null && args.AddedItems.Count != 0 && args.AddedItems[0] is TransformationVisualName name)
            {
                var index = TransformationVms.IndexOf(item);
                TransformationVms[index] = TransformationViewModelFactory.Create(name.Name);
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

    public class TransformationVisualName
    {
        public string Name { get; set; }
        public string PrettyName { get; set; }

        public TransformationVisualName(string name, string prettyName)
        {
            Name = name;
            PrettyName = prettyName;
        }
    }
}
