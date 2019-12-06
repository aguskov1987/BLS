using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BLS.SQLiteStorage
{
    internal class SqlQueryBuilder
    {
        internal string CreatePawnTable(BlGraphContainer container)
        {
            var builder = new StringBuilder($"CREATE TABLE {container.StorageContainerName}\n");
            builder.Append("(Id INTEGER PRIMARY KEY, \n");
            foreach (BlContainerProp prop in container.Properties)
            {
                Type type = prop.PropType;
                if (type == typeof(string))
                {
                    builder.Append($"{prop.Name} TEXT, \n");
                }
                else if (type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(long))
                {
                    builder.Append($"{prop.Name} INTEGER, \n");
                }
                else if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(bool))
                {
                    builder.Append($"{prop.Name} INTEGER, \n");
                }
                else if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
                {
                    builder.Append($"{prop.Name} REAL, \n");
                }
            }

            builder.Append(");");
            builder.Replace(", \n)", ")");
            
            return builder.ToString();
        }

        internal string CreateRelationalTable(BlGraphRelation relation)
        {
            var statement = $"CREATE TABLE {relation.RelationName} \n";
            statement += "(RelationId INTEGER PRIMARY KEY,\n";
            statement += $"{relation.SourceContainer.StorageContainerName} INTEGER,\n";
            statement += $"{relation.TargetContainer.StorageContainerName} INTEGER,\n";
            statement += $"FOREIGN KEY({relation.SourceContainer.StorageContainerName}) REFERENCES {relation.SourceContainer.StorageContainerName}(Id),\n";
            statement += $"FOREIGN KEY({relation.TargetContainer.StorageContainerName}) REFERENCES {relation.TargetContainer.StorageContainerName}(Id)\n";
            statement += ");";
            
            return statement;
        }
        
        [ExcludeFromCodeCoverage]
        internal string DropTable(BlGraphContainer container)
        {
            return $"DROP TABLE {container.StorageContainerName};";
        }

        [ExcludeFromCodeCoverage]
        internal string GetExistingTables()
        {
            return " SELECT name FROM sqlite_master WHERE type ='table' AND name NOT LIKE 'sqlite_%';";
        }

        [ExcludeFromCodeCoverage]
        internal string GetTableColumns(string table)
        {
            return $"PRAGMA table_info({table})";
        }
    }
}