using System.Collections.Generic;

namespace BLS
{
    internal class BlGraphContainer
    {
        public string BlContainerName { get; set; }
        public string StorageContainerName { get; set; }
        public List<BlContainerProp> Properties { get; set; }
    }
}