using Xunit;

namespace BLS.SQLiteStorage.Tests
{
    public class SqLiteConnectionTests
    {
        [Fact]
        public void sanity_check()
        {
            var dbTest = new DbTest();
            dbTest.TestConnection();
        }
    }
}