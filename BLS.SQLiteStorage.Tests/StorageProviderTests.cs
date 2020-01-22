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

        // should sync the first brand new collection (no relations)
        // should sync the second brand new collections (no relations)
        // should sync the relation between the two
        // should sync a third brand new collection

        // should sync adding a property to the third collection
        // should sync removing a property from the third collection (with retention)
        // should sync removing a property from the third collection (without retention)
        // should sync adding a previously deleted property to the third collection (with retention)
        // should sync adding a previously deleted property to the third collection (without retention)

        // should sync removing the first collection (should also remove the relation) (with retention)
        // should sync removing the first collection (should also remove the relation) (without retention)
        // should sync removing the relation between the first two collections (with retention)
        // should sync removing the relation between the first two collections (without retention)
        // should sync adding a previously deleted first collection (with retention)
        // should sync adding a previously deleted first collection (without retention)
    }
}