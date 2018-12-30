using DataVisualization.Models;
using System;

namespace DataVisualization.Services.DataPulling
{
    public sealed class DataPullerFactory
    {
        private readonly GlobalSettings _globalSettings;

        public DataPullerFactory(GlobalSettings globalSettings)
        {
            _globalSettings = globalSettings;
        }

        public IDataPuller Create(PullingMethods method)
        {
            switch (method)
            {
                case PullingMethods.LocalFile:
                    return new LocalFileDataPuller(_globalSettings);
                case PullingMethods.HttpJson:
                    return new HttpDataPuller(_globalSettings);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method));
            }
        }
    }
}
