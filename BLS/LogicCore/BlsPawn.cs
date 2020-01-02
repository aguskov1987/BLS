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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            if (!(obj is BlsPawn other))
            {
                return false;
            }

            if (_id == null || other._id == null)
            {
                return false;
            }

            // Let's be exact when comparing strings:
            return string.Equals(_id, other._id, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return _id == null ? 0 : _id.GetHashCode();
        }
    }
}