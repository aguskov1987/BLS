using System;

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
}