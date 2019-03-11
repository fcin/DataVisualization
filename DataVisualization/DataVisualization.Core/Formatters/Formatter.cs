using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DataVisualization.Models;

namespace DataVisualization.Core.Formatters
{
    public class Formatter
    {
        public Func<double, string> GetFormat(ColumnTypeDef type, IEnumerable<long> values)
        {
            if (type == ColumnTypeDef.Number)
                return val => val.ToString(CultureInfo.CurrentCulture);

            if (type == ColumnTypeDef.Datetime)
            {
                var interval = TimeSpan.FromTicks(values.Max() - values.Min());

                if (interval < TimeSpan.FromDays(1))
                    return val => new DateTime((long)val).ToString("hh:mm:ss");
                if (interval >= TimeSpan.FromDays(1) && interval < TimeSpan.FromDays(365))
                    return val => new DateTime((long)val).ToShortDateString();
                return val => new DateTime((long)val).ToString("yyyy");
            }

            throw new ArgumentException("Unsupported type");
        }
    }
}
