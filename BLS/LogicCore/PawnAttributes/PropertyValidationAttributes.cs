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
        /// Use this attribute if you want to restrict date range
        /// </summary>
        public class DateRestriction : Attribute
        {
            public string Earliest { get; set; }
            public string Latest { get; set; }
        }
    }
}