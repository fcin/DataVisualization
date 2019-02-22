namespace DataVisualization.Models.Transformations
{
    public interface ITransformation
    {
        string Name { get; }
        double Transform(double value);
    }
}
