using DataVisualization.Models;

namespace DataVisualization.Core.Events
{
    public class BeforeDataConfigurationDeletedEventArgs
    {
        public DataConfiguration ConfigToDelete { get; set; }
    }
}
