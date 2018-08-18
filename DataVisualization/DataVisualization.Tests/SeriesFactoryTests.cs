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
            Assert.Throws<ArgumentException>(() => _seriesFactory.CreateSeriesPoints(new Series(), new Series(), long.MaxValue, long.MinValue));
        }

        [Test]
        public void CreateSeriesPoints_SeriesIsNull_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => _seriesFactory.CreateSeriesPoints(null, new Series(), long.MinValue, long.MaxValue));
            Assert.Throws<ArgumentNullException>(() => _seriesFactory.CreateSeriesPoints(new Series(), null, long.MinValue, long.MaxValue));
        }

        [Test]
        public void CreateSeriesPoints_HorizontalAxis_NotHorizontal_ShouldThrow()
        {
            var horizontalSeries = new Series {IsHorizontalAxis = false};
            Assert.Throws<ArgumentException>(() => _seriesFactory.CreateSeriesPoints(horizontalSeries, new Series(), long.MinValue, long.MaxValue));
        }

        [Test]
        public void CreateSeriesPoints_SortedAxis_ShouldReturnAllPoints()
        {
            var horizontalSeries = new Series
            {
                IsHorizontalAxis = true,
                Values = new List<double> { 1, 2, 3 }
            };

            var dataSeries = new Series { Values = new List<double> { 5, 8, 2 } };

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries, long.MinValue, long.MaxValue).ToList();

            Assert.AreEqual(horizontalSeries.Values.Count, points.Count);

            Assert.AreEqual(horizontalSeries.Values[0], points[0].DateTime.Ticks);
            Assert.AreEqual(dataSeries.Values[0], points[0].Value);

            Assert.AreEqual(horizontalSeries.Values[1], points[1].DateTime.Ticks);
            Assert.AreEqual(dataSeries.Values[1], points[1].Value);

            Assert.AreEqual(horizontalSeries.Values[2], points[2].DateTime.Ticks);
            Assert.AreEqual(dataSeries.Values[2], points[2].Value);
        }

        [Test]
        public void CreateSeriesPoints_UnsortedAxis()
        {
            var horizontalSeries = new Series
            {
                IsHorizontalAxis = true,
                Values = new List<double> { 3, 1, 2 }
            };

            var dataSeries = new Series { Values = new List<double> { 5, 8, 2 } };

            // Todo: Should I support unsorted axis? How to find correct range if axis is unsorted?

            var points = _seriesFactory.CreateSeriesPoints(horizontalSeries, dataSeries, long.MinValue, long.MaxValue).ToList();
        }
    }
}
