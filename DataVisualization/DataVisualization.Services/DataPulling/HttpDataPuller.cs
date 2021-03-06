﻿using DataVisualization.Models;
using DataVisualization.Services.Exceptions;
using DataVisualization.Services.Extensions;
using DataVisualization.Services.Transform;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataVisualization.Services.DataPulling
{
    public class HttpDataPuller : IDataPuller
    {
        private HttpClient _httpClient;

        public HttpDataPuller()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(List<Series> latest, int readLines)> PullAsync(DataConfiguration config, int startFromLine)
        {

            try
            {
                string newestData;

                try
                {
                    newestData = await _httpClient.GetStringAsync(config.PullingMethod.EndpointUrl);
                }
                catch (InvalidOperationException ex)
                {
                    throw new DataPullingException("Invalid remote address", ex);
                }

                DataPacket[] packets;

                try
                {
                    packets = JsonConvert.DeserializeObject<DataPacket[]>(newestData, new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
                }
                catch (JsonException ex)
                {
                    throw new DataParsingException("Invalid format of JSON", ex);
                }

                var rows = packets.OrderBy(p => p.Added).SelectMany(p => p.Rows).ToList();

                var parser = new ValueParser(config.ThousandsSeparator, config.DecimalSeparator);
                var data = new List<List<double>>(config.Columns.Count);
                for (var index = config.Columns.Count - 1; index >= 0; index--)
                {
                    data.Add(new List<double>());
                }

                foreach (var row in rows)
                {
                    var values = row.Values.ToList();

                    if (values.Count < config.Columns.Count)
                    {
                        // log to in-app console.
                        continue;
                    }

                    for (var index = 0; index < config.Columns.Count; index++)
                    {
                        var configColumn = config.Columns[index];
                        var value = values[configColumn.Index];
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
                }

                return (data.Select((d, index) => new Series
                {
                    Chunks = d.ToChunks(GlobalSettings.PointsCount),
                    Name = config.Columns[index].Name
                }).ToList(), data.Count);
            }
            catch (HttpRequestException ex)
            {
                throw new DataPullingException("Http Request failed", ex);
            }
            catch (JsonSerializationException ex)
            {
                throw new DataParsingException("JSON Parsing failed", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
            }
        }

        private class DataPacket
        {
            public DateTime Added { get; set; }
            public string DataName { get; set; }
            public List<DataRow> Rows { get; set; }
        }

        private class DataRow
        {
            public List<string> Values { get; set; }
        }
    }
}
