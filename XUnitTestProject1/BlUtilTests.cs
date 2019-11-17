using System;
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
            public ChildEntity(string stringProp2, string stringProp1)
            {
                StringProp2 = stringProp2;
                StringProp1 = stringProp1;
            }
            
            public int IntProp { get; set; }
            public string StringProp1 { get; }
            public string StringProp2 { get; }
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

        [Fact]
        public void ShouldResolvePropertyNames()
        {
            var props = BlUtils.ResolvePropertyNames<ChildEntity>(c => new []{c.StringProp1, c.StringProp2});
            
            Assert.NotEmpty(props);
            Assert.Equal("StringProp1", props[0]);
            Assert.Equal("StringProp2", props[1]);
        }

        [Fact]
        public void ShouldFailTryingToResolvePropertyNamesWithLiterals()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var props = BlUtils.ResolvePropertyNames<ChildEntity>(c => new[] {"1", "2", "3"});
            });
        }
        
        [Fact]
        public void Should_FAIL_TryingToResolvePropertyNamesWithIncorrectInit()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var props = BlUtils.ResolvePropertyNames<ChildEntity>(c => Array.Empty<string>());
            });
        }
    }
}