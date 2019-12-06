using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BLS.SQLiteStorage.Tests")]
namespace BLS.SQLiteStorage
{
    /// <summary>
    /// This is an implementation of the <see cref="IBlStorageProvider"/> using the popular SQLite
    /// in-memory database. Tables are used as containers and foreign keys are used as relations.
    /// </summary>
    public class SqLiteStorageProvider : IBlStorageProvider
    {
        public SqLiteStorageProvider()
        {
            ProviderDetails = new SqLiteDetails();
        }

        public IStorageProviderDetails ProviderDetails { get; }

        #region Retrieval

        public T GetById<T>(string id, string containerName = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> FindInContainer<T>(string containerName, BlBinaryExpression filter = null, string sortProperty = null,
            string sortOrder = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            BlBinaryExpression filter = null, string sortProperty = null, string sortOrder = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public int GetContainerCount(string containerName)
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null,
            BlBinaryExpression filter = null, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        #endregion

        #region CRUD

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

        #endregion

        #region Transactions

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

        #endregion

        #region Sync and Query

        public Tuple<List<string>, List<string>> Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations)
        {
            // 1. get the list of tables from SQLite
            // 2. for every container:
            //     a. if the table does not exist, create the new table
            //     b. if the table exists:
            //         1. get the list of columns for the table
            //         2. for each property
            //             a. if corresponding column does not exist, add it
            //             b. if the corresponding column exists, check if the datatypes match and if they don't, try converting
            // 3. collect the unused tables
            // 4. get the list of relation tables (joins)
            // 5. for every relation:
            //     a. if the join does not exist, create new join
            //     b. if the join exists, leave it
            // 6. collect the unused joins
            // 7. if the flag is set, drop the unused relations
            // 8. if the flag is set, drop the unused containers
            throw new NotImplementedException();
        }
        
        public StorageCursor<T> ExecuteQuery<T>(string query) where T : new()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}