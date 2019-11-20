using System;

namespace BLS
{
    public class PawnNotRegisteredError : Exception
    {
        public PawnNotRegisteredError(string message) : base(message)
        {
        }
    }
}