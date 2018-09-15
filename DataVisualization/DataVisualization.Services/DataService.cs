using DataVisualization.Models;
using LiteDB;
using System;
using System.Linq;

namespace DataVisualization.Services
{
    public class DataService
    {
        private readonly string _dbPath;

        public DataService()
        {
            _dbPath = Settings.Instance.DbPath;
        }

        public void AddData(Data data)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>("Data");
                if (Exists(data.Name))
                    return;

                var seriesColl = db.GetCollection<Series>("Series");
                var chunkColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                foreach (var series in data.Series)
                {
                    foreach (var chunk in series.Chunks)
                    {
                        chunk.ChunkId = chunkColl.Insert(chunk);
                    }

                    series.SeriesId = seriesColl.Insert(series);
                }

                collection.EnsureIndex(nameof(Data.Name));
                collection.Insert(data);
            }
        }

        public bool Exists(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                if (!db.CollectionExists("Data"))
                    return false;

                var collection = db.GetCollection("Data");
                return collection.Exists(Query.EQ(nameof(Data.Name), name));
            }
        }

        public Data GetData(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>("Data");
                var chunksColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                var data = collection.Include(d => d.Series).FindOne(Query.EQ(nameof(Data.Name), name));
                foreach (var series in data.Series)
                {
                    for (int index = series.Chunks.Count - 1; index >= 0; index--)
                    {
                        series.Chunks[index] = chunksColl.FindById(series.Chunks[index].ChunkId);
                    }
                }

                return data;
            }
        }

        public void DeleteDataByName(string dataName, IProgress<LoadingBarStatus> progress)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>(nameof(Data));
                var seriesColl = db.GetCollection<Series>(nameof(Series));
                var chunksColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                var data = collection.Include(d => d.Series).FindOne(Query.EQ(nameof(Data.Name), dataName));
                var interval = 100d / data.Series.Sum(s => s.Chunks.Count);
                var elapsed = 0d;
                foreach (var series in data.Series)
                {
                    foreach (var chunk in series.Chunks)
                    {
                        chunksColl.Delete(Query.EQ(nameof(ValuesChunk.ChunkId), chunk.ChunkId));
                        elapsed += interval;
                        progress.Report(new LoadingBarStatus { PercentFinished = (int)elapsed, Message = "Deleting data..." });
                    }
                    seriesColl.Delete(Query.EQ(nameof(Series.SeriesId), series.SeriesId));
                }

                collection.Delete(Query.EQ(nameof(Data.Name), dataName));
            }
        }

        public void UpdateData(Data data)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>("Data");
                var serieColl = db.GetCollection<Series>(nameof(Series));
                var chunkColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                var existsInCollection = collection.Update(data);
                if(!existsInCollection)
                    throw new ArgumentException(nameof(data.Name));

                foreach (var serie in data.Series)
                {
                    foreach (var chunk in serie.Chunks)
                    {
                        if (chunk.ChunkId == 0)
                        {
                            chunk.ChunkId = chunkColl.Insert(chunk);
                        }
                        else
                        {
                            chunkColl.Update(chunk);
                        }
                    }
                    serieColl.Update(serie);
                }
            }
        }

        public Series GetSeriesByName(string seriesName)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Series>(nameof(Series));
                return collection.FindOne(Query.EQ(nameof(Series.Name), seriesName));
            }
        }

        public void UpdateSeries(Series series)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Series>(nameof(Series));
                collection.Update(series);
            }
        }

        public bool SeriesWithNameExists(string seriesName)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Series>(nameof(Series));
                return collection.Exists(Query.EQ(nameof(Series.Name), seriesName));
            }
        }
    }
}
