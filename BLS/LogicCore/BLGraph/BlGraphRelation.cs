namespace BLS
{
    public class BlGraphRelation
    {
        internal BlGraphContainer SourceContainer { get; set; }
        internal BlGraphContainer TargetContainer { get; set; }
        internal string RelationName { get; set; }
        internal int MinConnections { get; set; }
        internal int MaxConnections { get; set; }
    }
}