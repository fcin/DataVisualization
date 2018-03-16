using DataVisualization.Models;
using LiteDB;

namespace DataVisualization.Services
{
    public class DataService
    {
        private const string DocumentDistinctName = "_DataName";
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
                if (collection.Exists(f => f[DocumentDistinctName].Equals(data.Name)))
                    return;

                var document = BsonMapper.Global.ToDocument(data);
                document.Add(DocumentDistinctName, data.Name);
                collection.Insert(document);
            }
        }

        public bool Exists(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection("Data");
                return collection.Exists(doc => doc[DocumentDistinctName].Equals(name));
            }
        }

        public Data GetData(string name)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<Data>("Data");
                return collection.FindOne(doc => doc.Name.Equals(name));
            }
        }
    }
}
