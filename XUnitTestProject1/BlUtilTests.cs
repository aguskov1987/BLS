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
                Parent = new RelatesToOne<ParentEntity>(this);
            }

            public RelatesToMany<ChildEntity> Children { get; }
            public RelatesToOne<ParentEntity> Parent { get; }
        }

        class ChildEntity : BlEntity
        {
            public ChildEntity(string stringProp2, string stringProp1)
            {
                StringProp2 = stringProp2;
                StringProp1 = stringProp1;
            }
            
            public string StringProp1 { get; }
            public string StringProp2 { get; }
        }
        

        [Fact]
        public void ShouldResolvePropertyNames()
        {
            var props = BlUtils.ResolvePropertyNameArrayExpression<ChildEntity>(c => new []{c.StringProp1, c.StringProp2});
            
            Assert.NotEmpty(props);
            Assert.Equal("StringProp1", props[0]);
            Assert.Equal("StringProp2", props[1]);
        }

        [Fact]
        public void ShouldFailTryingToResolvePropertyNamesWithLiterals()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                BlUtils.ResolvePropertyNameArrayExpression<ChildEntity>(c => new[] {"1", "2", "3"});
            });
        }
        
        [Fact]
        public void ShouldFailTryingToResolvePropertyNamesWithIncorrectInit()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                BlUtils.ResolvePropertyNameArrayExpression<ChildEntity>(c => Array.Empty<string>());
            });
        }
    }
}