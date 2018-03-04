using CsvHelper;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
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

        public async Task<DataTable> GetSampleDataAsync(string filePath, int sampleSize)
        {
            var data = new DataTable();

            using (TextReader file = File.OpenText(filePath))
            {
                var reader = new CsvReader(file);
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;
                
                var index = 0;
                do
                {
                    await reader.ReadAsync();

                    if (data.Columns.Count == 0)
                    {
                        for (var colIndex = 0; colIndex < reader.Parser.Context.Record.Length; colIndex++)
                        {
                            data.Columns.Add(new DataColumn($"Column_{colIndex}", typeof(string)));
                        }
                    }

                    var row = data.NewRow();
                    var fieldIndex = 0;
                    while (reader.TryGetField(typeof(string), fieldIndex, out var field))
                    {
                        row[fieldIndex] = field;
                        fieldIndex++;
                    }
                    data.Rows.Add(row);

                    index++;
                } while (index < sampleSize);
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
