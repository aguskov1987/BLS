using Xunit;

namespace BLS.Tests
{
    public class ResolverTests
    {
        [Fact]
        public void ShouldCreateAResolver()
        {
            var resolver = new BlRelationResolver();

            Assert.NotNull(resolver);
        }
    }
}