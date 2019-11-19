using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BLS.Utilities;

namespace BLS
{
    public abstract class BlEntity
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastTimeModified { get; set; }
        
        internal event EventHandler<PersistEventArgs> OnPersist;
        
        private void PersistHandler(string transactionId)
        {
            EventHandler<PersistEventArgs> handler = OnPersist;
            handler?.Invoke(this, new PersistEventArgs(transactionId));
        }
        
        /// <summary>
        /// Save properties of the current BL entity
        /// </summary>
        public void Persist()
        {
            if (BlUtils.SystemRef == null)
            {
                throw new NullReferenceException("Figure is not bootstrapped to a BLS system");
            }

            if (BlUtils.SystemRef.StorageProvider == null)
            {
                throw new NullReferenceException("Storage provider is not registered in the BLS");
            }
            
            var transactionId = BeginTransaction();
            try
            {
                var store = BlUtils.SystemRef.StorageProvider;

                var id = Id == null ? store.InsertNewEntity(this, BlUtils.SystemRef.ResolveFigureContainerName(GetType()), transactionId)
                    : store.UpdateEntity(Id, this, BlUtils.SystemRef.ResolveFigureContainerName(GetType()), transactionId);

                if (!string.IsNullOrEmpty(id))
                {
                    PersistHandler(transactionId);
                }
            }
            catch (Exception e)
            {
                RevertTransaction(transactionId);
                throw;
            }
        }

        internal bool PersistWithNoPropagation(string transactionId)
        {
            var store = BlUtils.SystemRef.StorageProvider;
            var id = Id == null ? store.InsertNewEntity(this, BlUtils.SystemRef.ResolveFigureContainerName(GetType()), transactionId)
                : store.UpdateEntity(Id, this, BlUtils.SystemRef.ResolveFigureContainerName(GetType()), transactionId);

            return !string.IsNullOrEmpty(id);
        }
        
        private void PersistRelations(string transactionId)
        {
            try
            {
                var thisType = GetType();
                IEnumerable<PropertyInfo> relations = thisType.GetProperties()
                    .Where(p => p.PropertyType.BaseType.Name.Contains("BlConnected")).ToList();

                foreach (PropertyInfo propertyInfo in relations)
                {
                    Type relationType = propertyInfo.PropertyType;
                    
                    var theRelation = GetType().GetProperty(propertyInfo.Name)?
                        .GetValue(this, null);
                    
                    var result = relationType.BaseType.InvokeMember(
                        "PersistRelation",
                        BindingFlags.InvokeMethod,
                        null,
                        theRelation,
                        new object[] {transactionId});
                }

                CommitTransaction(transactionId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                RevertTransaction(transactionId);
                throw;
            }
        }

        #region Transactions

        private string BeginTransaction()
        {
            return BlUtils.SystemRef.StorageProvider.BeginTransaction();
        }

        private bool CommitTransaction(string transactionId)
        {
            return BlUtils.SystemRef.StorageProvider.CommitTransaction(transactionId);
        }

        private void RevertTransaction(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return;
            }
            BlUtils.SystemRef.StorageProvider.RevertTransaction(transactionId);
        }

        #endregion
    }
}