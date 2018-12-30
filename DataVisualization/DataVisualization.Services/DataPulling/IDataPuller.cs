using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataVisualization.Services.DataPulling
{
    public interface IDataPuller : IDisposable
    {
        Task<(List<Series> latest, int readLines)> PullAsync(DataConfiguration config, int startFromLine);
    }
}
