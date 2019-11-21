using System;

namespace BLS
{
    namespace DataSync
    {
        /// <summary>
        /// Apply this attribute if you have renamed:
        /// <list type="bullet">
        ///     <item>Pawn Type</item>
        ///     <item>Pawn property</item>
        /// </list>
        /// </summary>
        /// <example>
        ///     <code>
        ///         [RenamedFrom(OldName = "CustomerOrder")]
        ///         public class ClientOrder : BlsPawn
        ///         {
        ///             [RenamedFrom(OldName = "CustomerName")]
        ///             public virtual string ClientName
        ///         }
        ///     </code>
        /// </example>
        public class RenamedFrom : Attribute
        {
            public string OldName { get; set; }
        }
    }
}