using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracking;

namespace BLS
{
    /// <summary>
    /// Storage Cursor is the class you use to access pawns in storage as well as in BLS memory (pawns which need to be added for example)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StorageCursor<T>
    {
        private string _cursorId;
        private bool _hasMoreInStorage;
        private int _batchSize;
        private IBlStorageProvider _storageProvider;
        
        internal  List<T> BlsInMemoryCursorBuffer;
        internal List<T> StorageObjectBuffer;

        public StorageCursor(IBlStorageProvider storageProvider, List<T> itemsFromStorage, string cursorId, bool hasMore, int batchSize)
        {
            _storageProvider = storageProvider;
            StorageObjectBuffer = itemsFromStorage;
            _cursorId = cursorId;
            _hasMoreInStorage = hasMore;
            _batchSize = batchSize;
        }

        public List<T> GetNextStorageBatch()
        {
            return StorageObjectBuffer;
        }
        
        public List<T> GetInMemoryPawns()
        {
            return BlsInMemoryCursorBuffer;
        }

        public List<T> GetAll()
        {
            return BlsInMemoryCursorBuffer.Concat(StorageObjectBuffer).ToList();
        }

        internal StorageCursor<T> AttachInMemPawns(List<T> pawns)
        {
            BlsInMemoryCursorBuffer = pawns;
            return this;
        }

        /// <summary>
        /// internal constructor for testing
        /// </summary>
        internal StorageCursor()
        {
            BlsInMemoryCursorBuffer = new List<T>();
            StorageObjectBuffer = new List<T>();
            _hasMoreInStorage = false;
            _batchSize = 200;
        }
    }
}