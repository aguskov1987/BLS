using System;
using System.Collections.Generic;

namespace BLS
{
    public class StorageCursor<T>
    {
        internal Bls BlsRef;

        public List<T> GetNextBatch()
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}