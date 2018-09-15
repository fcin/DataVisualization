using DataVisualization.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DataVisualization.Services
{
    public interface IDataFileReader
    {
        Task<Data> ReadDataAsync(DataConfiguration config, IProgress<LoadingBarStatus> progress);
        Task<DataTable> ReadSampleAsync(string filePath, int sampleSize);
    }
}