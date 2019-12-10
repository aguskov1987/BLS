using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracking;

namespace BLS
{
    /// <summary>
    /// Storage Cursor is the class you use to access pawns in storage as well as in BLS memory (pawns which need to be added for example)
    /// </summary>
    /// <typeparam name="T">Type of the business object</typeparam>
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

        /// <summary>
        /// Call this method to return all results of the cursor. The method will return
        /// the storage data as well as any in-memory data. Please exercise caution when using this routine -
        /// it will retrieve every object from the storage (which can be a lot) and place it into memory. It is
        /// advised that this method is only used on small to medium datasets to prevent memory overload
        /// </summary>
        /// <returns>List of business objects of type <typeparam name="T"></typeparam></returns>
        public List<T> GetAll()
        {
            // TODO: fully load the storage before returning the result
            return BlsInMemoryCursorBuffer.Concat(StorageObjectBuffer).ToList();
        }

        internal StorageCursor<T> AttachInMemoryPawns(List<T> pawns)
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