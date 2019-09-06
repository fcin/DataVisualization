using DataVisualization.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Services
{
    public class DataService
    {
        private readonly string _dbPath;
        private static Dictionary<string, Data> _cache;

        public DataService()
        {
            _dbPath = GlobalSettings.DbPath;
            _cache = new Dictionary<string, Data>();
        }

        public void AddData<T>(T data) where T : Data
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                if (typeof(T) == typeof(ChartData))
                {
                    var chartData = data as ChartData;

                    var collection = db.GetCollection<ChartData>("Data");
                    if (Exists<T>(chartData.Name))
                        return;

                    var seriesColl = db.GetCollection<Series>("Series");
                    var chunkColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                    foreach (var series in chartData.Series)
                    {
                        foreach (var chunk in series.Chunks)
                        {
                            chunk.ChunkId = chunkColl.Insert(chunk);
                        }

                        series.SeriesId = seriesColl.Insert(series);
                    }

                    collection.EnsureIndex(nameof(ChartData.Name));
                    collection.Insert(chartData);
                }
                else
                {
                    var collection = db.GetCollection<T>();
                    collection.EnsureIndex(nameof(Data.Name));
                    collection.Insert(data);
                }
            }
        }

        public bool Exists<T>(string name) where T : Data
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<T>();
                return collection.Exists(Query.EQ(nameof(ChartData.Name), name));
            }
        }

        public T GetData<T>(string name) where T : Data
        {
            if (_cache.ContainsKey(name))
                return (T)_cache[name];

            using (var db = new LiteDatabase(_dbPath))
            {
                if (typeof(T) == typeof(ChartData))
                {
                    var collection = db.GetCollection<ChartData>("Data");
                    var chunksColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                    var data = collection.Include(d => d.Series).FindOne(Query.EQ(nameof(ChartData.Name), name));
                    foreach (var series in data.Series)
                    {
                        for (int index = 0; index < series.Chunks.Count; index++)
                        {
                            var chunk = chunksColl.FindById(series.Chunks[index].ChunkId);
                            series.ChangeChunk(index, chunk);
                        }
                    }

                    _cache.Add(name, data);
                    return data as T;
                }
                else
                {
                    var collection = db.GetCollection<T>();
                    return collection.FindOne(Query.EQ(nameof(Data.Name), name));
                }
            }
        }

        public void DeleteDataByName(string dataName, IProgress<LoadingBarStatus> progress)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<ChartData>(nameof(ChartData));
                var seriesColl = db.GetCollection<Series>(nameof(Series));
                var chunksColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                var data = collection.Include(d => d.Series).FindOne(Query.EQ(nameof(ChartData.Name), dataName));
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

                collection.Delete(Query.EQ(nameof(ChartData.Name), dataName));
            }
        }

        public void UpdateData<T>(T data) where T : Data
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                if (typeof(T) == typeof(ChartData))
                {
                    var chartData = data as ChartData;
                
                    var collection = db.GetCollection<ChartData>("Data");
                    var serieColl = db.GetCollection<Series>(nameof(Series));
                    var chunkColl = db.GetCollection<ValuesChunk>(nameof(ValuesChunk));

                    var existsInCollection = collection.Update(chartData);
                    if(!existsInCollection)
                        throw new ArgumentException(nameof(chartData.Name));

                    foreach (var serie in chartData.Series)
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
                else
                {
                    var collection = db.GetCollection<T>();
                    collection.Update(data);
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

            var keyToUpdate = _cache.OfType<KeyValuePair<string, ChartData>>().FirstOrDefault(s => s.Value.Series.Any(a => a.SeriesId == series.SeriesId)).Key;
            if (keyToUpdate != null)
            {
                var data = (ChartData)_cache[keyToUpdate];
                var index = data.Series.FindIndex(s => s.SeriesId == series.SeriesId);
                data.Series[index] = series;
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
