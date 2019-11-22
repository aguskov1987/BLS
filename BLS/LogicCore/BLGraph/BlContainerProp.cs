using System;

namespace BLS
{
    internal class BlContainerProp
    {
        public Type PropType { get; set; }
        public string Name { get; set; }
        public bool IsSoftDeleteProp { get; set; }
        public bool IsSearchable { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public int MaxChar { get; set; }
        public int MinChar { get; set; }
        public int MinCollectionCount { get; set; }
        public int MaxCollectionCount { get; set; }
    }
}