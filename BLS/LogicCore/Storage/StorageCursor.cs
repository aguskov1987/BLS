using System.Collections.Generic;
using System.Linq;

namespace BLS
{
    /// <summary>
    /// Storage cursor is the main gate though which you retrieve pawns. Any collection
    /// coming from the storage would be wrapped in a cursor. The results can be retrieved
    /// using the <see cref="GetNextStorageBatch"/> method in pre-defined batches.
    /// </summary>
    /// <typeparam name="T">Type of the business object - does not have to be a BlsPawn</typeparam>
    public class StorageCursor<T>
    {
        private string _cursorId;
        private bool _hasMoreInStorage;
        private int _batchSize;
        private Bls _bls;
        
        internal  List<T> BlsInMemoryCursorBuffer;
        internal readonly List<T> StorageObjectBuffer;

        public StorageCursor(List<T> itemsFromStorage, string cursorId, bool hasMore, int batchSize)
        {
            StorageObjectBuffer = itemsFromStorage;
            _cursorId = cursorId;
            _hasMoreInStorage = hasMore;
            _batchSize = batchSize;
        }

        public List<T> GetNextStorageBatch()
        {
            if (StorageObjectBuffer == null || StorageObjectBuffer.Count == 0)
            {
                return new List<T>();
            }

            if (StorageObjectBuffer[0] is BlsPawn)
            {
                var result = new List<BlsPawn>();
                foreach (T item in StorageObjectBuffer)
                {
                    _bls.ToUpdate.Add(item as BlsPawn);
                }
                StorageObjectBuffer.Clear();
                
                // todo: load next batch into StorageObjectBuffer

                return result as List<T>;
            }
            else
            {
                var result = new List<T>();
                result.AddRange(StorageObjectBuffer);
                StorageObjectBuffer.Clear();
                
                // todo: load next batch into StorageObjectBuffer
                
                return result;
            }
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

        internal StorageCursor<T> InjectBls(Bls bls)
        {
            _bls = bls;
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