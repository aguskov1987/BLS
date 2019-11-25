using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace BLS.Tests
{
    public class BlsTests
    {
        [Fact]
        public void ShouldBeAbleToFindAllPawns()
        {
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
                .Setup(provider => provider.FindInContainer<BasicPawn>(It.IsAny<string>(), null, null, null))
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>();

            // Assert
            Assert.NotNull(resultCursor);
            storageProviderMock.Verify(p => p.FindInContainer<BasicPawn>("BasicPawn", null, null, null));
        }

        [Fact]
        public void ShouldBeAbleToFindPawnsBasedOnFilter()
        {
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
                .Setup(provider => provider.FindInContainer<BasicPawn>(It.IsAny<string>(), null, null, null))
                .Returns(cursor);
            var bls = new Bls(storageProviderMock.Object, graphMock.Object);
            bls.RegisterBlPawns(new BasicPawn());

            // Act
            var resultCursor = bls.Find<BasicPawn>(filter: (bp) => bp.Name == "some name" && bp.Created  < DateTime.Today);

            // Assert
            Assert.NotNull(resultCursor);
        }

        [Fact]
        public void ShouldBeAbleToGetPawnById()
        {
            // Setup
            var basicPawn = new BasicPawn {Name = "Name"};
            var basicPawnContainer = new BlGraphContainer
            {
                BlContainerName = "BasicPawn", StorageContainerName = "BasicPawn"
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
        public void ShouldFailSpawningNewPawnIfNoPawnsAreRegistered()
        {
            // Setup
            Bls bls = new Bls(null);

            // Act & Assert
            Assert.Throws<PawnNotRegisteredError>(() =>
            {
                BasicPawn basicPawn = bls.SpawnNew<BasicPawn>();
            });
        }

        [Fact]
        public void ShouldNotBeAbleToGetPawnByIdIfItsNotRegistered()
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
                var found = bls.GetById<BasicPawn>("123");
            });
        }

        [Fact]
        public void ShouldSpawnNewPawn()
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