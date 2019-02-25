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
        private readonly GlobalSettings _globalSettings;

        public DataFileReader(GlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public async Task<Data> ReadDataAsync(DataConfiguration config, IProgress<LoadingBarStatus> progress = null)
        {
            var path = config.FilePath;
            var linesRead = 0;
            var fileSizeInBytes = new FileInfo(path).Length;

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
                        var (isParsed, parsedObject) = parser.TryParse(value, configColumn.ColumnType);
                        if (isParsed)
                        {
                            if (parsedObject is DateTime dtValue)
                                data[index].Add(dtValue.Ticks);
                            else
                                data[index].Add((double)parsedObject);
                        }
                        else
                        {
                            data[index].Add(double.NaN);
                        }
                    }

                    if (progress != null)
                    {
                        var readDataProgress = new LoadingBarStatus
                        {
                            Message = "Reading records from file...",
                            PercentFinished = (int)(reader.Context.CharPosition / (double)fileSizeInBytes * 100d)
                        };
                        progress.Report(readDataProgress);
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
                        Chunks = d.ToChunks(_globalSettings.PointsCount),
                        ColorHex = Color.FromArgb(255, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)).ToString(),
                        Thickness = 2,
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
    }
}
