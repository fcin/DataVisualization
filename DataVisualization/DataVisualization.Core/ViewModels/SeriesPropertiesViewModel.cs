﻿using Caliburn.Micro;
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

        private BindableCollection<ITransformationViewModel> _transformations;
        public BindableCollection<ITransformationViewModel> TransformationVms
        {
            get => _transformations;
            set => Set(ref _transformations, value);
        }

        public IEnumerable<string> AllTransformationDefinitionNames => _availableTransformations.Select(trans => trans.Name);

        private IEnumerable<ITransformationViewModel> _availableTransformations = new ITransformationViewModel[] {
            new AddTransformationViewModel(new AddTransformation(0)),
            new SubtractTransformationViewModel(new SubtractTransformation(0))
        };

        private readonly string _oldName;
        private readonly Series _series;
        private readonly DataService _dataService;

        public SeriesPropertiesViewModel(Series series, DataService dataService)
        {
            _series = series;
            _dataService = dataService;
            _oldName = series.Name;

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
            TransformationVms.Add(new AddTransformationViewModel(new AddTransformation(0)));
        }

        public void TransformationChanged(ITransformationViewModel item, SelectionChangedEventArgs args)
        {
            if (item == null)
                return;

            if ((args != null && args.RemovedItems.Count != 0))
            {
                var index = TransformationVms.IndexOf(item);
                switch (args.RemovedItems[0].ToString())
                {
                    case "Add":
                        TransformationVms[index] = new AddTransformationViewModel(new AddTransformation(0));
                        break;
                    case "Subtract":
                        TransformationVms[index] = new SubtractTransformationViewModel(new SubtractTransformation(0));
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            RecalculateAggregate();
            
            TransformationVms.Refresh();
        }

        private void RecalculateAggregate()
        {
            var globalAggregate = 0d;

            foreach (var transformation in TransformationVms)
            {
                globalAggregate = transformation.ApplyTransformation(globalAggregate);
            }
        }
    }
}
