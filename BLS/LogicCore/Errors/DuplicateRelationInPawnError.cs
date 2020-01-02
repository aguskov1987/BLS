using System;

namespace BLS
{
    internal class DuplicateRelationInPawnError : Exception
    {
        public DuplicateRelationInPawnError(string message) : base(message)
        {
        }
    }
}