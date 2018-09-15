namespace DataVisualization.Core
{
    public interface IProgressBarWindow
    {
        int PercentFinished { get; set; }
        string Message { get; set; }
    }
}
