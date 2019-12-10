namespace BLS.Syncing
{
    /// <summary>
    /// Current Stratus of an item. 'Item' in this case is a container, relation or a property (of a container)
    /// </summary>
    public enum ExistenceItemSyncStatus
    {
        InModel,
        InStorage,
        InBoth
    }
}