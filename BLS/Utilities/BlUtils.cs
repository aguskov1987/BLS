using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BLS.Tests")]
namespace BLS.Utilities
{
    internal static class BlUtils
    {
        [ExcludeFromCodeCoverage]
        public static List<string> ResolvePropertyNameArrayExpression<T>(
            Expression<Func<T,string[]>> searchProperties) where T : BlsPawn
        {
            NewArrayExpression castExpression;
            try
            {
                castExpression = (NewArrayExpression)searchProperties.Body;
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidOperationException("only array with property accessors are supported", ex);
            }
            
            var result = new List<string>();
            var items = castExpression.Expressions;

            foreach (var item in items)
            {
                if (item is MemberExpression)
                {
                    string propName = item.ToString().Split('.')[1];
                    result.Add(propName);
                }
                else
                {
                    throw new InvalidOperationException("only property access expression are supported in the array");
                }
            }

            return result;
        }
        
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