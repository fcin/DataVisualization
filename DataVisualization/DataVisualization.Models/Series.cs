using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace DataVisualization.Models
{
    public class Series
    {
        public Guid Id { get; set; }
        public bool IsHorizontalAxis { get; set; }
        public IList<double> Values { get; set; }
        public Color SeriesColor { get; set; }
        public string InternalType { get; set; }
    }
}
