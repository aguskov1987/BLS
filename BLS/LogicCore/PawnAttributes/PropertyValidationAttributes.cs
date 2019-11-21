using System;

namespace BLS
{
    namespace PropertyValidation
    {
        /// <summary>
        /// Use this attribute to apply a character limit on a string.
        /// </summary>
        public class StringLengthRestriction : Attribute
        {
            public int MinCharacters { get; set; }
            public int MaxCharacters { get; set; }
        }

        /// <summary>
        /// Apply this attribute if you want to restrict a numeric property
        /// </summary>
        public class NumberRestriction : Attribute
        {
            public float Minimum { get; set; }
            public float Maximum { get; set; }
        }

        /// <summary>
        /// Apply this attribute if you want to restrict a date property
        /// </summary>
        public class DateRestriction : Attribute
        {
            public float Minimum { get; set; }
            public float Maximum { get; set; }
        }

        /// <summary>
        /// Use this attribute if you want to restrict the count inside a connection property
        /// </summary>
        public class CollectionCountRestriction : Attribute
        {
            public float MinimumCount { get; set; }
            public float MaximumCount { get; set; }
        }
    }
}