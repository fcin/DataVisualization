using DataVisualization.Models;
using System;

namespace DataVisualization.Services.DataPulling
{
    public sealed class DataPullerFactory
    {
        public IDataPuller Create(PullingMethods method)
        {
            switch (method)
            {
                case PullingMethods.LocalFile:
                    return new LocalFileDataPuller();
                case PullingMethods.HttpJson:
                    return new HttpDataPuller();
                default:
                    throw new ArgumentOutOfRangeException(nameof(method));
            }
        }
    }
}
