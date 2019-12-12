using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BLS.Tests")]
namespace BLS.Utilities
{
    internal static class BlUtils
    {
        [ExcludeFromCodeCoverage]
        public static bool IsNumericType(Type tp)
        {
            switch (Type.GetTypeCode(tp))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
        
        public static bool IsEnumerableType(Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }
    }
}