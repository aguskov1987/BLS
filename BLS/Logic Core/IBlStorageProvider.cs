using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS
{
    /// <summary>
    /// Interface for a storage system. The interface abstracts the storage model
    /// to a set entities and their relations. Entities which are basically types are
    /// stored in containers. Relations are registered as named connections between containers.
    /// </summary>
    public interface IBlStorageProvider
    {
        /// <summary>
        /// Register an entity container. Each entity container must have a unique name
        /// consisting of letters and underscores only; each entity corresponds to one container
        /// </summary>
        /// <param name="containerNme">Name of the container to register</param>
        void RegisterEntityContainer(string containerNme);

        /// <summary>
        /// Register a relation between two entities. Each relation must be unique
        /// </summary>
        /// <param name="fromContainerName">Name of the source container</param>
        /// <param name="relation">Name of the relation</param>
        /// <param name="toContainerName">Name of the target container. Can be the same as the source container</param>
        /// <remarks>You can have more than one relation between the same containers as long as the name
        /// of the relation is unique</remarks>
        void RegisterRelation(string fromContainerName, string relation, string toContainerName);

        /// <summary>
        /// Register a property of entity to use as soft deletion flag
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="propertyName">Name of the entity's property to use as soft deletion flag.
        /// The property should be boolean but can be of any acceptable in the <see cref="BlEntity"/> type</param>
        void RegisterSoftDeletionFlag(string containerName, string propertyName);

        /// <summary>
        /// Register a property so it is available to search using the <c>this.SearchInContainer</c>
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="propertyName">Name of the property of the entity for search. Must be of string type</param>
        void RegisterFullTextSearchMember(string containerName, string propertyName);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="performBackupBeforeSync"></param>
        /// <returns>Two lists: the first contains orphan containers, the second - orphaned relations</returns>
        Tuple<List<string>, List<string>> Synchronize(bool performBackupBeforeSync);

        /// <summary>
        /// Get an entity object by ID
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="id">ID of the entity object</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <returns>Entity object or null if nothing is found</returns>
        T GetById<T>(string id, string containerName = null) where T: BlEntity;

        /// <summary>
        /// Find entity objects in a given container and a set of conditions to check
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="check">Binary expression to filter the result</param>
        /// <returns>Cursor containing the result set</returns>
        BlStorageCursor<T> FindInContainer<T>(string containerName, Expression<Func<T, bool>> check = null) where T : BlEntity;

        /// <summary>
        /// Find entity objects, given the container name and a term to search
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="propertiesToSearch">List of properties to search in</param>
        /// <see cref="BlFullTextSearchable"/>
        /// <param name="term">Term to search</param>
        /// <param name="check">Any additional filter to apply to the result of the search</param>
        /// <returns>Cursor containing the result set</returns>
        /// <remarks>The containers and search-enabled properties must be first added using the <c>this.RegisterFullTextSearchMember</c> method</remarks>
        BlStorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            Expression<Func<T, bool>> check = null) where T : BlEntity;

        /// <summary>
        /// Get the count of entity objects in a container
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <returns>Entity objects count</returns>
        int GetContainerEntityCount(string containerName);

        /// <summary>
        /// Get all entity objects related to the specified source object
        /// </summary>
        /// <typeparam name="T">Type of the related entities</typeparam>
        /// <param name="fromId">ID of the source entity object</param>
        /// <param name="relationName">Name of the relation to look for</param>
        /// <param name="containerName">Name of the container of the related objects</param>
        /// <returns>Cursor containing the result set</returns>
        BlStorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null) where T: BlEntity;

        /// <summary>
        /// Insert a new entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Type of entity</param>
        /// <param name="containerName">Name if the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns></returns>
        string InsertNewEntity<T>( T entity, string containerName = null, string tIdentifier=null) where T : BlEntity;

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="entityId">Id of the entity object</param>
        /// <param name="newEntity">The new object to update from</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="returnOld">Defaults to false but, if specified to true, return the old entity object</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>Updated or old entity object</returns>
        string UpdateEntity<T>(string entityId, T newEntity, string containerName = null, string tIdentifier = null, bool returnOld = false) where T : BlEntity;

        /// <summary>
        /// Remove an entity object
        /// </summary>
        /// <param name="entityId">ID of the entity object to remove</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>true if removal is successful</returns>
        bool RemoveEntity(string entityId, string containerName = null, string tIdentifier = null);

        /// <summary>
        /// Insert a new relation
        /// </summary>
        /// <param name="fromContainer"></param>
        /// <param name="fromId"></param>
        /// <param name="relationName"></param>
        /// <param name="toContainer"></param>
        /// <param name="toId"></param>
        /// <param name="tIdentifier"></param>
        /// <returns></returns>
        bool InsertRelation(string fromContainer,
                                string fromId,
                                    string relationName,
                                string toContainer,
                            string toId, string tIdentifier = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromContainer"></param>
        /// <param name="fromId"></param>
        /// <param name="relationName"></param>
        /// <param name="toContainer"></param>
        /// <param name="toId"></param>
        /// <param name="tIdentifier"></param>
        /// <returns></returns>
        bool RemoveRelation(string fromContainer,
                                string fromId,
                                    string relationName,
                                string toContainer,
                            string toId, string tIdentifier=null);

        /// <summary>
        /// Collect a set of entity objects by executing a query
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="query">Query</param>
        /// <returns>Cursor containing the result set</returns>
        BlStorageCursor<T> ExecuteQuery<T>(string query) where T : new();

        /// <summary>
        /// Remove relation from storage
        /// </summary>
        /// <param name="relationName">Name of the relation</param>
        void DropRelation(string relationName);
        
        /// <summary>
        /// Remove entity container from storage
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <remarks>Removing an entity container fails if the container is connected to any other container. Remove relations first</remarks>
        void DropContainer(string containerName);
        
        /// <summary>
        /// Begin transaction
        /// </summary>
        /// <returns>Transaction identifier</returns>
        string BeginTransaction();
        
        /// <summary>
        /// Commit transaction
        /// </summary>
        /// <param name="identifier">Transaction identifier</param>
        /// <returns>true if committed</returns>
        bool CommitTransaction(string identifier);
        
        /// <summary>
        /// Abort transaction
        /// </summary>
        /// <param name="identifier">Transaction identifier</param>
        /// <returns>true if aborted</returns>
        bool RevertTransaction(string identifier);
    }
}