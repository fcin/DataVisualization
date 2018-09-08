using LiteDB;
using System.Collections.Generic;
using System.Windows.Media;


namespace DataVisualization.Models
{
    public class Series
    {
        [BsonId]
        public int SeriesId { get; set; }
        public IList<double> Values { get; set; }
        public Color SeriesColor { get; set; }
        public ColumnTypeDef InternalType { get; set; }
        public string Name { get; set; }
        public Axes Axis { get; set; }
    }
}
