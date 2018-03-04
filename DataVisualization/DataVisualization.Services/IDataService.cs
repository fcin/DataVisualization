using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DataVisualization.Services
{
    public interface IDataService
    {
        Task<IEnumerable<double>> GetDataAsync();
        Task<DataTable> GetSampleDataAsync(string filePath, int sampleSize);
    }
}