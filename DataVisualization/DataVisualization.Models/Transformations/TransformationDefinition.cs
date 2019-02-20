using System;
using System.Collections.Generic;

namespace DataVisualization.Models.Transformations
{
    public class TransformationDefinition
    {
        public string Name { get; }
        public double Value { get; set; }

        internal TransformationDefinition(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }

    public class TransformationDefinitionFactory
    {
        public static TransformationDefinition GetDefinition(string transformationName)
        {
            switch (transformationName)
            {
                case "Add":
                    return new TransformationDefinition("Add", 0);
                case "Subtract":
                    return new TransformationDefinition("Subtract", 0);
                default:
                    throw new ArgumentException(nameof(transformationName));
            }
        }

        public static IEnumerable<TransformationDefinition> GetAllDefinitions()
        {
            return new TransformationDefinition[] {
                new TransformationDefinition("Add", 0),
                new TransformationDefinition("Subtract", 0)
            };
        }
    }
}
