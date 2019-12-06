using System;

namespace BLS.SQLiteStorage
{
    public class SqLiteDetails : IStorageProviderDetails
    {
        private readonly bool _performBackupBeforeSync = false;
        public int MaxContainerNameLength => int.MaxValue;
        public int MaxRelationNameLength => int.MaxValue;
        public bool SupportsFullTextSearch => false;
        public bool SupportsNestedObjectsAsPawnProperties => false;

        public bool PerformBackupBeforeSync
        {
            get => _performBackupBeforeSync;
            set => throw new NotImplementedException("This database is running in memory so there is no backup procedure");
        }

        public bool DeleteUnusedContainersAndRelationsOnSync { get; set; }
        public bool DeleteUnusedPropertiesOnSync { get; set; }
    }
}