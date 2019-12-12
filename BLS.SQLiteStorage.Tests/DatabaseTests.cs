using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using Xunit;

namespace BLS.SQLiteStorage.Tests
{
    public class DatabaseTests
    {
        [Fact]
        public void should_execute_query_with_1_to_1_property_mapping()
        {
            // Setup
            var conStr = "Data Source=:memory:;Version=3;New=True;";
            var db = new SQLiteConnection(conStr);
            
            db.Open();
            var command = db.CreateCommand();
            
            command.CommandText = @"
            CREATE TABLE cars
            (
                id INTEGER PRIMARY KEY,
                make TEXT,
                model TEXT,
                year INTEGER,
                doors INTEGER,
                color TEXT,
                engine_volume REAL
            );";
            
            command.ExecuteNonQuery();
            
            command.CommandText = @"
            INSERT INTO cars
            (make, model, year, doors, color, engine_volume)
            VALUES
            ('VW', 'Tiguan', 2014, 4, 'Silver', 2.4)";
            
            command.ExecuteNonQuery();

            string query = "SELECT * FROM cars";
            var props = new Dictionary<string, Type> {
                {"id", typeof(int)},
                {"make", typeof(string)},
                {"model", typeof(string)},
                {"year", typeof(int)},
                {"doors", typeof(int)},
                {"color", typeof(string)},
                {"engine_volume", typeof(float)}
            };

            // Act
            var database = new Database(db);
            List<Dictionary<string, object>> response = database.ExecuteSqlQuery(query, props);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("VW", response[0]["make"]);
            Assert.Equal("Tiguan", response[0]["model"]);
            Assert.Equal(2014, response[0]["year"]);
            Assert.Equal(4, response[0]["doors"]);

            if (db.State != ConnectionState.Closed)
            {
                db.Close();
            }
        }
        
        [Fact]
        public void should_execute_query_with_missing_storage_properties()
        {
            // Setup
            var conStr = "Data Source=:memory:;Version=3;New=True;";
            var db = new SQLiteConnection(conStr);
            
            db.Open();
            var command = db.CreateCommand();
            
            command.CommandText = @"
            CREATE TABLE cars
            (
                id INTEGER PRIMARY KEY,
                make TEXT,
                model TEXT,
                year INTEGER,
                doors INTEGER,
                color TEXT
            );";
            
            command.ExecuteNonQuery();
            
            command.CommandText = @"
            INSERT INTO cars
            (make, model, year, doors, color)
            VALUES
            ('VW', 'Tiguan', 2014, 4, 'Silver')";
            
            command.ExecuteNonQuery();

            string query = "SELECT * FROM cars";
            var props = new Dictionary<string, Type> {
                {"id", typeof(int)},
                {"make", typeof(string)},
                {"model", typeof(string)},
                {"year", typeof(int)},
                {"doors", typeof(int)},
                {"color", typeof(string)},
                {"engine_volume", typeof(float)}
            };

            // Act
            var database = new Database(db);
            List<Dictionary<string, object>> response = database.ExecuteSqlQuery(query, props);

            // Assert
            Assert.NotNull(response);
            Assert.Throws<KeyNotFoundException>(() =>
            {
                var test = response[0]["engine_volume"];
            });

            if (db.State != ConnectionState.Closed)
            {
                db.Close();
            }
        }
        
        [Fact]
        public void should_execute_query_with_extra_storage_properties()
        {
            // Setup
            var conStr = "Data Source=:memory:;Version=3;New=True;";
            var db = new SQLiteConnection(conStr);
            
            db.Open();
            var command = db.CreateCommand();
            
            command.CommandText = @"
            CREATE TABLE cars
            (
                id INTEGER PRIMARY KEY,
                make TEXT,
                model TEXT,
                year INTEGER,
                doors INTEGER,
                color TEXT
            );";
            
            command.ExecuteNonQuery();
            
            command.CommandText = @"
            INSERT INTO cars
            (make, model, year, doors, color)
            VALUES
            ('VW', 'Tiguan', 2014, 4, 'Silver')";
            
            command.ExecuteNonQuery();

            string query = "SELECT * FROM cars";
            var props = new Dictionary<string, Type> {
                {"id", typeof(int)},
                {"make", typeof(string)},
                {"model", typeof(string)}
            };

            // Act
            var database = new Database(db);
            List<Dictionary<string, object>> response = database.ExecuteSqlQuery(query, props);

            // Assert
            Assert.NotEmpty(response);
            Assert.Equal(3, response[0].Count);

            if (db.State != ConnectionState.Closed)
            {
                db.Close();
            }
        }
    }
}