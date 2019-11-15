using System;

namespace BLS
{
    public abstract class BlEntity
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastTimeModified { get; set; }

        /// <summary>
        /// Save properties of the current BL entity
        /// </summary>
        public void SaveThis()
        {
            throw new NotImplementedException();
        }
    }
}