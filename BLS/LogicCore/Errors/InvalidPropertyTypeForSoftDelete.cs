using System;

namespace BLS
{
    internal class InvalidPropertyTypeForSoftDelete : Exception
    {
        public InvalidPropertyTypeForSoftDelete(string message) : base(message)
        {
        }
    }
}