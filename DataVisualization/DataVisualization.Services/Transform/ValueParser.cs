using DataVisualization.Models;
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

        public (bool IsParsed, object ParsedObject) TryParse(string value, ColumnTypeDef valueType)
        {
            if (valueType == ColumnTypeDef.Number)
            {
                var parsed = double.TryParse(value, NumberStyles.Any, _numberFormat, out var result);
                return (parsed, result);
            }
            else if (valueType == ColumnTypeDef.Datetime)
            {
                var style = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | 
                    DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces;
                var parsed = DateTime.TryParse(value, CultureInfo.InvariantCulture, style, out var result);
                return (parsed, result);
            }
            else if (valueType == ColumnTypeDef.Unknown)
            {
                return (true, value);
            }
            else
            {
                throw new ArgumentException("Unknown type");
            }
        }

        public (bool Parsed, List<object> ParsedValues) TryParseAll(IEnumerable<string> values, ColumnTypeDef type)
        {
            var parsedValues = new List<object>();
            foreach (var value in values)
            {
                var (isParsed, parsedObject) = TryParse(value, type);
                if (!isParsed)
                    return (false, new List<object>());
                parsedValues.Add(parsedObject);
            }
            return (true, parsedValues);
        }
    }
}
