using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class Data
    {
        public string Name { get; set; }
        public IEnumerable<IEnumerable<object>> Values { get; set; }
    }
}
