using System;

namespace DataVisualization.Models.Transformations
{
    public class AddTransformation : ITransformation
    {
        private readonly double _adder;

        public AddTransformation(double adder)
        {
            _adder = adder;
        }

        public string Name => "Add";

        public double Transform(double value)
        {
            return value + _adder;
        }
    }

    public class SubtractTransformation : ITransformation
    {
        private readonly double _subtrahend;

        public SubtractTransformation(double subtrahend)
        {
            _subtrahend = subtrahend;
        }

        public string Name => "Subtract";

        public double Transform(double value)
        {
            return value - _subtrahend;
        }
    }
}
