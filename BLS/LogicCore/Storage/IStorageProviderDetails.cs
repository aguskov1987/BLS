namespace BLS
{
    public interface IStorageProviderDetails
    {
        /// <summary>
        /// Maximum number of characters in the container name
        /// </summary>
        int MaxContainerNameLength { get; }
        
        /// <summary>
        /// Maximum number of characters in the relation name
        /// </summary>
        int MaxRelationNameLength { get; }
        
        /// <summary>
        /// The flag indicates whether the storage engine supports full text search
        /// </summary>
        bool SupportsFullTextSearch { get; }
        
        /// <summary>
        /// Flag indicates whether nested objects can be used as properties on pawns
        /// </summary>
        bool SupportsNestedObjectsAsPawnProperties { get; }
        
        /// <summary>
        /// The flag indicates whether the storage engine will perform a backup each time the
        /// engine syncs the business model with the data storage
        /// </summary>
        bool PerformBackupBeforeSync { get; set; }
        
        /// <summary>
        /// If set to true, unused relations and containers will be removed by the storage
        /// engine after each sync if the container/relation is no longer present in the model.
        /// Data stored in those containers will be lost and, if container/relation with the same name
        /// is introduced in the model again, it will be re-created as brand new container/relation.
        /// If set to false (default), the storage engine will keep containers and relations in place
        /// and, if you re-introduce them again in the model, will pick them up in storage.
        /// </summary>
        bool DeleteUnusedContainersAndRelationsOnSync { get; set; }
        
        /// <summary>
        /// If set to true, the storage engine will scan and remove any properties on pawn which
        /// are still in the storage but are no longer in the pawn model. If a property with the same name is
        /// re-introduced, it will be treated as brand new property. If set to false, the engine will keep
        /// any non-used properties in the storage; if the property is re-introduced again, it will be
        /// populated from storage like it never disappeared.
        /// </summary>
        bool DeleteUnusedPropertiesOnSync { get; set; }
    }
}