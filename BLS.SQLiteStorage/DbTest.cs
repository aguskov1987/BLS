using System.Data.SQLite;
using System.Linq.Expressions;

namespace BLS.SQLiteStorage
{
    public class DbTest
    {
        public void TestConnection()
        {
            var conStr = "Data Source=:memory:;Version=3;New=True;";
            var db = new SQLiteConnection(conStr);
            
            db.Open();
            var command = db.CreateCommand();
            command.CommandText = "CREATE TABLE test (id integer PRIMARY KEY, text varchar(100));";
            command.ExecuteNonQuery();
            command.CommandText = "DROP TABLE IF EXISTS test";
            command.ExecuteNonQuery();
            db.Close();
        }
    }
}