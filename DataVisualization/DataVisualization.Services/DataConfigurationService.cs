using DataVisualization.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataVisualization.Services
{
    public class DataConfigurationService
    {
        private readonly string _dbPath;

        public DataConfigurationService()
        {
            _dbPath = GlobalSettings.DbPath;
        }

        public void Add(DataConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.DataName) || configuration.Columns.Count == 0)
            {
                throw new ArgumentException("Configuration contains invalid values.");
            }

            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");

                if (Exists(configuration.DataName))
                    throw new ArgumentException($"Document with name {configuration.DataName} already exists!");

                collection.EnsureIndex(nameof(DataConfiguration.DataName));
                collection.Insert(configuration);
            }
        }

        public IEnumerable<DataConfiguration> GetAll()
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");
                return collection.FindAll();
            }
        }

        public bool Exists(string configName)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                if (!db.CollectionExists("DataConfiguration"))
                    return false;

                var collection = db.GetCollection("DataConfiguration");
                return collection.Exists(Query.EQ(nameof(DataConfiguration.DataName), configName));
            }
        }

        public void DeleteConfigurationByName(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                if (!db.CollectionExists("DataConfiguration"))
                    throw new ArgumentException(nameof(name));

                var collection = db.GetCollection("DataConfiguration");
                collection.Delete(Query.EQ(nameof(DataConfiguration.DataName), name));
            }
        }

        public DataConfiguration GetByName(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");
                return collection.Find(Query.EQ(nameof(DataConfiguration.DataName), name)).FirstOrDefault();
            }
        }

        public bool Update(DataConfiguration dataConfiguration)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");
                return collection.Update(dataConfiguration);
            }
        }
    }
}
