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

        public T GetById<T>(string id, string containerName = null) where T : new()
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> FindInContainer<T>(string containerName, Expression<Func<T, bool>> check = null) where T : new()
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            Expression<Func<T, bool>> check = null) where T : new()
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null)
        {
            throw new NotImplementedException();
        }

        public T InsertNewEntity<T>(T entity, string containerName = null) where T : new()
        {
            throw new NotImplementedException();
        }

        public T UpdateEntity<T>(string entityId, T newEntity, string containerName = null, bool returnOld = false) where T : new()
        {
            throw new NotImplementedException();
        }

        public bool RemoveEntity(string entityId, string containerName = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> ExecuteQuery<T>(string query) where T : new()
        {
            throw new NotImplementedException();
        }
    }
}