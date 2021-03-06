﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BLS.Syncing;

[assembly: InternalsVisibleTo("BLS.SQLiteStorage.Tests")]
namespace BLS.SQLiteStorage
{
    /// <summary>
    /// This is an implementation of the <see cref="IBlStorageProvider"/> using the popular SQLite
    /// in-memory database. Containers are implemented using tables. Relations are also implemented
    /// using tables with foreign keys to the container tables.
    /// </summary>
    public class SqLiteStorageProvider : IBlStorageProvider
    {
        private List<SqLiteCursor> _cursors;
        public SqLiteStorageProvider()
        {
            ProviderDetails = new SqLiteDetails();
            _cursors = new List<SqLiteCursor>();
        }

        public IStorageProviderDetails ProviderDetails { get; }

        #region Retrieval

        public T GetById<T>(string id, string containerName = null) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> FindInContainer<T>(string containerName, BlBinaryExpression filter = null, string sortProperty = null,
            string sortOrder = null, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            BlBinaryExpression filter = null, string sortProperty = null, string sortOrder = null, int batchSize = 200) where T : BlsPawn
        {
            throw new NotImplementedException();
        }

        public int GetContainerCount(string containerName, BlBinaryExpression filter = null)
        {
            throw new NotImplementedException();
        }

        public StorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null,
            BlBinaryExpression filter = null, string sortProperty = null, Sort sortDir = Sort.Asc, int batchSize = 200) where T : BlsPawn
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

        public SyncPlan Sync(List<BlGraphContainer> containers, List<BlGraphRelation> relations, bool generatePlanOnly = false)
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
            //     a. if the join table does not exist, create new join
            //     b. if the join table exists, leave it
            // 6. collect the unused join tables
            // 7. if the flag is set, drop the unused relations
            // 8. if the flag is set, drop the unused containers

            if (containers == null || containers.Count == 0)
            {
                throw new Exception("No containers are provided to sync");
            }

            return null;
        }
        
        public StorageCursor<T> ExecuteQuery<T>(string query) where T : new()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}