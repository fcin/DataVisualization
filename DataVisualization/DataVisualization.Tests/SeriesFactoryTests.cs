using System;
using System.Collections.Generic;
using System.Linq;
using DataVisualization.Core;
using DataVisualization.Models;
using NUnit.Framework;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class SeriesFactoryTests
    {
        private ISeriesFactory _seriesFactory;

        [SetUp]
        public void Setup()
        {
            _seriesFactory = new SeriesFactory();
        }

        [Test]
        public void CreateSeriesPoints_MinIsBiggerOrEqualToMax_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => _seriesFactory.CreateSeriesPoints(new Series(), new Series()));
        }

        [Test]
        public void CreateSeriesPoints_SeriesIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _seriesFactory.CreateSeriesPoints(null, new Series()));
            Assert.Throws<ArgumentNullException>(() => _seriesFactory.CreateSeriesPoints(new Series(), null));
        }

        [Test]
        public void CreateSeriesPoints_HorizontalAxis_NotHorizontal_ShouldThrow()
        {
            var horizontalSeries = new Series { Axis = Axes.Y2 };
            Assert.Throws<ArgumentException>(() => _seriesFactory.CreateSeriesPoints(horizontalSeries, new Series()));
        }

        [Test]
        public void CreateSeriesPoints_SortedHorizontalAxis_ShouldReturnAllPoints()
        {
            var horizontalSeries = new Series
            {
                Axis = Axes.X1,
                Values = new List<double> { 1, 2, 3 }
            };

            var dataSeries = new Series { Values = new List<double> { 5, 8, 2 } };

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries).ToList();

            Assert.AreEqual(horizontalSeries.Values.Count, points.Count);

            Assert.AreEqual(horizontalSeries.Values[0], points[0].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[1], points[1].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[2], points[2].HorizontalAxis);
            Assert.AreEqual(dataSeries.Values[0], points[0].Value);
            Assert.AreEqual(dataSeries.Values[1], points[1].Value);
            Assert.AreEqual(dataSeries.Values[2], points[2].Value);
        }

        [Test]
        public void CreateSeriesPoints_UnsortedHorizontalAxis()
        {
            var horizontalSeries = new Series
            {
                Axis = Axes.X1,
                Values = new List<double> { 3, 1, 2 }
            };

            var dataSeries = new Series { Values = new List<double> { 5, 8, 2 } };

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries).ToList();

            Assert.AreEqual(horizontalSeries.Values[0], points[0].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[1], points[1].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[2], points[2].HorizontalAxis);
            Assert.AreEqual(dataSeries.Values[0], points[0].Value);
            Assert.AreEqual(dataSeries.Values[1], points[1].Value);
            Assert.AreEqual(dataSeries.Values[2], points[2].Value);
        }

        [Test]
        public void CreateSeriesPoints_SortedHorizontalAxis_MinAndMaxRangeSpecified_ShouldReturnPointsWithinRangeInclusive()
        {
            const long min = 2;
            const long max = 4;
            var horizontalSeries = new Series
            {
                Axis = Axes.X1,
                Values = new List<double> { 1, 2, 3, 4, 5 }
            };
            var dataSeries = new Series { Values = new List<double> { 1, 2, 3, 4, 5 } };

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries, min, max).ToList();

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(horizontalSeries.Values[1], points[0].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[2], points[1].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[3], points[2].HorizontalAxis);
            Assert.AreEqual(dataSeries.Values[1], points[0].Value);
            Assert.AreEqual(dataSeries.Values[2], points[1].Value);
            Assert.AreEqual(dataSeries.Values[3], points[2].Value);
        }

        [Test]
        public void CreateSeriesPoints_UnsortedHorizontalAxis_MinAndMaxRangeSpecified_ShouldReturnPointsWithinRangeInclusive()
        {
            const long min = 2;
            const long max = 7;
            var horizontalSeries = new Series
            {
                Axis = Axes.X1,
                Values = new List<double> { 3, 1, 2, 5, 7, 6, 4 }
            };
            var dataSeries = new Series { Values = new List<double> { 1, 2, 3, 4, 5, 6, 7 } };

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries, min, max).ToList();

            Assert.AreEqual(3, points.Count);
            Assert.AreEqual(horizontalSeries.Values[2], points[0].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[3], points[1].HorizontalAxis);
            Assert.AreEqual(horizontalSeries.Values[4], points[2].HorizontalAxis);
            Assert.AreEqual(dataSeries.Values[2], points[0].Value);
            Assert.AreEqual(dataSeries.Values[3], points[1].Value);
            Assert.AreEqual(dataSeries.Values[4], points[2].Value);
        }
    }
}
