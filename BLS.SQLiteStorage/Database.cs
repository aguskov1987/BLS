using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;

namespace BLS.SQLiteStorage
{
    internal class Database
    {
        private readonly SQLiteConnection _connection;

        internal Database()
        {
            var connectionString = BuildConnectionString();
            _connection = new SQLiteConnection(connectionString);
        }

        internal Database(SQLiteConnection connection)
        {
            _connection = connection;
        }

        internal int ExecuteSqlCommand(string statements)
        {
            _connection.Open();
            
            var command = _connection.CreateCommand();
            command.CommandText = statements;
            int result = command.ExecuteNonQuery();
            
            _connection.Close();
            return result;
        }

        internal List<Dictionary<string, object>> ExecuteSqlQuery(string query, Dictionary<string, Type> columnsToLoad)
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            var command = _connection.CreateCommand();
            command.CommandText = query;

            var resultDataset = new List<Dictionary<string, object>>();
            
            var reader = command.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                NameValueCollection values = reader.GetValues();

                var relevantValues = new Dictionary<string, Tuple<string, Type>>();
                
                foreach (string column in values)
                {
                    if (columnsToLoad.ContainsKey(column))
                    {
                        Type systemType = columnsToLoad[column];
                        relevantValues.Add(column, new Tuple<string, Type>(values[column], systemType));
                    }
                }
                
                var row = new Dictionary<string, object>();

                foreach (var value in relevantValues)
                {
                    try
                    {
                        object convertedProp = Convert.ChangeType(value.Value.Item1, value.Value.Item2);
                        row.Add(value.Key, convertedProp);
                    }
                    catch (InvalidCastException exception)
                    {
                        // TODO: replace with a custom error
                        throw new InvalidCastException("invalid cast");
                    }
                }
                
                resultDataset.Add(row);
            }
            reader.Close();
            
            return resultDataset;
        }

        private string BuildConnectionString()
        {
            var builder = new SQLiteConnectionStringBuilder
            {
                DataSource = "database.sqlite", ForeignKeys = true, JournalMode = SQLiteJournalModeEnum.Memory, Version = 3
            };
            return builder.ToString();
        }
    }
}