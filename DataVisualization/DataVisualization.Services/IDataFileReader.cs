using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataVisualization.Models;

namespace DataVisualization.Services
{
    public interface IDataFileReader
    {
        Task<IEnumerable<List<object>>> ReadDataAsync(DataConfiguration config);
        Task<DataTable> ReadSampleAsync(string filePath, int sampleSize);
    }
}