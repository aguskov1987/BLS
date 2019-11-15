using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS
{
    public class BlSystem
    {
        private IBlStorageProvider _storageProvider;
        private readonly BlRelationResolver _resolver;

        public BlSystem()
        {
            _resolver = new BlRelationResolver();
        }

        public void RegisterStorageProvider(IBlStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void VerifyRelationalIntegrity()
        {
            _resolver.VerifyRelationalIntegrity();
        }

        public void SyncWithStorage()
        {
            List<string> containers = _resolver.GetResolvedContainers();
            List<Tuple<string, string, string>> relations = _resolver.GetResolvedRelations();
            foreach (string name in containers)
            {
                _storageProvider.RegisterEntityContainer(name);
            }

            foreach (var relation in relations)
            {
                _storageProvider.RegisterRelation(relation.Item1, relation.Item2, relation.Item3);
            }
        }

        /// <summary>
        /// Provide the type of every entity in the business logic system
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterEntity(BlEntity entity)
        {
            _resolver.AddEntityWithRelation(entity);
        }

        public BlStorageCursor<T> Find<T>(Expression<Func<T, bool>> check = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(string id)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> GetByQuery<T>(string query) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts a new entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T AddNew<T>(T entity) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an entity and all its directly connected entities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Remove<T>(T entity) where T : BlEntity
        {
            throw new NotImplementedException();
        }
    }
}