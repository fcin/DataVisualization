using System;
using System.Threading.Tasks;
using DataVisualization.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DataVisualization.Services
{
    public class DataConfigurationService
    {
        public async Task AddConfigurationAsync(DataConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration.DataName) || configuration.Columns.Count == 0)
            {
                throw new ArgumentException("Configuration contains invalid values.");
            }

            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("DataVisualizationDb");

            var collection = db.GetCollection<BsonDocument>("DataConfiguration");

            var document = new BsonDocument { ["DataName"] = configuration.DataName };
            foreach (var column in configuration.Columns)
            {
                document[column.Name] = column.ColumnType;
            }

            await collection.InsertOneAsync(document);
        }
    }
}
