using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Models.Transformations;
using DataVisualization.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

        private BindableCollection<TransformationListItem> _transformations;
        public BindableCollection<TransformationListItem> Transformations
        {
            get => _transformations;
            set => Set(ref _transformations, value);
        }

        public IEnumerable<string> AllTransformationDefinitionNames => _availableTransformations.Select(trans => trans.Transformation.Name);

        private IEnumerable<TransformationListItem> _availableTransformations = new TransformationListItem[] {
            new TransformationListItem { Value = 0, Transformation = new AddTransformation(0) },
            new TransformationListItem { Value = 0, Transformation = new SubtractTransformation(0) }
        };

        private readonly string _oldName;
        private readonly Series _series;
        private readonly DataService _dataService;

        public SeriesPropertiesViewModel(Series series, DataService dataService)
        {
            _series = series;
            _dataService = dataService;
            _oldName = series.Name;

            Transformations = new BindableCollection<TransformationListItem>
            {
                new TransformationListItem { Name = "Add", Value = 0, Transformation = new AddTransformation(0) }
            };
        }

        public void OnSave()
        {
            if (SeriesName != _oldName && _dataService.SeriesWithNameExists(SeriesName))
            {
                MessageBox.Show($"Series with name {SeriesName} already exists", "Series name already exists", MessageBoxButton.OK);
                return;
            }

            _dataService.UpdateSeries(_series);

            TryClose(true);
        }

        public void OnCancel()
        {
            TryClose(false);
        }

        public void AddTransformation()
        {
            Transformations.Add(new TransformationListItem { Name = "Add", Value = 0, Transformation = new AddTransformation(0) });
        }

        public void TransformationChanged(TransformationListItem item)
        {
            if (item == null)
                return;

            switch (item.Name)
            {
                case "Add":
                    item.Transformation = new AddTransformation(item.Value);
                    break;
                case "Subtract":
                    item.Transformation = new SubtractTransformation(item.Value);
                    break;
                default:
                    throw new ArgumentException();
            }

            var globalAggregate = 0d;

            foreach (var transformation in Transformations)
            {
                globalAggregate = transformation.Transformation.Transform(globalAggregate);
                transformation.Aggregate = globalAggregate;
            }
            
            Transformations.Refresh();
        }
    }

    public class TransformationListItem
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double Aggregate { get; set; }
        public ITransformation Transformation { get; set; }
    }
}
