using Caliburn.Micro;

namespace DataVisualization.Core.ViewModels
{
    public enum PopupBoxResult
    {
        None, Ok, Yes, No
    }

    public enum PopupBoxType
    {
        None, Ok, YesNo
    }

    public class PopupBoxViewModel : PropertyChangedBase
    {
        public string Message { get; set; }
        public bool IsOkButtonVisible { get; set; }
        public bool IsYesButtonVisible { get; set; }
        public bool IsNoButtonVisible { get; set; }
        public bool ShowWarning { get; set; }

        public PopupBoxViewModel(PopupBoxType type, string message, bool showWarning = false)
        {
            Message = message;
            ShowWarning = showWarning;

            if (type == PopupBoxType.Ok)
            {
                IsOkButtonVisible = true;
            }
            else if (type == PopupBoxType.YesNo)
            {
                IsYesButtonVisible = true;
                IsNoButtonVisible = true;
            }
        }
    }
}
