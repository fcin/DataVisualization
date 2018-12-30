﻿using CsvHelper;
using DataVisualization.Models;
using DataVisualization.Services.Extensions;
using DataVisualization.Services.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataVisualization.Services.DataPulling
{
    public class LocalFileDataPuller : IDataPuller
    {
        private readonly GlobalSettings _globalSettings;

        public LocalFileDataPuller(GlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public async Task<(List<Series> latest, int readLines)> PullAsync(DataConfiguration config, int startFromLine)
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
                    Chunks = d.ToChunks(_globalSettings.PointsCount),
                    Name = config.Columns[index].Name
                }).ToList(), countLines);
            }
        }

        public void Dispose()
        {
            
        }
    }
}