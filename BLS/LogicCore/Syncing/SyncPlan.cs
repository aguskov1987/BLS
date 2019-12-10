using System.Collections.Generic;

namespace BLS.Syncing
{
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