using System;

namespace BLS
{
    internal class InvalidSortPropertyError : Exception
    {
        public InvalidSortPropertyError(string message) : base(message)
        {
        }
    }
}