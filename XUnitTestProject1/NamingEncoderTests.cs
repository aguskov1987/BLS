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
            var encoded = encoder.EncodePawnContainerName(new FirstPawn());

            // Assert
            Assert.Equal("FirstPawn", encoded);
        }

        [Fact]
        public void ShouldEncodeRelationWithoutMultiplexer()
        {
            // Setup
            var encoder = new NaiveStorageNamingEncoder();

            // Act
            var encoded = encoder.EncodePawnRelationName(new FirstPawn(), new SecondPawn(), "");

            // Assert
            Assert.Equal("FirstPawnSecondPawn", encoded);
        }
        
        [Fact]
        public void ShouldEncodeRelationWithMultiplexer()
        {
            // Setup
            var encoder = new NaiveStorageNamingEncoder();

            // Act
            var encoded = encoder.EncodePawnRelationName(new FirstPawn(), new SecondPawn(), "Relates");

            // Assert
            Assert.Equal("FirstPawnRelatesSecondPawn", encoded);
        }
    }
}