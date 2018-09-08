using LiteDB;
using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class Data
    {
        [BsonId]
        public int DataId { get; set; }
        public string Name { get; set; }
        [BsonRef("Series")]
        public List<Series> Series { get; set; }
        public int FileLinesRead { get; set; }
    }
}
