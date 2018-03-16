using CsvHelper;
using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace DataVisualization.Services
{
    public class DataFileReader : IDataFileReader
    {
        public int ColumnsCount { get; set; }

        public async Task<IEnumerable<List<object>>> ReadDataAsync(DataConfiguration config)
        {
            var path = config.FilePath;

            using (TextReader file = File.OpenText(path))
            using (var reader = new CsvReader(file))
            {
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                ColumnsCount = reader.Context.Record.Length;
                var data = new List<List<object>>(config.Columns.Count);
                for (var index = config.Columns.Count - 1; index >= 0; index--)
                {
                    data.Add(new List<object>());
                }

                while (await reader.ReadAsync())
                {
                    for (var index = 0; index < config.Columns.Count; index++)
                    {
                        var configColumn = config.Columns[index];
                        var fileColumnIndex = configColumn.Index;
                        var value = reader.Context.Record[fileColumnIndex];
                        var convertedValue = Convert.ChangeType(value, Type.GetType(configColumn.ColumnType));
                        data[index].Add(convertedValue);
                    }
                }

                return data;
            }
        }

        public async Task<DataTable> ReadSampleAsync(string filePath, int sampleSize)
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
                            data.Columns.Add(new System.Data.DataColumn($"Column_{colIndex}", typeof(string)));
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
