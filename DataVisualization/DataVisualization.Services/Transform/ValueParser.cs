using System;
using System.Collections.Generic;
using System.Globalization;

namespace DataVisualization.Services.Transform
{
    public class ValueParser
    {
        private readonly NumberFormatInfo _numberFormat;

        public ValueParser(string thousandsSeparator, string decimalSeparator)
        {
            if (thousandsSeparator == decimalSeparator)
                throw new ArgumentException("Thousands separator and Decimal separator cannot be the same");

            _numberFormat = new NumberFormatInfo
            {
                NumberGroupSeparator = thousandsSeparator,
                NumberDecimalSeparator = decimalSeparator
            };
        }

        public (bool IsParsed, object ParsedObject) TryParse(string value, string valueType)
        {
            switch (valueType)
            {
                case "System.Double":
                    {
                        var parsed = double.TryParse(value, NumberStyles.Any, _numberFormat, out var result);
                        return (parsed, result);
                    }
                case "System.DateTime":
                    {
                        var parsed = DateTime.TryParse(value, out var result);
                        return (parsed, result);
                    }
                case "System.String":
                    return (true, value);
                default:
                    throw new ArgumentException("Unknown type");
            }
        }

        public (bool Parsed, List<object> ParsedValues) TryParseAll(IEnumerable<string> values, string type)
        {
            var parsedValues = new List<object>();
            foreach (var value in values)
            {
                var parsedValue = TryParse(value, type);
                if (!parsedValue.IsParsed)
                    return (false, new List<object>());
                parsedValues.Add(parsedValue.ParsedObject);
            }
            return (true, parsedValues);
        }
    }
}
