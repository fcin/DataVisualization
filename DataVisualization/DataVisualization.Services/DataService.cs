using DataVisualization.Models;
using LiteDB;

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
                var collection = db.GetCollection("Data");
                if (Exists(data.Name))
                    return;

                var document = BsonMapper.Global.ToDocument(data);
                document.Add(nameof(Data.Name), data.Name);

                collection.EnsureIndex(nameof(Data.Name));
                collection.Insert(document);
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
                return collection.FindOne(Query.EQ(nameof(Data.Name), name));
            }
        }
    }
}
