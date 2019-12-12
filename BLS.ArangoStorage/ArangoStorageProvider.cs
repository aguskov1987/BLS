using System;
using System.Collections.Generic;
using BLS.Syncing;

namespace BLS.ArangoStorage
{
    public class ArangoStorageProvider : IBlStorageProvider
    {
        public IStorageProviderDetails ProviderDetails { get; }
        public SyncPlan Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations, bool generatePlanOnly = false)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(string id, string containerName = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> FindInContainer<T>(string containerName, BlBinaryExpression filter = null, string sortProperty = null,
            string sortOrder = null, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            BlBinaryExpression filter = null, string sortProperty = null, string sortOrder = null, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public int GetContainerCount(string containerName, BlBinaryExpression filter = null)
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null,
            BlBinaryExpression filter = null, string sortProperty = null, Sort sortDir = Sort.Asc, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public string SaveNew<T>(T entity, string containerName = null, string tIdentifier = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public string Update<T>(string id, T newObject, string containerName = null, string tIdentifier = null,
            bool returnOld = false) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public bool Delete(string entityId, string containerName = null, string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public bool SaveRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
            string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
            string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> ExecuteQuery<T>(string query) where T : new()
        {
            throw new NotImplementedException();
        }

        public string BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public bool CommitTransaction(string identifier)
        {
            throw new NotImplementedException();
        }

        public bool RevertTransaction(string identifier)
        {
            throw new NotImplementedException();
        }
    }
}