using DataVisualization.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataVisualization.Core
{
    public class Formatter
    {
        public Func<double, string> GetFormat(ColumnTypeDef type, IEnumerable<long> values)
        {
            if (type == ColumnTypeDef.Number)
                return val => val.ToString(CultureInfo.CurrentCulture);
            else if (type == ColumnTypeDef.Datetime)
            {
                var interval = TimeSpan.FromTicks(values.Max() - values.Min());

                if (interval < TimeSpan.FromDays(1))
                    return val => new DateTime((long)val).ToString("hh:mm:ss");
                else if (interval >= TimeSpan.FromDays(1) && interval < TimeSpan.FromDays(365))
                    return val => new DateTime((long)val).ToShortDateString();
                else
                    return val => new DateTime((long)val).ToString("yyyy");
            }
            else
                throw new ArgumentException("Unsupported type");
        }
    }
}
