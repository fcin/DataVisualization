using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DataVisualization.Services
{
    public class DataService : IDataService
    {

        public double Highest { get; set; } = double.MinValue;
        public double Lowest { get; set; } = double.MaxValue;
        public int ColumnsCount { get; set; }
        public int AverageCount => _count / (ColumnsCount * 100);

        private int _count;

        public async Task<IEnumerable<double>> GetDataAsync()
        {
            var data = new List<double>();
            var path = GetFilePath();

            using (TextReader file = File.OpenText(path))
            {
                var reader = new CsvReader(file);
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                ColumnsCount = reader.Context.Record.Length;
                
                while (await reader.ReadAsync())
                {
                    var record = (double)reader.GetField(typeof(double), 2);
                    data.Add(record);

                    if (record > Highest)
                        Highest = record;
                    else if (record < Lowest)
                        Lowest = record;
                }
            }

            _count += data.Count;
            return data;
        }

        public async Task<IEnumerable<string>> GetSampleDataAsync(string filePath, int sampleSize)
        {
            var data = new List<string>();
            
            using (TextReader file = File.OpenText(filePath))
            {
                var reader = new CsvReader(file);
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                for (var index = 0; index < sampleSize; index++)
                {
                    await reader.ReadAsync();
                    var record = reader.GetField(typeof(string), 2).ToString();
                    data.Add(record);
                }
            }

            return data;
        }

        private string GetFilePath()
        {
#if DEBUG
            var start = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            return Path.GetFullPath(Path.Combine(start, @"..\", "TestFiles", "VisualizationData", "CsvData.csv"));
#else
            throw new NotImplementedException();
#endif
        }
    }
}
