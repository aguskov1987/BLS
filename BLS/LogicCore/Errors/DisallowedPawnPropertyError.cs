using System;

namespace BLS
{
    internal class DisallowedPawnPropertyError : Exception
    {
        public DisallowedPawnPropertyError(string message) : base(message)
        {
        }
    }
}