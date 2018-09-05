using System;

namespace DataVisualization.Models
{
    public enum ColumnTypes
    {
        Unknown,
        Number,
        DateTime
    }

    public struct ColumnTypeDef : IEquatable<ColumnTypeDef>
    {
        public ColumnTypes PrettyType { get; set; }
        public string InternalType { get; set; }

        public static ColumnTypeDef Unknown => new ColumnTypeDef(ColumnTypes.Unknown, typeof(string).FullName);
        public static ColumnTypeDef Number => new ColumnTypeDef(ColumnTypes.Number, typeof(double).FullName);
        public static ColumnTypeDef Datetime => new ColumnTypeDef(ColumnTypes.DateTime, typeof(DateTime).FullName);
        public static ColumnTypeDef[] AllTypes => new ColumnTypeDef[] { Unknown, Number, Datetime };

        private ColumnTypeDef(ColumnTypes prettyType, string internalType)
        {
            PrettyType = prettyType;
            InternalType = internalType;
        }

        public static bool operator ==(ColumnTypeDef type1, ColumnTypeDef type2)
        {
            return type1.Equals(type2);
        }

        public static bool operator !=(ColumnTypeDef type1, ColumnTypeDef type2)
        {
            return !(type1 == type2);
        }

        public override bool Equals(object obj)
        {
            return obj is ColumnTypeDef && Equals((ColumnTypeDef)obj);
        }

        public bool Equals(ColumnTypeDef other)
        {
            return PrettyType == other.PrettyType && InternalType == other.InternalType;
        }

        public override int GetHashCode()
        {
            var hashCode = 2105327189;
            hashCode = hashCode * -1521134295 + PrettyType.GetHashCode();
            hashCode = hashCode * -1521134295 + InternalType.GetHashCode();
            return hashCode;
        }
    }
}
