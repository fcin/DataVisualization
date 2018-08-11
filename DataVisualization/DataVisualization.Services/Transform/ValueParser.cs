using System;
using System.Globalization;

namespace DataVisualization.Services.Transform
{

    public class ValueParser
    {
        private readonly NumberFormatInfo _numberFormat;

        public ValueParser(string thousandsSeparator, string decimalSeparator)
        {
            if(thousandsSeparator == decimalSeparator)
                throw new ArgumentException("Thousands separator and Decimal separator cannot be the same");

            _numberFormat = new NumberFormatInfo
            {
                NumberGroupSeparator = thousandsSeparator,
                NumberDecimalSeparator = decimalSeparator
            };
        }

        public object Parse(string value, string valueType)
        {
            switch (valueType)
            {
                case "System.Double":
                    return double.Parse(value, _numberFormat);
                case "System.DateTime":
                    return DateTime.Parse(value);
                default:
                    throw new ArgumentException("Uknown type");
            }
        }
    }
}
