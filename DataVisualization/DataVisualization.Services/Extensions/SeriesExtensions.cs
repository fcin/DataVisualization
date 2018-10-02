using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Services.Extensions
{
    public static class SeriesExtensions
    {
        public static List<ValuesChunk> ToChunks(this List<double> values, int chunkSize)
        {
            return Enumerable.Range(0, (int)Math.Ceiling(values.Count / (double)chunkSize))
                             .Select(i => new ValuesChunk {
                                 Chunk = values.Skip(i * chunkSize).Take(chunkSize).ToList()
                             }).ToList();
        }


        public static void AddToChunks(this Series series, List<double> values, int chunkSize)
        {
            if (values.Count > chunkSize)
                throw new ArgumentException(nameof(values));

            if (values.Count == 0)
                return;

            if (series.Chunks.Count == 0 || series.Chunks.Last().Chunk.Count == chunkSize)
            {
                series.Chunks.Add(new ValuesChunk { Chunk = values });
                return;
            }

            var missing = chunkSize - series.Chunks.Last().Chunk.Count;

            if (values.Count <= missing)
            {
                series.Chunks.Last().Chunk.AddRange(values);
            }
            else
            {
                series.Chunks.Last().Chunk.AddRange(values.Take(missing));
                series.Chunks.Add(new ValuesChunk { Chunk = values.Skip(missing).ToList() });
            }
        }
    }
}
