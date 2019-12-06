using System.Collections.Generic;

namespace BLS
{
    namespace Syncing
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

        public enum SyncAction
        {
            RemoveFromStorage,
            KeepInStorage,
            AddToStorage,
            DoNothing
        }

        public class PropertySyncPlan
        {
            /// <summary>
            /// Name of the property
            /// </summary>
            public string Name { get; set; }
            public ExistenceItemSyncStatus ExistenceItemSyncStatus { get; set; }
            public SyncAction SyncAction { get; set; }
        }

        public class ContainerSyncPlan
        {
            public ContainerSyncPlan()
            {
                Properties = new List<PropertySyncPlan>();
            }

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
            
            public List<PropertySyncPlan> Properties { get; set; }
        }

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

        /// <summary>
        /// Sync plan is the list of changes the storage engine has to do in order to
        /// synchronize the current model with whatever the engine sees in the storage.
        /// Changes may include adding/removing new containers, relations as well as container
        /// modifications (add/remove properties)
        /// </summary>
        public class SyncPlan
        {
            public SyncPlan()
            {
                Containers = new List<ContainerSyncPlan>();
                Relations = new List<RelationSyncPlan>();
            }

            public List<ContainerSyncPlan> Containers { get; set; }
            public List<RelationSyncPlan> Relations { get; set; }
        }
    }
}