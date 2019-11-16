using BLS.Utilities;
using Xunit;

namespace BLS.Tests
{
    public class BlUtilTests
    {
        class ParentEntity : BlEntity
        {
            public ParentEntity()
            {
                Children = new RelatesToMany<ChildEntity>(this);
            }

            public RelatesToMany<ChildEntity> Children { get; }
        }

        class ChildEntity : BlEntity
        {
            public ChildEntity()
            {
                Parent = new RelatesToOne<ParentEntity>(this);
            }

            public RelatesToOne<ParentEntity> Parent { get; }
        }
        
        [Fact]
        public void ShouldResolveContainerName()
        {
            var name = BlUtils.ResolveContainerName(typeof(ParentEntity));
            
            Assert.Equal("BLS-5B96865B", name);
        }

        [Fact]
        public void ShouldResolveRelationNameForward()
        {
            var name = BlUtils.ResolveRelationName(typeof(ParentEntity), typeof(ChildEntity));
            
            Assert.Equal("BLS-14377D3C", name);
        }
        
        [Fact]
        public void ShouldResolveRelationNameBackward()
        {
            var name = BlUtils.ResolveRelationName(typeof(ChildEntity), typeof(ParentEntity));
            
            Assert.Equal("BLS-14377D3C", name);
        }
        
        [Fact]
        public void ShouldResolveRelationNameForwardWithMultiplexer()
        {
            var name = BlUtils.ResolveRelationName(typeof(ParentEntity), typeof(ChildEntity), "qwe");
            
            Assert.Equal("BLS-33C45081", name);
        }
        
        [Fact]
        public void ShouldResolveRelationNameBackwardWithMultiplexer()
        {
            var name = BlUtils.ResolveRelationName(typeof(ChildEntity), typeof(ParentEntity), "qwe");
            
            Assert.Equal("BLS-33C45081", name);
        }
    }
}