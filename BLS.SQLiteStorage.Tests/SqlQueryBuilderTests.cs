using System;
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
                )";

            Assert.Equal(
                statement.Replace("\n", "").Replace("\r", "").Replace(" ", ""),
                createTableCommand.Replace("\n", "").Replace("\r", "").Replace(" ", ""));
        }
    }
}