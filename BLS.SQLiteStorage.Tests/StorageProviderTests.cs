using System;
using System.Collections.Generic;
using Xunit;

namespace BLS.SQLiteStorage.Tests
{
    public class StorageProviderTests
    {
        [Fact]
        public void should_error_if_there_are_no_collections()
        {
            // Setup
            var provider = new SqLiteStorageProvider();
            var containers = new List<BlGraphContainer>();
            var relations = new List<BlGraphRelation>();

            // Act & Assert
            Assert.Throws<Exception>(() => { provider.Sync(containers, relations); });
        }
        
        // should sync one brand new collection (no relations)
        // should sync two brand new collections (no relations)
        // should sync two brand new collection and a relation between the two
    }
}