using LiteDB;
using System.Collections.Generic;
using System.Linq;


namespace DataVisualization.Models
{
    public class Series
    {
        [BsonId]
        public int SeriesId { get; set; }

        [BsonIgnore]
        private List<double> _values { get; set; }
        [BsonIgnore]
        public List<double> Values => GetValues();

        [BsonRef("Chunks")]
        public List<ValuesChunk> Chunks { get; set; }

        public string ColorHex { get; set; }

        public int Thickness { get; set; }

        public ColumnTypeDef InternalType { get; set; }

        public string Name { get; set; }

        public Axes Axis { get; set; }

        private List<double> GetValues()
        {
            if (_values == null || Chunks.Sum(c => c.Chunk.Count) != _values.Count)
                _values = Chunks.SelectMany(c => c.Chunk).ToList();
            return _values;
        }
    }

    public class ValuesChunk
    {
        [BsonId]
        public int ChunkId { get; set; }
        public List<double> Chunk { get; set; }
    }
}
