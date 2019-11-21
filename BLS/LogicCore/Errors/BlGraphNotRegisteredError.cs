using System;

namespace BLS
{
    public class BlGraphNotRegisteredError : Exception
    {
        public BlGraphNotRegisteredError(string message) : base(message)
        {
        }
    }
}