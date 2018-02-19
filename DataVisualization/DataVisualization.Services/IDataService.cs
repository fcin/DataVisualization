using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataVisualization.Services
{
    public interface IDataService
    {
        Task<IEnumerable<double>> GetDataAsync();
        Task<IEnumerable<string>> GetSampleDataAsync(string filePath, int sampleSize);
    }
}