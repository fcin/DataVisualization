using DataVisualization.Models.Transformations;
using System;

namespace DataVisualization.Core.ViewModels
{
    public interface ITransformationViewModel
    {
        Guid Id { get; }
        string Name { get; set; }
        ITransformation Transformation { get; }
        double Aggregate { get; set; }
        double ApplyTransformation(double aggregate);
    }

    public static class TransformationViewModelFactory
    {
        public static ITransformationViewModel Create(ITransformation transformation)
        {
            if (transformation is AddTransformation addTransformation)
            {
                return new AddTransformationViewModel(addTransformation);
            }
            else if(transformation is SubtractTransformation subtractTransformation)
            {
                return new SubtractTransformationViewModel(subtractTransformation);
            }

            throw new ArgumentException(nameof(transformation));
        }
    }

    public class AddTransformationViewModel : ITransformationViewModel
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Aggregate { get; set; }
        public ITransformation Transformation { get; private set; }

        public AddTransformationViewModel(ITransformation transformation)
        {
            if (!(transformation is AddTransformation addTransformation))
                throw new ArgumentException(nameof(transformation));

            Id = Guid.NewGuid();
            Name = addTransformation.Name;
            Value = addTransformation.Adder;
            Transformation = transformation;
        }

        public double ApplyTransformation(double aggregate)
        {
            Transformation = new AddTransformation(Value);
            Aggregate = Transformation.Transform(aggregate);
            return Aggregate;
        }
    }

    public class SubtractTransformationViewModel : ITransformationViewModel
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Aggregate { get; set; }
        public ITransformation Transformation { get; private set; }

        public SubtractTransformationViewModel(ITransformation transformation)
        {
            if (!(transformation is SubtractTransformation subtractTransformation))
                throw new ArgumentException(nameof(transformation));

            Id = Guid.NewGuid();
            Name = subtractTransformation.Name;
            Value = subtractTransformation.Subtrahend;
            Transformation = transformation;
        }

        public double ApplyTransformation(double aggregate)
        {
            Transformation = new SubtractTransformation(Value);
            Aggregate = Transformation.Transform(aggregate);
            return Aggregate;
        }
    }
}
