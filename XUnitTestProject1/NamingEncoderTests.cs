using Xunit;

namespace BLS.Tests
{
    public class NamingEncoderTests
    {
        class FirstPawn : BlsPawn {}
        class SecondPawn : BlsPawn {}
        
        [Fact]
        public void ShouldEncodeContainerName()
        {
            // Setup
            var encoder = new NaiveStorageNamingEncoder();

            // Act
            var encoded = encoder.EncodePawnContainerName(new FirstPawn().GetType().Name);

            // Assert
            Assert.Equal("FirstPawn", encoded);
        }

        [Fact]
        public void ShouldEncodeRelationWithoutMultiplexer()
        {
            // Setup
            var encoder = new NaiveStorageNamingEncoder();

            // Act
            var encoded = encoder.EncodePawnRelationName(new FirstPawn().GetType().Name, new SecondPawn().GetType().Name, "");

            // Assert
            Assert.Equal("FirstPawnSecondPawn", encoded);
        }
        
        [Fact]
        public void ShouldEncodeRelationWithMultiplexer()
        {
            // Setup
            var encoder = new NaiveStorageNamingEncoder();

            // Act
            var encoded = encoder.EncodePawnRelationName(new FirstPawn().GetType().Name, new SecondPawn().GetType().Name, "Relates");

            // Assert
            Assert.Equal("FirstPawnRelatesSecondPawn", encoded);
        }
    }
}