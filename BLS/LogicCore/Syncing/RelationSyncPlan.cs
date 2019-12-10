namespace BLS.Syncing
{
    public class RelationSyncPlan
    {
        /// <summary>
        /// name of the container as known in the model
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// encoded name of the container
        /// </summary>
        public string StorageName { get; set; }
            
        public ExistenceItemSyncStatus ExistenceItemSyncStatus { get; set; }
            
        public SyncAction SyncAction { get; set; }
    }
}