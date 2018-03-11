using System;
using CsvHelper;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using DataVisualization.Models;
using DataColumn = System.Data.DataColumn;

namespace DataVisualization.Services
{
    public class DataService : IDataService
    {
        public int ColumnsCount { get; set; }

        public async Task<IEnumerable<List<object>>> GetDataAsync(DataConfiguration config)
        {
            var path = config.FilePath;

            using (TextReader file = File.OpenText(path))
            using(var reader = new CsvReader(file))
            {
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                ColumnsCount = reader.Context.Record.Length;
                var data = new List<List<object>>(ColumnsCount);
                for (var index = ColumnsCount - 1; index >= 0; index--)
                {
                    data.Add(new List<object>());
                }
                
                while (await reader.ReadAsync())
                {
                    for (var index = 0; index < reader.Context.Record.Length; index++)
                    {
                        var value = reader.Context.Record[index];
                        var convertedValue = Convert.ChangeType(value, Type.GetType(config.Columns[index].ColumnType));
                        data[index].Add(convertedValue);
                    }
                }

                return data;
            }
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
    }
}
