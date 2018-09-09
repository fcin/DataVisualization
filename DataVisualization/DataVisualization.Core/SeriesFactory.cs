using System;
using System.Collections.Generic;
using System.Linq;
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
        IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, double? min = null, double? max = null);
    }

    public class SeriesFactory : ISeriesFactory
    {
        private const int PointsCount = 10_000;
        private readonly CartesianMapper<DateModel> _dayConfig = Mappers.Xy<DateModel>()
            .X(dayModel => dayModel.HorizontalAxis)
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
                Stroke = new SolidColorBrush(series.SeriesColor),
                Title = series.Name,
                ScalesYAt = series.Axis == Axes.Y1 ? 0 : 1
            };
        }

        public IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, double? min = null, double? max = null)
        {
            if(min >= max)
                throw new ArgumentException($"{nameof(min)} is bigger of equal to {nameof(max)}");

            if(horizontalSeries == null || dataSeries == null)
                throw new ArgumentNullException($"{nameof(horizontalSeries)} or {nameof(dataSeries)} is null");

            if(horizontalSeries.Axis != Axes.X1)
                throw new ArgumentException(nameof(horizontalSeries));

            var dateRow = horizontalSeries;
            var row = dataSeries;

            var minIndex = 0;
            var maxIndex = 0;

            if (min == null || max == null)
            {
                minIndex = 0;
                maxIndex = dateRow.Values.Count;
            }
            else
            {
                var nearestMin = dateRow.Values.Aggregate((x, y) => Math.Abs(x - min.Value) < Math.Abs(y - min.Value) ? x : y);
                var nearestMax = dateRow.Values.Aggregate((x, y) => Math.Abs(x - max.Value) < Math.Abs(y - max.Value) ? x : y);
                minIndex = dateRow.Values.IndexOf(nearestMin);
                maxIndex = dateRow.Values.IndexOf(nearestMax) + 1; // inclusive
            }

            var increment = Math.Max((maxIndex - minIndex) / PointsCount, 1);

            var seriesPoints = new List<DateModel>();
            for (var cellIndex = minIndex; cellIndex < maxIndex; cellIndex += increment)
            {
                var x = (long)dateRow.Values[cellIndex];
                seriesPoints.Add(new DateModel
                {
                    HorizontalAxis = x,
                    Value = row.Values[cellIndex]
                });
            }

            return seriesPoints;
        }
    }
}
