using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BLS
{
    /// <summary>
    /// Interface for a storage system. The interface abstracts the storage model
    /// to a set of containers and their relations. Containers hold objects.
    /// Relations are registered as named connections between containers.
    /// </summary>
    public interface IBlStorageProvider
    {
        IStorageProviderDetails ProviderDetails { get; }
        
        /// <summary>
        /// Synchronize containers and relations with storage
        /// </summary>
        /// <param name="containers">List of containers</param>
        /// <param name="relations">List of relations</param>
        /// <returns></returns>
        Tuple<List<string>, List<string>> Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations);

        /// <summary>
        /// Get an entity object by ID
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="id">ID of the entity object</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <returns>Entity object or null if nothing is found</returns>
        T GetById<T>(string id, string containerName = null) where T: BlsPawn;

        /// <summary>
        /// Find entity objects in a given container and a set of conditions to check
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="filter">Expression to </param>
        /// <param name="sortProperty"></param>
        /// <param name="sortOrder"></param>
        /// <returns>Cursor containing the result set</returns>
        StorageCursor<T> FindInContainer<T>(string containerName, BinaryExpression filter = null,
            string sortProperty = null, string sortOrder = null) where T : BlsPawn;

        /// <summary>
        /// Find objects, given the container name and a term to search
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="propertiesToSearch">List of properties to search in</param>
        /// <param name="term">Term to search</param>
        /// <param name="filter"></param>
        /// <param name="sortProperty"></param>
        /// <param name="sortOrder"></param>
        /// <returns>Cursor containing the result set</returns>
        /// <remarks>The containers and search-enabled properties must be first added using the <c>this.RegisterFullTextSearchMember</c> method</remarks>
        StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            BinaryExpression filter = null, string sortProperty = null, string sortOrder = null) where T : BlsPawn;

        /// <summary>
        /// Get the count of objects in a container
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <returns>Entity objects count</returns>
        int GetContainerCount(string containerName);

        /// <summary>
        /// Get all objects related to the specified source object
        /// </summary>
        /// <typeparam name="T">Type of the related entities</typeparam>
        /// <param name="fromId">ID of the source entity object</param>
        /// <param name="relationName">Name of the relation to look for</param>
        /// <param name="containerName">Name of the container of the related objects</param>
        /// <returns>Cursor containing the result set</returns>
        StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null) where T: BlsPawn;

        /// <summary>
        /// Insert a new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">Type of entity</param>
        /// <param name="containerName">Name if the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns></returns>
        string SaveNew<T>( T entity, string containerName = null, string tIdentifier=null) where T : BlsPawn;

        /// <summary>
        /// Update an object
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="id">Id of the entity object</param>
        /// <param name="newObject">The new object to update from</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="returnOld">Defaults to false but, if specified to true, return the old entity object</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>Updated or old entity object</returns>
        string Update<T>(string id, T newObject, string containerName = null, string tIdentifier = null, bool returnOld = false) where T : BlsPawn;

        /// <summary>
        /// Delete an object from storage
        /// </summary>
        /// <param name="entityId">ID of the object to remove</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>true if removal is successful</returns>
        bool Delete(string entityId, string containerName = null, string tIdentifier = null);

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
        bool SaveRelation(string fromContainer, string fromId, string relationName,
            string toContainer, string toId, string tIdentifier = null);

        /// <summary>
        /// Delete a relation between two objects
        /// </summary>
        /// <param name="fromContainer"></param>
        /// <param name="fromId"></param>
        /// <param name="relationName"></param>
        /// <param name="toContainer"></param>
        /// <param name="toId"></param>
        /// <param name="tIdentifier"></param>
        /// <returns></returns>
        bool DeleteRelation(string fromContainer, string fromId, string relationName,
            string toContainer, string toId, string tIdentifier=null);

        /// <summary>
        /// Collect a set of objects by executing a query directly against the storage
        /// </summary>
        /// <typeparam name="T">Type of the entity</typeparam>
        /// <param name="query">Query</param>
        /// <returns>Cursor containing the result set</returns>
        StorageCursor<T> ExecuteQuery<T>(string query) where T : new();

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