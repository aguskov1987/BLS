using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    public class DuplicateFoundInPawnCollectionError : Exception
    {
        public DuplicateFoundInPawnCollectionError(string message) : base(message)
        {
        }
    }
}