using System;
using System.Collections.Generic;

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

    public class SubtractTransformation : ITransformation
    {
        public string Name => "Subtract";
        public double Subtrahend { get; set; }

        public SubtractTransformation() => Subtrahend = 0;
        public SubtractTransformation(double subtrahend) => Subtrahend = subtrahend;

        public double Transform(double value)
        {
            return value - Subtrahend;
        }
    }
}
