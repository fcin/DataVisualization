using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DataVisualization.Models;

namespace DataVisualization.Services
{
    public interface IDataService
    {
        Task<IEnumerable<List<object>>> GetDataAsync(DataConfiguration config);
        Task<DataTable> GetSampleDataAsync(string filePath, int sampleSize);
    }
}