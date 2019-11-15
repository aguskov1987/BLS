using Xunit;

namespace BLS.Tests
{
    /// <summary>
    /// Entities are basic building blocks of the Business Logic System. Entities contain
    /// properties (value types) and are connected to other entities through relations. Relations
    /// are represented by two classes: RelatesToMany and RelatedToOne. Both should be initialized
    /// in the entity's constructor with it's (entity's) reference
    /// </summary>
    public class BlEntityTests
    {
        class SimpleEntity : BlEntity
        {
            public string Property { get; set; }
        }

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
        public void ShouldCreateAnEntity()
        {
            var entity = new SimpleEntity { Property = "property"};

            Assert.NotNull(entity);
            Assert.Null(entity.Id);
        }

        [Fact]
        public void ShouldCreateAnEntityWithOneToManyRelation()
        {
            var parentEntity = new ParentEntity();

            Assert.NotNull(parentEntity);
            Assert.NotNull(parentEntity.Children);
            Assert.Equal(BlConnectionType.OneToMany, parentEntity.Children.ConnectionType);
        }

        [Fact]
        public void ShouldCreateAnEntityWithOneToOneRelation()
        {
            var childEntity = new ChildEntity();

            Assert.NotNull(childEntity);
            Assert.NotNull(childEntity.Parent);
            Assert.Equal(BlConnectionType.OneToOne, childEntity.Parent.ConnectionType);
        }
    }
}