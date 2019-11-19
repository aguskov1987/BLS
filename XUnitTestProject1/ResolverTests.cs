using System.Collections.Generic;
using BLS.Tests.Mocks_and_Doubles;
using BLS.Utilities;
using Xunit;

namespace BLS.Tests
{
    public class ResolverTests
    {
        class PersonEntity : BlEntity
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        class ParentEntity : PersonEntity
        {
            public ParentEntity()
            {
                Children = new RelatesToMany<ChildEntity>(this);
                Parent = new RelatesToOne<ParentEntity>(this);
            }

            public RelatesToMany<ChildEntity> Children { get; }
            public RelatesToOne<ParentEntity> Parent { get; }
        }

        class ChildEntity: PersonEntity
        {
            public ChildEntity()
            {
                Parent = new RelatesToOne<ParentEntity>(this);
            }
            
            public RelatesToOne<ParentEntity> Parent { get; }
        }
        
        [Fact]
        public void ShouldCreateAResolver()
        {
            var resolver = new BlRelationResolver();

            Assert.NotNull(resolver);
        }
        
        [Fact]
        public void ShouldResolveEntityNameAndItsRelations()
        {
            var system = new BlSystem(new StorageProviderMock());
            
            var resolver = new BlRelationResolver();
            resolver.AddEntityWithRelation(new ParentEntity());
            resolver.AddEntityWithRelation(new ChildEntity());

            List<string> names = resolver.GetResolvedContainers();
            
            Assert.NotEmpty(names);
            Assert.Equal("ParentEntity", names[0]);
            Assert.Equal("ChildEntity", names[1]);
        }
    }
}