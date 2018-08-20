using System;
using CsvHelper;
using DataVisualization.Models;
using DataVisualization.Services.Transform;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataVisualization.Services
{
    public class DataFileReader : IDataFileReader
    {
        public int ColumnsCount { get; set; }

        public async Task<Data> ReadDataAsync(DataConfiguration config)
        {
            var path = config.FilePath;

            using (TextReader file = File.OpenText(path))
            using (var reader = new CsvReader(file))
            {
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                ColumnsCount = reader.Context.Record.Length;
                var data = new List<List<double>>(config.Columns.Count);
                for (var index = config.Columns.Count - 1; index >= 0; index--)
                {
                    data.Add(new List<double>());
                }

                var parser = new ValueParser(config.ThousandsSeparator, config.DecimalSeparator);

                while (await reader.ReadAsync())
                {
                    for (var index = 0; index < config.Columns.Count; index++)
                    {
                        var configColumn = config.Columns[index];
                        var fileColumnIndex = configColumn.Index;
                        var value = reader.Context.Record[fileColumnIndex];
                        var convertedValue = parser.Parse(value, configColumn.ColumnType);
                        if(convertedValue is DateTime dtValue)
                            data[index].Add(dtValue.Ticks);
                        else
                            data[index].Add((double)convertedValue);
                    }
                }

                var rand = new Random();

                return new Data
                {
                    Name = config.DataName,
                    Series = data.Select((d, index) => new Series
                    {
                        Values = d.ToList(),
                        SeriesColor = Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)),
                        Id = Guid.NewGuid(),
                        IsHorizontalAxis = index == 0,
                        InternalType = config.Columns[index].ColumnType
                    }).ToList()
                };
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
