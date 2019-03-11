using System;
using System.Globalization;
using DataVisualization.Models;

namespace DataVisualization.Core.Formatters
{
    public class TooltipTitleFormatter
    {
        public static Func<double, string> GetFormat(ColumnTypeDef columnType)
        {
            if (columnType == ColumnTypeDef.Number)
                return value => value.ToString(new NumberFormatInfo());

            if (columnType == ColumnTypeDef.Datetime)
            {
                return value => new DateTime((long)value).ToString("u");
            }

            throw new ArgumentException("Unsupported type");
        }
    }
}
