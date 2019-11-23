using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS.ArangoStorage
{
    public class ArangoStorageProvider : IBlStorageProvider
    {
        public IStorageProviderDetails ProviderDetails { get; }
        public Tuple<List<string>, List<string>> Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(string id, string containerName = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> FindInContainer<T>(string containerName, Expression<Func<T, bool>> check = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term, Expression<Func<T, bool>> check = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public int GetContainerCount(string containerName)
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null) where T : BlsPawn
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

        public bool InsertRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
            string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
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