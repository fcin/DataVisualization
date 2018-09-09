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
using DataVisualization.Services.Extensions;

namespace DataVisualization.Services
{
    public class DataFileReader : IDataFileReader
    {
        public async Task<Data> ReadDataAsync(DataConfiguration config)
        {
            var path = config.FilePath;
            var linesRead = 0;

            using (var file = File.OpenText(path))
            using (var reader = new CsvReader(file))
            {
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                var data = new List<List<double>>(config.Columns.Count);
                for (var index = config.Columns.Count - 1; index >= 0; index--)
                {
                    data.Add(new List<double>());
                }

                var parser = new ValueParser(config.ThousandsSeparator, config.DecimalSeparator);
                
                do
                {
                    linesRead++;
                    for (var index = 0; index < config.Columns.Count; index++)
                    {
                        var configColumn = config.Columns[index];
                        var fileColumnIndex = configColumn.Index;
                        var value = reader.Context.Record[fileColumnIndex];
                        var convertedValue = parser.TryParse(value, configColumn.ColumnType);
                        if (convertedValue.IsParsed)
                        {
                            if (convertedValue.ParsedObject is DateTime dtValue)
                                data[index].Add(dtValue.Ticks);
                            else
                                data[index].Add((double)convertedValue.ParsedObject);
                        }
                        else
                        {
                            data[index].Add(double.NaN);
                        }
                    }
                }
                while (await reader.ReadAsync());

                var rand = new Random();
                
                return new Data
                {
                    Name = config.DataName,
                    FileLinesRead = linesRead,
                    Series = data.Select((d, index) => new Series
                    {
                        Chunks = d.ToChunks(),
                        SeriesColor = Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)),
                        InternalType = config.Columns[index].ColumnType,
                        Name = config.Columns[index].Name,
                        Axis = config.Columns[index].Axis
                    }).ToList()
                };
            }
        }

        public async Task<DataTable> ReadSampleAsync(string filePath, int sampleSize)
        {
            var data = new DataTable();

            using (var file = File.OpenText(filePath))
            {
                var reader = new CsvReader(file);
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                var index = 0;
                do
                {
                    var canReadNext = await reader.ReadAsync();
                    if (!canReadNext)
                        break;

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

        public async Task<(List<Series> latest, int readLines)> ReadLatestAsync(DataConfiguration config, int startFromLine)
        {
            var path = config.FilePath;
            var countLines = 0;

            using (var file = File.OpenText(path))
            using (var reader = new CsvReader(file))
            {
                reader.Configuration.Delimiter = ";";
                reader.Configuration.HasHeaderRecord = false;

                await reader.ReadAsync();

                var columnsCount = reader.Context.Record.Length;
                var data = new List<List<double>>(config.Columns.Count);
                for (var index = config.Columns.Count - 1; index >= 0; index--)
                {
                    data.Add(new List<double>());
                }

                var parser = new ValueParser(config.ThousandsSeparator, config.DecimalSeparator);

                // Skip what we already have.
                for (var index = 0; index < startFromLine - 1; index++)
                {
                    var hasNextLine = await reader.ReadAsync();
                    if (!hasNextLine)
                        return (new List<Series>(), -(startFromLine - index));
                }

                while (await reader.ReadAsync())
                {
                    countLines++;
                    for (var index = 0; index < config.Columns.Count; index++)
                    {
                        var configColumn = config.Columns[index];
                        var fileColumnIndex = configColumn.Index;
                        var value = reader.Context.Record[fileColumnIndex];
                        var convertedValue = parser.TryParse(value, configColumn.ColumnType);
                        if (convertedValue.IsParsed)
                        {
                            if (convertedValue.ParsedObject is DateTime dtValue)
                                data[index].Add(dtValue.Ticks);
                            else
                                data[index].Add((double)convertedValue.ParsedObject);
                        }
                        else
                        {
                            data[index].Add(double.NaN);
                        }
                    }
                }

                return (data.Select((d, index) => new Series
                {
                    Chunks = d.ToChunks(),
                    Name = config.Columns[index].Name
                }).ToList(), countLines);
            }
        }
    }
}
