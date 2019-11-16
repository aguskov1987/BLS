using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS.Storage_Providers
{
    public class ArangoStorageProvider : IBlStorageProvider
    {
        public void RegisterEntityContainer(string containerNme)
        {
            throw new NotImplementedException();
        }

        public void RegisterRelation(string fromContainerName, string relation, string toContainerName)
        {
            throw new NotImplementedException();
        }

        public void RegisterSoftDeletionFlag(string containerName, string propertyName)
        {
            throw new NotImplementedException();
        }

        public void RegisterFullTextSearchMember(string containerName, string propertyName)
        {
            throw new NotImplementedException();
        }

        public Tuple<List<string>, List<string>> Synchronize(bool performBackupBeforeSync)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(string id, string containerName = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> FindInContainer<T>(string containerName, Expression<Func<T, bool>> check = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch,
            string term,
            Expression<Func<T, bool>> check = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public int GetContainerEntityCount(string containerName)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null) where T: BlEntity
        {
            throw new NotImplementedException();
        }

        public string InsertNewEntity<T>(T entity, string containerName = null, string tIdentifier=null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public string UpdateEntity<T>(string entityId, T newEntity, string containerName = null, string tIdentifier=null, bool returnOld = false) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public bool RemoveEntity(string entityId, string containerName = null, string tIdentifier=null)
        {
            throw new NotImplementedException();
        }

        public bool InsertRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId, string tIdentifier=null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId, string tIdentifier=null)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> ExecuteQuery<T>(string query) where T : new()
        {
            throw new NotImplementedException();
        }

        public void DropRelation(string relationName)
        {
            throw new NotImplementedException();
        }

        public void DropContainer(string containerName)
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