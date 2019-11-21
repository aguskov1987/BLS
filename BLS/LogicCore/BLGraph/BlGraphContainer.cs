using System.Collections.Generic;

namespace BLS
{
    internal class BlGraphContainer
    {
        internal string BlContainerName { get; set; }
        internal string StorageContainerName { get; set; }
        public List<BlContainerProp> Properties { get; set; }
    }
}