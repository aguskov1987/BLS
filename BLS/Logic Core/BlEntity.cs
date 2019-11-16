using System;
using BLS.Utilities;

namespace BLS
{
    internal class PersistEventArgs : EventArgs
    {
        public PersistEventArgs(string transactionId)
        {
            TransactionId = transactionId;
        }

        public string TransactionId { get; }
    }

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
            string transactionId = BeginTransaction();
            try
            {
                var store = BlUtils.StorageRef;
                var id = Id == null ? store.InsertNewEntity(this, BlUtils.ResolveContainerName(GetType()), transactionId)
                    : store.UpdateEntity(Id, this, BlUtils.ResolveContainerName(GetType()), transactionId);

                if (!string.IsNullOrEmpty(id))
                {
                    PersistHandler(transactionId);
                }
            }
            catch (Exception e)
            {
                RevertTransaction(transactionId);
                Console.WriteLine(e);
                throw;
            }
        }

        internal bool PersistWithNoPropagation(string transactionId)
        {
            var store = BlUtils.StorageRef;
            var id = Id == null ? store.InsertNewEntity(this, BlUtils.ResolveContainerName(GetType()), transactionId)
                : store.UpdateEntity(Id, this, BlUtils.ResolveContainerName(GetType()), transactionId);

            if (!string.IsNullOrEmpty(id))
            {
                return true;
            }

            return false;
        }

        #region Transactions

        private string BeginTransaction()
        {
            return BlUtils.StorageRef.BeginTransaction();
        }

        private void RevertTransaction(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                return;
            }
            BlUtils.StorageRef.RevertTransaction(transactionId);
        }

        #endregion
    }
}