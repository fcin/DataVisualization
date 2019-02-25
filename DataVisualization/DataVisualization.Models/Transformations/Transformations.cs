namespace DataVisualization.Models.Transformations
{
    public class AddTransformation : ITransformation
    {
        public string Name => "Add";
        public double Adder { get; set; }

        public AddTransformation() => Adder = 0;
        public AddTransformation(double adder) => Adder = adder;

        public double Transform(double value)
        {
            return value + Adder;
        }
    }

    public class MultiplyTransformation : ITransformation
    {
        public string Name => "Multiply";
        public double Multiplier { get; set; }

        public MultiplyTransformation() => Multiplier = 0;
        public MultiplyTransformation(double multiplier) => Multiplier = multiplier;

        public double Transform(double value)
        {
            return value * Multiplier;
        }
    }
}
