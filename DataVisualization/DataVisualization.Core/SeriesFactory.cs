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
        IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, long min, long max);
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

        public IEnumerable<DateModel> CreateSeriesPoints(Series horizontalSeries, Series dataSeries, long min, long max)
        {
            if(min >= max)
                throw new ArgumentException($"{nameof(min)} is bigger of equal to {nameof(max)}");

            if(horizontalSeries == null || dataSeries == null)
                throw new ArgumentNullException($"{nameof(horizontalSeries)} or {nameof(dataSeries)} is null");

            if(!horizontalSeries.IsHorizontalAxis)
                throw new ArgumentException(nameof(horizontalSeries));

            var dateRow = horizontalSeries;
            var row = dataSeries;

            var (minIndex, maxIndex) = GetMinAndMaxIndex(dateRow.Values, min, max);
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

        private (int minIndex, int maxIndex) GetMinAndMaxIndex(IList<double> dateRowValues, long min, long max)
        {
            var foundMin = 0;
            for (var index = 0; index < dateRowValues.Count; index++)
            {
                var item = dateRowValues[index];
                if (item > min)
                {
                    foundMin = index;
                    break;
                }
            }

            for (int index = foundMin; index < dateRowValues.Count; index++)
            {
                var item = dateRowValues[index];

                if (item > max)
                    return (foundMin, index);
            }

            return (foundMin, dateRowValues.Count);
        }
    }
}
