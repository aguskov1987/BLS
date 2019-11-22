using System;
using System.Diagnostics.CodeAnalysis;

namespace BLS
{
    [ExcludeFromCodeCoverage]
    internal class DuplicateSoftDeletionFlagError : Exception
    {
        public DuplicateSoftDeletionFlagError(string message) : base(message)
        {
        }
    }
}