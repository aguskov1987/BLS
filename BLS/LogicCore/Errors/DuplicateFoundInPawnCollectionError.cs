using System;

namespace BLS
{
    public class DuplicateFoundInPawnCollectionError : Exception
    {
        public DuplicateFoundInPawnCollectionError(string message) : base(message)
        {
        }
    }
}