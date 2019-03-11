using LiteDB;
using System;
using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class DataConfiguration
    {
        [BsonId]
        public int Id { get; set; }
        public string DataName { get; set; }
        public string FilePath { get; set; }
        public List<DataColumn> Columns { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public TimeSpan RefreshRate { get; set; }
        public PullingMethodProperties PullingMethod { get; set; }
        public bool IsLiveByDefault { get; set; }

        public DataConfiguration()
        {
            Columns = new List<DataColumn>();
        }
    }
}
