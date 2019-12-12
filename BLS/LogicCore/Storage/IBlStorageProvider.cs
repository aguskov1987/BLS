using System.Collections.Generic;
using BLS.Syncing;

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
        /// <param name="generatePlanOnly">If set to true, the storage engine will not perform any changes
        /// to the storage itself but will generate a sync plan as if it did; the parameter defaults to false</param>
        /// <returns>Sync plan containing the changes made or would be made to the storage</returns>
        SyncPlan Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations, bool generatePlanOnly = false);

        /// <summary>
        /// Get a pawn object from storage by its ID
        /// </summary>
        /// <typeparam name="T">pawn type</typeparam>
        /// <param name="id">ID of the pawn object</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <returns>pawn object or null if nothing is found</returns>
        T GetById<T>(string id, string containerName = null) where T: BlsPawn;

        /// <summary>
        /// Find pawn objects in storage given a filter and sort property (both can be NULLs)
        /// </summary>
        /// <typeparam name="T">Type of the pawn</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="filter">Expression to </param>
        /// <param name="sortProperty"></param>
        /// <param name="sortOrder"></param>
        /// <param name="batchSize"></param>
        /// <returns>Cursor containing the result set</returns>
        StorageCursor<T> FindInContainer<T>(string containerName, BlBinaryExpression filter = null,
            string sortProperty = null, string sortOrder = null, int batchSize = 200) where T : BlsPawn;

        /// <summary>
        /// Find pawn objects, given the container name and a term to search
        /// </summary>
        /// <typeparam name="T">Type of the pawn</typeparam>
        /// <param name="containerName">Name of the container</param>
        /// <param name="propertiesToSearch">List of properties to search in</param>
        /// <param name="term">Term to search</param>
        /// <param name="filter"></param>
        /// <param name="sortProperty"></param>
        /// <param name="sortOrder"></param>
        /// <param name="batchSize"></param>
        /// <returns>Cursor containing the result set</returns>
        /// <remarks>The containers and search-enabled properties must be first added using the <c>this.RegisterFullTextSearchMember</c> method</remarks>
        StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            BlBinaryExpression filter = null, string sortProperty = null, string sortOrder = null, int batchSize = 200) where T : BlsPawn;

        /// <summary>
        /// Get the count of objects in a container
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="filter"></param>
        /// <returns>pawn objects count</returns>
        int GetContainerCount(string containerName, BlBinaryExpression filter = null);

        /// <summary>
        /// Get all objects related to the specified source object
        /// </summary>
        /// <typeparam name="T">Type of the related entities</typeparam>
        /// <param name="fromId">ID of the source pawn object</param>
        /// <param name="relationName">Name of the relation to look for</param>
        /// <param name="containerName">Name of the container of the related objects</param>
        /// <param name="filter">Filter t apply to the result set</param>
        /// <param name="sortDir"></param>
        /// <param name="batchSize"></param>
        /// <param name="sortProperty"></param>
        /// <returns>Cursor containing the result set</returns>
        StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null,
            BlBinaryExpression filter = null, string sortProperty = null, Sort sortDir = Sort.Asc, int batchSize = 200) where T: BlsPawn;

        /// <summary>
        /// Insert a new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pawn">Type of pawn</param>
        /// <param name="containerName">Name if the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns></returns>
        string SaveNew<T>( T pawn, string containerName = null, string tIdentifier = null) where T : BlsPawn;

        /// <summary>
        /// Update a pawn
        /// </summary>
        /// <typeparam name="T">Type of pawn</typeparam>
        /// <param name="id">Id of the pawn object</param>
        /// <param name="newObject">The new object to update from</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="returnOld">Defaults to false but, if specified to true, return the old pawn object</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>Updated or old pawn object</returns>
        string Update<T>(string id, T newObject, string containerName = null, string tIdentifier = null, bool returnOld = false) where T : BlsPawn;

        /// <summary>
        /// Delete pawn from storage.
        /// </summary>
        /// <param name="pawnId">ID of the object to remove</param>
        /// <param name="containerName">Name of the container - optional</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns>true if removal is successful</returns>
        bool Delete(string pawnId, string containerName = null, string tIdentifier = null);

        /// <summary>
        /// Insert a new relation between two pawns.
        /// </summary>
        /// <param name="fromContainer">Name of the source pawn container</param>
        /// <param name="fromId">ID of the source pawn</param>
        /// <param name="relationName">Name of the relation</param>
        /// <param name="toContainer">Name of the target pawn container</param>
        /// <param name="toId">ID of the target pawn</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns></returns>
        bool SaveRelation(string fromContainer, string fromId, string relationName,
            string toContainer, string toId, string tIdentifier = null);

        /// <summary>
        /// Delete a relation between two pawns.
        /// </summary>
        /// <param name="fromContainer">Name of the source pawn container</param>
        /// <param name="fromId">ID of the source pawn</param>
        /// <param name="relationName">Name of the relation</param>
        /// <param name="toContainer">Name of the target pawn container</param>
        /// <param name="toId">ID of the target pawn</param>
        /// <param name="tIdentifier">Transaction identifier in case the call is part of an initialized transaction</param>
        /// <returns></returns>
        bool DeleteRelation(string fromContainer, string fromId, string relationName,
            string toContainer, string toId, string tIdentifier=null);

        /// <summary>
        /// Collect a set of objects by executing a query directly against the storage.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
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