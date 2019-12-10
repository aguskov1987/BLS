using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    internal struct Connection
    {
        public BlsPawn From { get; set; }
        public BlsPawn To { get; set; }
        public string RelationName { get; set; }
    }
}