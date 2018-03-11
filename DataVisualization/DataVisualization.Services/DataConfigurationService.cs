using DataVisualization.Models;
using LiteDB;
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace DataVisualization.Services
{
    public class DataConfigurationService
    {
        private readonly string _dbPath;
        private const string DataConfigurationDocumentName = "_DataConfigurationName";

        public DataConfigurationService()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data.db");
        }

        public void AddConfigurationAsync(DataConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.DataName) || configuration.Columns.Count == 0)
            {
                throw new ArgumentException("Configuration contains invalid values.");
            }
            
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection("DataConfiguration");

                if(collection.Exists(doc => doc[DataConfigurationDocumentName].Equals(configuration.DataName)))
                    throw new ArgumentException($"Document with name {configuration.DataName} already exists!");

                var document = BsonMapper.Global.ToDocument(configuration);
                collection.Insert(document);
                collection.EnsureIndex(DataConfigurationDocumentName);
            }
        }

        public bool ConfigurationExists(string configName)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection("DataConfiguration");
                return collection.Exists(x => x[DataConfigurationDocumentName].Equals(configName));
            }
        }

        public DataConfiguration Get(Expression<Func<DataConfiguration, bool>> predicate)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<DataConfiguration>("DataConfiguration");
                return collection.Find(predicate).FirstOrDefault();
            }
        }
    }
}
