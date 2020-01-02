using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    internal struct Connection
    {
        public BlsPawn From { get; set; }
        public BlsPawn To { get; set; }
        public string RelationName { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is Connection))
            {
                return false;
            }

            var other = (Connection)obj;

            bool reciprocal = From == other.From && To == other.To || From == other.To && To == other.From;
            return reciprocal && RelationName == other.RelationName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From.GetHashCode(), To.GetHashCode());
        }
    }
}