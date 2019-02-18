namespace DataVisualization.Models.Transformations
{
    public class AddTransformation : ITransformation
    {
        private readonly double _adder;

        public AddTransformation(double adder)
        {
            _adder = adder;
        }

        public double Transform(double value)
        {
            return value + _adder;
        }
    }
}
