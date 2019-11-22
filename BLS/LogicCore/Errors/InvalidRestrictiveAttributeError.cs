using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    internal class InvalidRestrictiveAttributeError : Exception
    {
        public InvalidRestrictiveAttributeError(string message) : base(message)
        {
        }
    }
}