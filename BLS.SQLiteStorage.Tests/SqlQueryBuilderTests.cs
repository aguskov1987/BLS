using System.Collections.Generic;
using Xunit;

namespace BLS.SQLiteStorage.Tests
{
    public class SqlQueryBuilderTests
    {
        [Fact]
        public void should_create_query_for_new_table()
        {
            // Setup
            var container = new BlGraphContainer
            {
                BlContainerName = "TestPawn",
                StorageContainerName = "test_pawn",
                Properties = new List<BlContainerProp>
                {
                    new BlContainerProp {Name = "StrProp", PropType = typeof(string)},
                    new BlContainerProp {Name = "IntProp", PropType = typeof(int)},
                    new BlContainerProp {Name = "UIntProp", PropType = typeof(uint)},
                    new BlContainerProp {Name = "ShortProp", PropType = typeof(short)},
                    new BlContainerProp {Name = "LongProp", PropType = typeof(long)},
                    new BlContainerProp {Name = "FloatProp", PropType = typeof(float)},
                    new BlContainerProp {Name = "DecimalProp", PropType = typeof(decimal)},
                    new BlContainerProp {Name = "DoubleProp", PropType = typeof(double)},
                    new BlContainerProp {Name = "ByteProp", PropType = typeof(byte)},
                    new BlContainerProp {Name = "SByteProp", PropType = typeof(sbyte)},
                    new BlContainerProp {Name = "BoolProp", PropType = typeof(bool)}
                }
            };

            var builder = new SqlQueryBuilder();

            // Act
            string createTableCommand = builder.CreatePawnTable(container);

            // Assert
            var statement = @"CREATE TABLE test_pawn (
                Id INTEGER PRIMARY KEY,
                StrProp TEXT,
                IntProp INTEGER,
                UIntProp INTEGER,
                ShortProp INTEGER,
                LongProp INTEGER,
                FloatProp REAL,
                DecimalProp REAL,
                DoubleProp REAL,
                ByteProp INTEGER,
                SByteProp INTEGER,
                BoolProp INTEGER
                );";

            Assert.Equal(
                statement.Replace("\n", "").Replace("\r", "").Replace(" ", ""),
                createTableCommand.Replace("\n", "").Replace("\r", "").Replace(" ", ""));
        }

        [Fact]
        public void should_create_select_query_no_filter_no_sort()
        {
            // Setup
            var sqlBuilder = new SqlQueryBuilder();

            // Act
            var query = sqlBuilder.SelectPawnsFromTable("cars", 0, 200);

            // Assert
            Assert.Equal("SELECT * FROM cars  ORDER BY Id LIMIT 200 OFFSET 0", query);
        }

        [Fact]
        public void should_create_select_query_with_simple_filter_no_sort()
        {
            // Setup
            var filter = new BlBinaryExpression
            {
                Operator = BlOperator.Eq, IsLeaf = true, PropName = "doors", Value = 4
            };
            var sqlBuilder = new SqlQueryBuilder();

            // Act
            var query = sqlBuilder.SelectPawnsFromTable("cars", 0, 200, filter);

            // Assert
            Assert.Equal("SELECT * FROM cars WHERE (doors = 4) ORDER BY Id LIMIT 200 OFFSET 0", query);
        }
        
        [Fact]
        public void should_create_select_query_with_complex_filter_no_sort()
        {
            // Setup
            var filter1 = new BlBinaryExpression
            {
                Operator = BlOperator.Eq, IsLeaf = true, PropName = "doors", Value = 4
            };
            var filter2 = new BlBinaryExpression
            {
                Operator = BlOperator.Eq, IsLeaf = true, PropName = "doors", Value = 2
            };
            var filter = new BlBinaryExpression
            {
                IsLeaf = false,
                Left = filter1,
                Right = filter2,
                Operator = BlOperator.Or
            };
            var sqlBuilder = new SqlQueryBuilder();

            // Act
            var query = sqlBuilder.SelectPawnsFromTable("cars", 0, 200, filter);

            // Assert
            Assert.Equal("SELECT * FROM cars WHERE ((doors = 4) OR (doors = 2)) ORDER BY Id LIMIT 200 OFFSET 0", query);
        }

        [Fact]
        public void should_create_select_query_with_complex_filter_and_sort()
        {
            // Setup
            var filter1 = new BlBinaryExpression
            {
                Operator = BlOperator.Eq, IsLeaf = true, PropName = "doors", Value = 4
            };
            var filter2 = new BlBinaryExpression
            {
                Operator = BlOperator.Eq, IsLeaf = true, PropName = "doors", Value = 2
            };
            var filter = new BlBinaryExpression
            {
                IsLeaf = false,
                Left = filter1,
                Right = filter2,
                Operator = BlOperator.And
            };
            var sqlBuilder = new SqlQueryBuilder();

            // Act
            var query = sqlBuilder.SelectPawnsFromTable("cars", 200, 200, filter, "model");

            // Assert
            Assert.Equal("SELECT * FROM cars WHERE ((doors = 4) AND (doors = 2)) ORDER BY model Asc LIMIT 200 OFFSET 200", query);
        }
    }
}