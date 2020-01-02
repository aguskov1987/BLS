using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    public class BlGraphRelation
    {
        public BlGraphContainer SourceContainer { get; set; }
        public BlGraphContainer TargetContainer { get; set; }
        public string RelationName { get; set; }
        public int MinConnections { get; set; }
        public int MaxConnections { get; set; }
    }
}