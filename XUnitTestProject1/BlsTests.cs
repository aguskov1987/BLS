using System.Collections.Generic;
using Moq;
using Xunit;

namespace BLS.Tests
{
    public class BlsTests
    {
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
            bls.RegisterBlPawns(new BlsPawn[] {new BasicPawn(), });
            
            BasicPawn basicPawn = bls.SpawnNew<BasicPawn>();
            
            // Assert
            Assert.NotNull(basicPawn);
        }
    }
}