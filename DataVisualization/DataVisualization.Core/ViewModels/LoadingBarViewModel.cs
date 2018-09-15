using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public class LoadingBarViewModel : Screen, ILoadingBarWindow
    {
        private int _percentFinished;
        public int PercentFinished
        {
            get => _percentFinished;
            set => Set(ref _percentFinished, value);
        }
        private string _message;
        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public LoadingBarViewModel()
        {
            Message = string.Empty;
            PercentFinished = 0;
        }

        public void Close()
        {
            TryClose();
        }
    }
}
