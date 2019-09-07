using System;
using System.Threading.Tasks;
using DataVisualization.Models;

namespace DataVisualization.Services
{
    public class ScriptDataService
    {
        private readonly DataService _dataService;
        private readonly DataFileSaver _dataFileSaver;
        private readonly DataConfigurationService _dataConfigurationService;
        private readonly DataFileReader _dataFileReader;

        public ScriptDataService(DataService dataService, DataFileSaver dataFileSaver,
            DataConfigurationService dataConfigurationService, DataFileReader dataFileReader)
        {
            _dataService = dataService;
            _dataFileSaver = dataFileSaver;
            _dataConfigurationService = dataConfigurationService;
            _dataFileReader = dataFileReader;
        }

        public async Task SaveAsync(string dataName, string content)
        {
            var data = _dataService.GetData<ScriptData>(dataName);
            data.Data = content;

            _dataService.UpdateData(data);

            var config = _dataConfigurationService.GetByName<ScriptDataConfiguration>(dataName);
            await _dataFileSaver.SaveAsync(config.FilePath, content);
        }

        public async Task<ScriptData> GetDataAsync(string dataName)
        {
            var cachedData = _dataService.GetData<ScriptData>(dataName);
            var config = _dataConfigurationService.GetByName<ScriptDataConfiguration>(dataName);
            var fileData = await _dataFileReader.ReadDataAsync(config);

            if (!cachedData.Data.Equals(fileData.Data, StringComparison.InvariantCulture))
            {
                cachedData.Data = fileData.Data;
                _dataService.UpdateData(cachedData);
            }

            return cachedData;
        }
    }
}
