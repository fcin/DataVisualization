using DataVisualization.Models.Transformations;
using System;
using System.Collections.Generic;
using System.Linq;

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
            switch (transformation)
            {
                case AddTransformation addTransformation:
                    return new AddTransformationViewModel(addTransformation);
                case MultiplyTransformation multiplyTransformation:
                    return new MultiplyTransformationViewModel(multiplyTransformation);
                case RadiansToDegreesTransformation radiansToDegreesTransformation:
                    return new RadiansToDegreesTransformationViewModel(radiansToDegreesTransformation);
                default:
                    throw new ArgumentException(nameof(transformation));
            }
        }

        public static ITransformationViewModel Create(string transformationName)
        {
            var transformation = GetAllTransformationVms()
                .Select(t => t.Transformation)
                .First(t => t.Name == transformationName);

            return Create(transformation);
        }

        public static IEnumerable<ITransformationViewModel> GetAllTransformationVms()
        {
            return new ITransformationViewModel[] 
            {
                new AddTransformationViewModel(new AddTransformation(0)),
                new MultiplyTransformationViewModel(new MultiplyTransformation(0)),
                new RadiansToDegreesTransformationViewModel(new RadiansToDegreesTransformation())
            };
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

    public class RadiansToDegreesTransformationViewModel : ITransformationViewModel
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public ITransformation Transformation { get; private set; }
        public double Aggregate { get; set; }

        public RadiansToDegreesTransformationViewModel(ITransformation transformation)
        {
            if (!(transformation is RadiansToDegreesTransformation))
                throw new ArgumentException(nameof(transformation));

            Id = Guid.NewGuid();
            Transformation = transformation;
            Name = transformation.Name;
        }

        public double ApplyTransformation(double aggregate)
        {
            Transformation = new RadiansToDegreesTransformation();
            Aggregate = Transformation.Transform(aggregate);
            return Aggregate;
        }
    }
}
