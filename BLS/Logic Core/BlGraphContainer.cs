namespace BLS
{
    public class BlGraphContainer
    {
        internal string BlContainerName { get; set; }
        internal string StorageContainerName { get; set; }
        internal string[] FtsEnabledFields { get; set; }
        internal string SoftDeleteProperty { get; set; }
    }
}