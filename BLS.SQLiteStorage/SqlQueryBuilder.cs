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

        internal string SelectPawnsFromTable(
            string tableName,
            int offset,
            int howMany,
            BlBinaryExpression filter = null,
            string sortColumn = null,
            Sort sort = Sort.Asc)
        {
            string selectClause = $" SELECT * FROM {tableName} ";

            string whereClause = string.Empty;
            if (filter != null)
            {
                BuildWhereClauseFromFilter(filter, ref whereClause);
            }

            string sortClause = sortColumn == null ? " ORDER BY Id" : $" ORDER BY {sortColumn} {sort.ToString()} ";
            string limitClause = $" LIMIT {howMany} OFFSET {offset} ";

            return selectClause + whereClause + sortClause + limitClause;
        }

        private void BuildWhereClauseFromFilter(BlBinaryExpression filter, ref string str)
        {
            if (filter == null)
            {
                return;
            }

            string left = string.Empty;
            string right = string.Empty;
            string opr = TranslateOperator(filter.Operator);

            if (filter.IsLeaf)
            {
                left = filter.PropName;
                right = filter.Value.ToString();
            }
            else
            {
                BuildWhereClauseFromFilter(filter.Left, ref left);
                BuildWhereClauseFromFilter(filter.Right, ref right);
            }

            str = $"WHERE ({left} {opr} {right})";
        }

        private string TranslateOperator(BlOperator filterOperator)
        {
            switch (filterOperator)
            {
                case BlOperator.And:
                    return "AND";
                    break;
                case BlOperator.Or:
                    return "OR";
                    break;
                case BlOperator.Eq:
                    return "= ";
                    break;
                case BlOperator.NotEq:
                    return "!=";
                    break;
                case BlOperator.Grt:
                    return ">";
                    break;
                case BlOperator.Ls:
                    return "<";
                    break;
                case BlOperator.GrtOrEq:
                    return ">=";
                    break;
                case BlOperator.LsOrEq:
                    return "<=";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterOperator), filterOperator, null);
            }
        }
    }
}