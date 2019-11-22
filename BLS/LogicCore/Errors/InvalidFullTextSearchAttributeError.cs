using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    internal class InvalidFullTextSearchAttributeError : Exception
    {
        public InvalidFullTextSearchAttributeError(string message) : base(message)
        {
        }
    }
}