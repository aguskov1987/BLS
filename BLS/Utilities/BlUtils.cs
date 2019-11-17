﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BLS.Tests")]
namespace BLS.Utilities
{
    public static class StringExtensionMethods
    {
        public static int GetStableHashCode(this string str)
        {
            unchecked
            {
                int hash1 = 5381;
                int hash2 = hash1;

                for(int i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i+1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i+1];
                }

                return hash1 + (hash2*1566083941);
            }
        }
    }
    
    public static class BlUtils
    {
        internal static IBlStorageProvider StorageRef { get; set; }

        public static string ResolveRelationName(Type source, Type target, string multiplexer = "")
        {
            string[] names = {source.FullName, target.FullName};
            Array.Sort(names);
            var combined = names[0] + multiplexer + names[1];
            return $"BLS-{combined.GetStableHashCode():X}";
        }

        public static string ResolveContainerName(Type entityType)
        {
            var name = entityType.FullName;
            return name != null ? $"BLS-{name.GetStableHashCode():X}" : null;
        }
        
        internal static List<string> ResolvePropertyNames<T>(
            Expression<Func<T,string[]>> searchProperties) where T : BlEntity
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
    }
}