using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BLS
{
    public abstract class BlsPawn
    {
        internal Bls SystemRef { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime LastTimeModified { get; set; }
    }
}