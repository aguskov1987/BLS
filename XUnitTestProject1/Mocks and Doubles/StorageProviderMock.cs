using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace BLS.Tests.Mocks_and_Doubles
{
    public enum TransactionStatus
    {
        Open,
        Committed,
        Reverted
    }
    internal class StorageProviderDetails : IStorageProviderDetails
    {
        public int MaxContainerNameLength => 100;
        public int MaxRelationNameLength => 100;
    }

    public class StorageProviderMock : IBlStorageProvider
    {
        public List<Tuple<string, TransactionStatus>> Transactions { get; }
        public List<BlEntity> InsertedFigures { get; }
        public List<BlEntity> RemovedFigires { get; }
        public List<string> InsertedRelations { get; }
        public List<string> RemovedRelations { get; }

        private List<string> _containers;
        private List<Tuple<string, string, string>> _relations;
        private List<Tuple<string, string>> _fullTextSearchProps;
        private List<Tuple<string, string>> _softDeleteProps;

        public StorageProviderMock()
        {
            Transactions = new List<Tuple<string, TransactionStatus>>();
            
            _containers = new List<string>();
            _relations = new List<Tuple<string, string, string>>();
            _fullTextSearchProps = new List<Tuple<string, string>>();
            _softDeleteProps = new List<Tuple<string, string>>();
            
            InsertedFigures = new List<BlEntity>();
            InsertedRelations = new List<string>();
            RemovedFigires = new List<BlEntity>();
            RemovedRelations = new List<string>();
        }

        public IStorageProviderDetails ProviderDetails => new StorageProviderDetails();

        public void RegisterEntityContainer(string containerNme)
        {
            var encoded = EncodeNameForStorage(containerNme);
            if (_containers.Any(c => c == encoded))
            {
                throw new DuplicateNameException(containerNme);
            }
            _containers.Add(encoded);
        }

        public void RegisterRelation(string fromContainerName, string relation, string toContainerName)
        {
            var encodedSource = EncodeNameForStorage(fromContainerName);
            var encodedRelation = EncodeNameForStorage(relation);
            var encodedTarget = EncodeNameForStorage(toContainerName);
            
            if (_relations.Any(r => r.Item2 == encodedRelation))
            {
                throw new DuplicateNameException(relation);
            }
            _relations.Add(new Tuple<string, string, string>(encodedSource, encodedRelation, encodedTarget));
        }

        public void RegisterSoftDeletionFlag(string containerName, string propertyName)
        {
            _softDeleteProps.Add(new Tuple<string, string>(containerName, propertyName));
        }

        public void RegisterFullTextSearchMember(string containerName, string propertyName)
        {
            _fullTextSearchProps.Add(new Tuple<string, string>(containerName, propertyName));
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

        public BlStorageCursor<T> SearchInContainer<T>(string containerName, List<string> propertiesToSearch, string term,
            Expression<Func<T, bool>> check = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public int GetContainerEntityCount(string containerName)
        {
            throw new NotImplementedException();
        }

        public BlStorageCursor<T> GetByRelation<T>(string fromId, string relationName, string containerName = null) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public string InsertNewEntity<T>(T entity, string containerName = null, string tIdentifier = null) where T : BlEntity
        {
            var gen = new Random();
            return gen.Next().ToString();
        }

        public string UpdateEntity<T>(string entityId, T newEntity, string containerName = null, string tIdentifier = null,
            bool returnOld = false) where T : BlEntity
        {
            throw new NotImplementedException();
        }

        public bool RemoveEntity(string entityId, string containerName = null, string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public bool InsertRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
            string tIdentifier = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRelation(string fromContainer, string fromId, string relationName, string toContainer, string toId,
            string tIdentifier = null)
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
            var gen = new Random();
            var transactionId = gen.Next().ToString();
            Transactions.Add(new Tuple<string, TransactionStatus>(transactionId, TransactionStatus.Open));
            return transactionId;
        }

        public bool CommitTransaction(string identifier)
        {
            var trans = Transactions.FirstOrDefault(t => t.Item1 == identifier);
            if (trans != null)
            {
                Transactions.Remove(trans);
                Transactions.Add(new Tuple<string, TransactionStatus>(trans.Item1, TransactionStatus.Committed));
                return true;
            }

            return false;
        }

        public bool RevertTransaction(string identifier)
        {
            var trans = Transactions.FirstOrDefault(t => t.Item1 == identifier);
            if (trans != null)
            {
                trans = new Tuple<string, TransactionStatus>(trans.Item1, TransactionStatus.Reverted);
                return true;
            }

            return false;
        }

        public string EncodeNameForStorage(string name)
        {
            return name;
        }
    }
}