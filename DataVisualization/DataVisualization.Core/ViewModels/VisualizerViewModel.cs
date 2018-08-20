﻿using Caliburn.Micro;
using DataVisualization.Models;
using DataVisualization.Services;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DataVisualization.Core.ViewModels
{
    public class VisualizerViewModel : Screen
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();

        private Func<double, string> _formatterX = null;
        public Func<double, string> FormatterX
        {
            get
            {
                if (_formatterX == null)
                {
                    var xLineType = _data?.First(d => d.IsHorizontalAxis).InternalType;
                    if (xLineType == null)
                        return val => val.ToString(CultureInfo.CurrentCulture);
                    switch (xLineType)
                    {
                        case "System.Double":
                            _formatterX = val => val.ToString(CultureInfo.CurrentCulture);
                            break;
                        case "System.DateTime":
                            _formatterX = val => new DateTime((long)val).ToString("MM/dd/yyyy");
                            break;
                        default:
                            throw new ArgumentException("Unsupported type");
                    }
                }

                return _formatterX;
            }
        }

        private long? _minX = null;
        public long? MinX
        {
            get => _minX;
            set => SetValue(ref _minX, value);
        }

        private long? _maxX = null;
        public long? MaxX
        {
            get => _maxX;
            set => SetValue(ref _maxX, value);
        }

        private readonly ISeriesFactory _seriesFactory;
        private readonly DataFileReader _dataFileReader = new DataFileReader();
        private readonly DataService _dataService = new DataService();
        private readonly DataConfigurationService _dataConfigurationService = new DataConfigurationService();
        private List<Series> _data;

        public VisualizerViewModel(ISeriesFactory seriesFactory)
        {
            _seriesFactory = seriesFactory;
        }

        public void OnRangeChanged(long newMin, long newMax)
        {
            RecreateSeries();
        }

        protected override async void OnActivate()
        {
            var config = _dataConfigurationService.Get(conf => conf.DataName.Equals("CsvData"));

            if (config == null)
                return;

            if (!_dataService.Exists(config.DataName))
            {
                var loadedData = await _dataFileReader.ReadDataAsync(config);
                _dataService.AddData(loadedData);
            }

            _data = _dataService.GetData(config.DataName).Series.ToList();

            MinX = (long)_data[0].Values[0];
            MaxX = (long)_data[0].Values[_data[0].Values.Count - 1];

            RecreateSeries();

            base.OnActivate();
        }

        private void RecreateSeries()
        {
            foreach (var series in SeriesCollection)
                series.Values.Clear();

            var horizontalAxisSeries = _data.First(d => d.IsHorizontalAxis);
            foreach (var series in _data.Where(s => !s.IsHorizontalAxis))
            {
                var points = _seriesFactory.CreateSeriesPoints(horizontalAxisSeries, series, MinX, MaxX);
                var seriesView = _seriesFactory.CreateLineSeries(points, series);
                SeriesCollection.Add(seriesView);
            }
        }

        private void SetValue<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return;

            oldValue = newValue;
            NotifyOfPropertyChange(propertyName);
        }
    }
}
