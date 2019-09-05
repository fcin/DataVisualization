using System.Threading.Tasks;
using Caliburn.Micro;

namespace DataVisualization.Core
{
    public abstract class LoaderViewModelBase : PropertyChangedBase
    {
        public abstract Task InitializeAsync(string filePath);
    }
}
