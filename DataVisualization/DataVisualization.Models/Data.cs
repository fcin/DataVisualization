﻿using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Models
{
    public abstract class Data
    {
        [BsonId]
        public int DataId { get; set; }
        public string Name { get; set; }
    }

    public class ChartData : Data
    {
        [BsonRef("Series")]
        public List<Series> Series { get; set; }
        public int FileLinesRead { get; set; }

        private bool _transformationsApplied = false;

        public void ApplyTransformations()
        {
            if (_transformationsApplied)
                return;

            _transformationsApplied = !_transformationsApplied;

            foreach (var serie in Series.Where(s => s.Axis != Axes.X1 && s.Axis != Axes.X2))
            {
                serie.ApplyTransformations();
            }
        }
    }

    public class ScriptData : Data
    {
        public string Data { get; set; }
    }
}
