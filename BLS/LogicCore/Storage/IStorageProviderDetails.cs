namespace BLS
{
    public interface IStorageProviderDetails
    {
        int MaxContainerNameLength { get; }
        int MaxRelationNameLength { get; }
        bool SupportsFullTextSearch { get; }
        
        bool PerformBackupBeforeSync { get; set; }
        bool DeleteUnusedContainersAndRelations { get; set; }
    }
}