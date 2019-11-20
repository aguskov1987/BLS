using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BLS.Tests")]
namespace BLS.Utilities
{
    public static class BlUtils
    {
        internal static List<string> ResolvePropertyNameArrayExpression<T>(
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

        internal static void ResolveBinaryFilterExpression<T>(Expression<Func<T, bool>> filter)
        {
            
        }
    }
}