using DataVisualization.Models;
using LiteDB;
using System;

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

                foreach (var series in data.Series)
                {
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
                return collection.Include(d => d.Series).FindOne(Query.EQ(nameof(Data.Name), name));
            }
        }

        public void UpdateData(Data data)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>("Data");
                var existsInCollection = collection.Update(data);
                if(!existsInCollection)
                    throw new ArgumentException(nameof(data.Name));
            }
        }
    }
}
