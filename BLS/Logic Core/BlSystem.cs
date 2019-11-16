using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BLS.Utilities;

namespace BLS
{
    public class BlSystem
    {
        private readonly BlRelationResolver _resolver;
        private bool _verified;
        
        public BlSystem()
        {
            _resolver = new BlRelationResolver();
        }

        public void RegisterStorageProvider(IBlStorageProvider storageProvider)
        {
            BlUtils.StorageRef = storageProvider;
        }

        /// <summary>
        /// Add a business entity to the system
        /// </summary>
        /// <param name="entity"></param>
        public void RegisterEntity(BlEntity entity)
        {
            _resolver.AddEntityWithRelation(entity);
        }
        
        /// <summary>
        /// Verify relations can be resolved between the registered entities. 
        /// </summary>
        public void VerifyRelationalIntegrity()
        {
            _resolver.VerifyRelationalIntegrity();
            _verified = true;
        }

        /// <summary>
        /// Synchronize storage with the currently registered entities
        /// </summary>
        public void SyncWithStorage()
        {
            if (!_verified)
            {
                VerifyRelationalIntegrity();
            }

            List<string> containers = _resolver.GetResolvedContainers();
            List<Tuple<string, string, string>> relations = _resolver.GetResolvedRelations();
            foreach (string name in containers)
            {
                BlUtils.StorageRef.RegisterEntityContainer(name);
            }

            foreach (var relation in relations)
            {
                BlUtils.StorageRef.RegisterRelation(relation.Item1, relation.Item2, relation.Item3);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BlStorageCursor<T> Find<T>(Expression<Func<T, bool>> filter = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="searchProperty"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BlStorageCursor<T> Search<T>(
            string searchTerm,
            Expression<Func<T, string>> searchProperty,
            Expression<Func<T, bool>> filter = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get an entity object by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Entity object or null if nothing is found</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T GetById<T>(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BlStorageCursor<T> GetByQuery<T>(string query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete entity and all its relations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void DeleteEntity<T>(T entity) where T : BlEntity
        {
            throw new NotImplementedException();
        }
        
    }
}