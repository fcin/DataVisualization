using System.Collections.Generic;

namespace DataVisualization.Models
{
    public class DataConfiguration
    {
        public string DataName { get; set; }
        public List<DataColumn> Columns { get; set; }

        public DataConfiguration()
        {
            Columns = new List<DataColumn>();
        }
    }
}
