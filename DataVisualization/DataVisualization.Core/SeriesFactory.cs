using System;
using System.Collections.Generic;
using System.Windows.Media;
using DataVisualization.Models;
using LiveCharts.Configurations;
using LiveCharts.Definitions.Series;
using LiveCharts.Geared;

namespace DataVisualization.Core
{
    public interface ISeriesFactory
    {
        ISeriesView CreateLineSeries(IEnumerable<DateModel> values, Series series);

        /// <summary>
        /// Returns a series of points with X with values from Horizontal Axis and Y from specified dataSeries.
        /// </summary>
        /// <param name="horizontalSeries">Data for X values</param>
        /// <param name="dataSeries">Data for Y values</param>
        /// <param name="min">Inclusive start of range</param>
        /// <param name="max">Exclusive start of range</param>
        IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, long? min = null, long? max = null);
    }

    public class SeriesFactory : ISeriesFactory
    {
        private const int PointsCount = 10_000;
        private readonly CartesianMapper<DateModel> _dayConfig = Mappers.Xy<DateModel>()
            .X(dayModel => dayModel.DateTime.Ticks)
            .Y(dayModel => dayModel.Value);


        public ISeriesView CreateLineSeries(IEnumerable<DateModel> values, Series series)
        {
            var gearedValues = new GearedValues<DateModel> { Quality = Quality.Low };
            gearedValues.AddRange(values);

            return new GLineSeries(_dayConfig)
            {
                Values = gearedValues,
                Fill = Brushes.Transparent,
                PointGeometry = null,
                LineSmoothness = 0,
                DataLabels = false,
                Stroke = new SolidColorBrush(series.SeriesColor)
            };
        }

        public IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, long? min = null, long? max = null)
        {
            if(min >= max)
                throw new ArgumentException($"{nameof(min)} is bigger of equal to {nameof(max)}");

            if(horizontalSeries == null || dataSeries == null)
                throw new ArgumentNullException($"{nameof(horizontalSeries)} or {nameof(dataSeries)} is null");

            if(!horizontalSeries.IsHorizontalAxis)
                throw new ArgumentException(nameof(horizontalSeries));

            var dateRow = horizontalSeries;
            var row = dataSeries;
            
            var minIndex = min == null ? 0 : dateRow.Values.IndexOf(min.Value);
            var maxIndex = max == null ? dateRow.Values.Count : dateRow.Values.IndexOf(max.Value);
            var increment = Math.Max((maxIndex - minIndex) / PointsCount, 1);

            var seriesPoints = new List<DateModel>();
            for (var cellIndex = minIndex; cellIndex < maxIndex; cellIndex += increment)
            {
                var x = new DateTime((long)dateRow.Values[cellIndex]);
                seriesPoints.Add(new DateModel
                {
                    DateTime = x,
                    Value = row.Values[cellIndex]
                });
            }

            return seriesPoints;
        }
    }
}
