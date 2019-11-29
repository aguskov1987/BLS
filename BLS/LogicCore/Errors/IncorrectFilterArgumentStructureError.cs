using System;

namespace BLS
{
    internal class IncorrectFilterArgumentStructureError : Exception
    {
        public IncorrectFilterArgumentStructureError(string message) : base(message)
        {
        }
    }
}