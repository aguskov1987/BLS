using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace BLS.Tests
{
    public class BlsTests
    {
        [Fact]
        public void should_fail_to_persist_if_no_storage_provider_is_set()
        {
            // Setup
            var bls = new Bls(null, null);

            // Act & Assert
            Assert.Throws<NoStorageProviderRegisteredError>(() => {bls.PersistChanges();});
        }
        
        [Fact]
        public void should_be_able_to_find_all_pawns()
        {
            // Setup
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn",
                Properties = new List<BlContainerProp>()
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var cursor = new StorageCursor<BasicPawn>();
            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock
                .Setup(provider =>
                    provider.FindInContainer<BasicPawn>(It.IsAny<string>(), null, null, It.IsAny<string>(), 200))
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>();

            // Assert
            Assert.NotNull(resultCursor);
            storageProviderMock.Verify(p => p.FindInContainer<BasicPawn>("BasicPawn", null, null, "Asc", 200));
        }

        [Fact]
        public void should_be_able_to_find_pawns_based_on_multiple_filters()
        {
            // Setup
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn",
                Properties = new List<BlContainerProp>()
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var cursor = new StorageCursor<BasicPawn>();
            var storageProviderMock = new Mock<IBlStorageProvider>();

            BlBinaryExpression filterExpression = null;
            storageProviderMock
                .Setup(provider =>
                    provider.FindInContainer<BasicPawn>(It.IsAny<string>(), It.IsAny<BlBinaryExpression>(), null,
                        "Asc", 200))
                .Callback<string, BlBinaryExpression, string, string, int>((container, exp, sort, order, batchSize) =>
                    filterExpression = exp)
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>(filter: (bp) => bp.Date > DateTime.Today && bp.Name == "some name" || bp.Date < DateTime.Today);

            // Assert
            Assert.NotNull(resultCursor);
            Assert.NotNull(filterExpression);
        }

        [Fact]
        public void should_be_able_to_find_pawns_based_on_single_evaluated_filter()
        {
            // Setup
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn",
                Properties = new List<BlContainerProp>()
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var cursor = new StorageCursor<BasicPawn>();
            var storageProviderMock = new Mock<IBlStorageProvider>();

            BlBinaryExpression filterExpression = null;
            storageProviderMock
                .Setup(provider =>
                    provider.FindInContainer<BasicPawn>(It.IsAny<string>(), It.IsAny<BlBinaryExpression>(), null,
                        "Asc", 200))
                .Callback<string, BlBinaryExpression, string, string,int >((container, exp, sort, order, batchSize) =>
                    filterExpression = exp)
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>(filter: (bp) => bp.Date > DateTime.Today);

            // Assert
            Assert.NotNull(resultCursor);
            Assert.NotNull(filterExpression);
            
            Assert.Equal("Date", filterExpression.PropName);
            Assert.Equal(BlOperator.Grt, filterExpression.Operator);
            Assert.Equal(DateTime.Today.ToString("O"),( (DateTime)filterExpression.Value).ToString("O"));
            Assert.Null(filterExpression.Left);
            Assert.Null(filterExpression.Right);
            Assert.True(filterExpression.IsLeaf);
        }

        [Fact]
        public void should_be_able_to_find_pawns_based_on_single_literal_filter()
        {
            // Setup
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn",
                Properties = new List<BlContainerProp>()
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var cursor = new StorageCursor<BasicPawn>();
            var storageProviderMock = new Mock<IBlStorageProvider>();

            BlBinaryExpression filterExpression = null;
            storageProviderMock
                .Setup(provider =>
                    provider.FindInContainer<BasicPawn>(It.IsAny<string>(), It.IsAny<BlBinaryExpression>(), null,
                        "Asc", 200))
                .Callback<string, BlBinaryExpression, string, string, int>((container, exp, sort, order, batchSize) =>
                    filterExpression = exp)
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>(filter: (bp) => bp.Name == "some name");

            // Assert
            Assert.NotNull(resultCursor);
            Assert.NotNull(filterExpression);
            
            Assert.Equal("Name", filterExpression.PropName);
            Assert.Equal(BlOperator.Eq, filterExpression.Operator);
            Assert.Equal("some name", filterExpression.Value);
            Assert.Null(filterExpression.Left);
            Assert.Null(filterExpression.Right);
            Assert.True(filterExpression.IsLeaf);
        }

        [Fact]
        public void should_be_able_to_get_pawn_by_id()
        {
            // Setup
            var basicPawn = new BasicPawn {Name = "Name"};
            basicPawn.SetId("123_id");
            
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn",
                Properties = new List<BlContainerProp>()
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock.Setup(provider
                => provider.GetById<BasicPawn>(It.IsAny<string>(), It.IsAny<string>())).Returns(basicPawn);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var found = bls.GetById<BasicPawn>("123");

            // Assert
            Assert.NotNull(found);
            storageProviderMock.Verify(provider => provider.GetById<BasicPawn>("123", "BasicPawn"), Times.Once());
        }

        [Fact]
        public void should_fail_on_find_if_single_filter_property_is_incorrectly_formatted()
        {
            // There is some strange lambda syntax here which people would probably never use but it's
            // still a good idea to guard against these types of error
            
            // Setup
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn", StorageContainerName = "BasicPawn"
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.GetStorageContainerNameForPawn(It.IsAny<BasicPawn>())).Returns("BasicPawn");
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var cursor = new StorageCursor<BasicPawn>();
            var storageProviderMock = new Mock<IBlStorageProvider>();

            storageProviderMock
                .Setup(provider =>
                    provider.FindInContainer<BasicPawn>(It.IsAny<string>(), It.IsAny<BlBinaryExpression>(), null,
                        "Asc", 200))
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act & Assert
            Assert.Throws<IncorrectFilterArgumentStructureError>(() =>
            {
                // ReSharper disable once EqualExpressionComparison
                bls.Find<BasicPawn>(filter: (pawn => "" == ""));
            });
            Assert.Throws<IncorrectFilterArgumentStructureError>(() =>
            {
                bls.Find<BasicPawn>(filter: (pawn => "" == pawn.Name));
            });
            Assert.Throws<IncorrectFilterArgumentStructureError>(() =>
            {
                bls.Find<BasicPawn>(filter: (pawn => string.Empty == ""));
            });
            Assert.Throws<IncorrectFilterArgumentStructureError>(() =>
            {
                bls.Find<BasicPawn>(filter: (pawn => true));
            });
        }

        [Fact]
        public void should_fail_spawning_new_pawn_if_no_pawns_are_registered()
        {
            // Setup
            Bls bls = new Bls(null);

            // Act & Assert
            Assert.Throws<PawnNotRegisteredError>(() =>
            {
                bls.SpawnNew<BasicPawn>();
            });
        }

        [Fact]
        public void should_not_be_able_to_get_pawn_by_id_if_its_not_registered()
        {
            // Setup
            var basicPawn = new BasicPawn {Name = "Name"};

            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn", StorageContainerName = "BasicPawn"
            };
            var containerList = new List<BlGraphContainer> {basicPawnContainer};
            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock
                .Setup(provider => provider.GetById<BasicPawn>(It.IsAny<string>(), null))
                .Returns(basicPawn);

            var bls = new Bls(storageProviderMock.Object, graphMock.Object);

            // Act & Assert
            Assert.Throws<PawnNotRegisteredError>(() =>
            {
                bls.GetById<BasicPawn>("123");
            });
        }

        [Fact]
        public void should_spawn_new_pawn()
        {
            // Setup
            var graphContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn",
                StorageContainerName = "BasicPawn"
            };
            var containerList = new List<BlGraphContainer> {graphContainer};

            var graphMock = new Mock<IBlGraph>();
            graphMock.Setup(graph => graph.CompiledCollections).Returns(containerList);

            // Act
            Bls bls = new Bls(null, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            BasicPawn basicPawn = bls.SpawnNew<BasicPawn>();

            // Assert
            Assert.NotNull(basicPawn);
        }
    }
}