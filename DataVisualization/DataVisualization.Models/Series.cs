using DataVisualization.Models.Transformations;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DataVisualization.Models
{
    public class Series
    {
        [BsonId]
        public int SeriesId { get; set; }

        [BsonIgnore]
        private int _chunksCountCache;

        [BsonIgnore]
        private List<double> _values;
        [BsonIgnore]
        public List<double> Values
        {
            get
            {
                if (_values == null || _chunksCountCache == 0 || _chunksCountCache != _values.Count)
                {
                    _values = Chunks.SelectMany(c => c.Chunk).ToList();
                    _chunksCountCache = _values.Count;
                }
                return _values;
            }
        }

        private List<ValuesChunk> _chunks;
        [BsonRef("Chunks")]
        public List<ValuesChunk> Chunks
        {
            get => _chunks;
            set
            {
                _chunks = value.ToList();
                _chunksCountCache = _chunks.Count;
            }
        }

        public string ColorHex { get; set; }

        public int Thickness { get; set; }

        public ColumnTypeDef InternalType { get; set; }

        public string Name { get; set; }

        public Axes Axis { get; set; }

        private readonly IList<ITransformation> _transformations;

        [BsonIgnore]
        private bool _transformationsApplied = false;

        public Series()
        {
            _transformations = new List<ITransformation>();
        }

        public void ApplyTransformations()
        {
            if (_transformationsApplied)
                return;

            _transformationsApplied = !_transformationsApplied;

            for (int index = 0; index < Values.Count; index++)
            {
                foreach (var transformation in _transformations)
                {
                    Values[index] = transformation.Transform(Values[index]);
                }
            }
        }

        public void ChangeChunk(int chunkIndex, ValuesChunk chunk)
        {
            if (chunkIndex < 0 || chunkIndex >= _chunks.Count)
                throw new IndexOutOfRangeException(nameof(chunkIndex));

            var oldCount = _chunks[chunkIndex].Chunk?.Count ?? 0;
            _chunks[chunkIndex] = chunk;
            _chunksCountCache += (chunk.Chunk.Count - oldCount);
        }

        public void AddChunk(ValuesChunk chunk)
        {
            _chunks.Add(chunk);
            _chunksCountCache += chunk.Chunk.Count;
        }
    }

    public class ValuesChunk
    {
        [BsonId]
        public int ChunkId { get; set; }
        public List<double> Chunk { get; set; }
    }
}
