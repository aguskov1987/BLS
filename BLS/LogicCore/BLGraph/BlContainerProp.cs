using System;

namespace BLS
{
    public class BlContainerProp
    {
        public Type PropType { get; set; }
        public string Name { get; set; }
        public bool IsSoftDeleteProp { get; set; }
        public bool IsSearchable { get; set; }
        public float MaxValue { get; set; }
        public float MinValue { get; set; }
        public int MaxChar { get; set; }
        public int MinChar { get; set; }
        public DateTime EarliestDate { get; set; }
        public DateTime LatestDate { get; set; }
    }
}