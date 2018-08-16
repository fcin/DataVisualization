using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class Data
    {
        public string Name { get; set; }
        public IList<Series> Series { get; set; }
    }
}
