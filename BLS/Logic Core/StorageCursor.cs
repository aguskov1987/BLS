using System;
using System.Collections.Generic;

namespace BLS
{
    public class StorageCursor<T>
    {
        private string _cursorId;
        private bool _hasMore;
        public bool HasMore => _hasMore;
        private bool _error;
        public bool HasError => _error;

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