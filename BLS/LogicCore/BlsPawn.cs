using System;
using ChangeTracking;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace BLS
{
    /// <summary>
    /// This is the base class for all BLS pawns you wish to include in your system.
    /// </summary>
    public abstract class BlsPawn
    {
        private string _id;

        [DoNoTrack]
        internal virtual Bls SystemRef { get; set; }

        public virtual DateTime Created { get; set; }
        public virtual DateTime LastTimeModified { get; set; }

        internal void SetId(string id)
        {
            _id = id;
        }

        internal string GetId()
        {
            return _id;
        }
    }
}