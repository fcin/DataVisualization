using DataVisualization.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace DataVisualization.Tests
{
    [TestFixture]
    public class SeriesExtensionsTests
    {
        private Series _series;

        [SetUp]
        public void Setup()
        {
            _series = new Series
            {
                Chunks = new List<ValuesChunk>()
            };
        }
    }
}
