using System.Collections.Generic;
using System.Windows.Media;


namespace DataVisualization.Models
{
    public class Series
    {
        public IList<double> Values { get; set; }
        public Color SeriesColor { get; set; }
    }
}
