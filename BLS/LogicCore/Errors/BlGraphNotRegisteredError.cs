using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    public class BlGraphNotRegisteredError : Exception
    {
        public BlGraphNotRegisteredError(string message) : base(message)
        {
        }
    }
}