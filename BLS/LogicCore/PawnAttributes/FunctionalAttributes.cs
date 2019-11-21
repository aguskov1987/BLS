using System;

namespace BLS
{
    namespace Functional
    {
        /// <summary>
        /// Use this attribute to enable full test search on the specified property.
        /// </summary>
        public class FullTextSearchable : Attribute
        {
        }

        /// <summary>
        /// Apply this attribute if you want to use a certain property as soft deletion flag. Only one
        /// per pawn is allowed
        /// </summary>
        public class UsedForSoftDeletes : Attribute
        {
        }
    }
}