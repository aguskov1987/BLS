namespace BLS.Syncing
{
    public class PropertySyncPlan
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; set; }
        public ExistenceItemSyncStatus ExistenceItemSyncStatus { get; set; }
        public SyncAction SyncAction { get; set; }
    }
}