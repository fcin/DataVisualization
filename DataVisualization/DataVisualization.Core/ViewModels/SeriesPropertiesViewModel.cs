﻿using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Models.Transformations;
using DataVisualization.Services;
using System.Collections.Generic;
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

        private List<TransformationDefinition> _transformations;
        public List<TransformationDefinition> Transformations
        {
            get => _transformations;
            set => Set(ref _transformations, value);
        }

        private readonly string _oldName;
        private readonly Series _series;
        private readonly DataService _dataService;

        public SeriesPropertiesViewModel(Series series, DataService dataService)
        {
            _series = series;
            _dataService = dataService;
            _oldName = series.Name;

            Transformations = new List<TransformationDefinition>
            {
                TransformationDefinitionFactory.GetDefinition("Add")
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
    }
}
