﻿using DataVisualization.Models;
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
            _dbPath = Settings.Instance.DbPath;
        }

        public void Add(DataConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.DataName) || configuration.Columns.Count == 0)
            {
                throw new ArgumentException("Configuration contains invalid values.");
            }

            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection("DataConfiguration");

                if (Exists(configuration.DataName))
                    throw new ArgumentException($"Document with name {configuration.DataName} already exists!");

                var document = BsonMapper.Global.ToDocument(configuration);
                document.Add(nameof(DataConfiguration.DataName), configuration.DataName);

                collection.Insert(document);
                collection.EnsureIndex(nameof(DataConfiguration.DataName));
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

        public DataConfiguration GetByName(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");
                return collection.Find(Query.EQ(nameof(DataConfiguration.DataName), name)).FirstOrDefault();
            }
        }
    }
}
