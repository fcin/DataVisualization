using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Series> Series { get; set; }
        public int FileLinesRead { get; set; }
    }
}
