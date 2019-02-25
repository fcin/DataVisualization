using DataVisualization.Models.Transformations;
using System;
using System.Collections.Generic;

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
            else if (transformation is MultiplyTransformation multiplyTransformation)
            {
                return new MultiplyTransformationViewModel(multiplyTransformation);
            }

            throw new ArgumentException(nameof(transformation));
        }

        public static ITransformationViewModel Create(string transformationName)
        {
            ITransformation transformation = null;
            switch (transformationName)
            {
                case "Add":
                    transformation = new AddTransformation(0);
                    break;
                case "Multiply":
                    transformation = new MultiplyTransformation(0);
                    break;
                default:
                    throw new ArgumentException(nameof(transformationName));
            }

            return Create(transformation);
        }

        public static IEnumerable<ITransformationViewModel> GetAllTransformationVms()
        {
            yield return new AddTransformationViewModel(new AddTransformation(0));
            yield return new MultiplyTransformationViewModel(new MultiplyTransformation(0));
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

    public class MultiplyTransformationViewModel : ITransformationViewModel
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Aggregate { get; set; }
        public ITransformation Transformation { get; private set; }

        public MultiplyTransformationViewModel(ITransformation transformation)
        {
            if (!(transformation is MultiplyTransformation multiplyTransformation))
                throw new ArgumentException(nameof(transformation));

            Id = Guid.NewGuid();
            Name = multiplyTransformation.Name;
            Value = multiplyTransformation.Multiplier;
            Transformation = transformation;
        }

        public double ApplyTransformation(double aggregate)
        {
            Transformation = new MultiplyTransformation(Value);
            Aggregate = Transformation.Transform(aggregate);
            return Aggregate;
        }
    }
}
