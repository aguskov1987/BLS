using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BLS.Utilities;

namespace BLS
{
    public class BlSystem
    {
        private IBlStorageProvider _storageProvider;
        private readonly IBlRelationResolver _resolver;

        internal IBlStorageProvider StorageProvider => _storageProvider;

        public bool Verified { get; internal set; }
        public bool Synced { get; internal set; }
        
        public BlSystem(IBlStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            _resolver = new BlRelationResolver();
            BlUtils.SystemRef = this;
        }

        ~BlSystem()
        {
            BlUtils.SystemRef = null;
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
        /// Resolve name of the container for the given figure
        /// </summary>
        /// <param name="figureType">Type of the figure</param>
        /// <returns>Name of the container</returns>
        internal string ResolveFigureContainerName(Type figureType)
        {
            return figureType.Name;
        }

        /// <summary>
        /// Resolve the name of the relation between given figures
        /// </summary>
        /// <param name="sourceFigureType">Type of the source figure</param>
        /// <param name="targetFigureType">Type of the target figure</param>
        /// <param name="multiplexer"></param>
        /// <returns>Name of the relation</returns>
        internal string ResolveFigureRelationName(Type sourceFigureType, Type targetFigureType,
            string multiplexer = "")
        {
            return $"{sourceFigureType.Name}-to-{multiplexer}-{targetFigureType.Name}";
        }

        /// <summary>
        /// Verify relations can be resolved between the registered entities. 
        /// </summary>
        public void VerifyRelationalIntegrity()
        {
            _resolver.VerifyRelationalIntegrity();
            Verified = true;
        }

        /// <summary>
        /// Synchronize storage with the currently registered entities
        /// </summary>
        public void SyncWithStorage()
        {
            if (!Verified)
            {
                VerifyRelationalIntegrity();
            }

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

            Synced = true;
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
            if (!Synced || !Verified)
            {
                throw new InvalidOperationException("System is not synchronized with storage");
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="searchProperties"></param>
        /// <param name="filter"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BlStorageCursor<T> Search<T>(
            string searchTerm,
            Expression<Func<T, string[]>> searchProperties,
            Expression<Func<T, bool>> filter = null) where T : BlEntity
        {
            if (!Synced || !Verified)
            {
                throw new InvalidOperationException("System is not synchronized with storage");
            }
            
            var resolvedContainer = ResolveFigureContainerName(typeof(T));
            List<string> props = ResolveSearchProperties(searchProperties);
            return _storageProvider.SearchInContainer(resolvedContainer, props, searchTerm, filter);
        }

        /// <summary>
        /// Get an entity object by its ID
        /// </summary>
        /// <param name="id">ID of the entity to get</param>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>Entity object or null if nothing is found</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T GetById<T>(string id) where T: BlEntity
        {            
            if (!Synced || !Verified)
            {
                throw new InvalidOperationException("System is not synchronized with storage");
            }
            
            return _storageProvider.GetById<T>(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public BlStorageCursor<T> GetByQuery<T>(string query) where T: new()
        {
            if (!Synced || !Verified)
            {
                throw new InvalidOperationException("System is not synchronized with storage");
            }
            
            return _storageProvider.ExecuteQuery<T>(query);
        }

        /// <summary>
        /// Delete entity and all its relations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void DeleteEntity<T>(T entity) where T : BlEntity
        {
            if (!Synced || !Verified)
            {
                throw new InvalidOperationException("System is not synchronized with storage");
            }
            
            if (entity.Id != null)
            {
                _storageProvider.RemoveEntity(entity.Id);
            }
        }
        
        private List<string> ResolveSearchProperties<T>(Expression<Func<T, string[]>> searchProperties) where T : BlEntity
        {
            return BlUtils.ResolvePropertyNameArrayExpression(searchProperties);
        }
    }
}