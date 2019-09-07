using DataVisualization.Models;

namespace DataVisualization.Core.Events
{
    public class DataConfigurationOpenedEventArgs
    {
        public object Publisher { get; set; }
        public DataConfiguration Opened { get; set; }

        public DataConfigurationOpenedEventArgs()
        {
            
        }

        public DataConfigurationOpenedEventArgs(object publisher, DataConfiguration opened)
        {
            Publisher = publisher;
            Opened = opened;
        }
    }
}
