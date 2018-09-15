namespace DataVisualization.Core
{
    public interface ILoadingBarWindow
    {
        int PercentFinished { get; set; }
        string Message { get; set; }
    }
}
