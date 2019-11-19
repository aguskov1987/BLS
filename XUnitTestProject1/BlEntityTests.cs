using System;
using System.Collections.Generic;
using System.Diagnostics;
using BLS.Tests.Mocks_and_Doubles;
using BLS.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace BLS.Tests
{
    /// <summary>
    /// Entities are basic building blocks of the Business Logic System. Entities contain
    /// properties and are connected to other entities through relations. Relations
    /// are represented by two classes: RelatesToMany and RelatedToOne. Both should be initialized
    /// in the entity's constructor with it's (entity's) reference
    /// </summary>
    public class BlEntityTests
    {
        private readonly ITestOutputHelper _output;
        
        class PersonFigure : BlEntity
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        class MotherFigure : PersonFigure
        {
            public MotherFigure()
            {
                Children = new RelatesToMany<ChildFigure>(this);
            }

            public RelatesToMany<ChildFigure> Children { get; }
        }

        class FatherFigure : PersonFigure
        {
            public FatherFigure()
            {
                Children = new RelatesToMany<ChildFigure>(this);
            }

            public RelatesToMany<ChildFigure> Children { get; }
        }

        class ChildFigure : PersonFigure
        {
            public ChildFigure()
            {
                Mother = new RelatesToOne<MotherFigure>(this);
                Father = new RelatesToOne<FatherFigure>(this);
            }

            public RelatesToOne<MotherFigure> Mother { get; }
            public RelatesToOne<FatherFigure> Father { get; }
        }

        public BlEntityTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldCreateAnEntity()
        {
            BlUtils.SystemRef = null;
            
            var entity = new PersonFigure { Name = "James Brown"};

            Assert.NotNull(entity);
            Assert.Null(entity.Id);
        }

        [Fact]
        public void ShouldCreateAnEntityWithOneToManyRelation()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new MotherFigure());

            var parentEntity = new MotherFigure();

            Assert.NotNull(parentEntity);
            Assert.NotNull(parentEntity.Children);
            Assert.Equal(BlConnectionType.OneToMany, parentEntity.Children.ConnectionType);
        }

        [Fact]
        public void ShouldCreateAnEntityWithOneToOneRelation()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new ChildFigure());

            var childEntity = new ChildFigure();

            Assert.NotNull(childEntity);
            Assert.NotNull(childEntity.Mother);
            Assert.Equal(BlConnectionType.OneToOne, childEntity.Mother.ConnectionType);
        }

        [Fact]
        public void ShouldBeAbleToHaveAccessToProperties()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new ChildFigure());

            var child = new ChildFigure {Name = "Sophia", Age = 2};
            
            Assert.NotNull(child);
        }

        [Fact]
        public void ShouldBeAbleToHaveAccessToRelations()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new ChildFigure());
            system.RegisterEntity(new MotherFigure());

            var mother = new MotherFigure();
            var child = new ChildFigure {Name = "Sophia", Age = 2};
            
            child.Mother.Connect(mother);
            
            Assert.NotNull(child);
        }

        [Fact]
        public void ShouldFailToPersistIfSystemIsNotInitialized()
        {
            var mother = new MotherFigure();
            mother.Age = 32;
            mother.Persist();
        }

        [Fact]
        public void ShouldFailToPersistIfNoStorageProviderIsSet()
        {
            var system = new BlSystem(null);
            system.RegisterEntity(new ChildFigure());
            system.RegisterEntity(new MotherFigure());
            
            var mother = new MotherFigure();
            var child = new ChildFigure {Name = "Sophia", Age = 2};
            
            child.Mother.Connect(mother);

            Assert.Throws<NullReferenceException>(() => { child.Persist(); });
        }

        [Fact]
        public void ShouldPersistFigureWithEmptyRelations()
        {
            var system = new BlSystem(new StorageProviderMock());
            
            system.RegisterEntity(new ChildFigure());
            
            var child = new ChildFigure {Name = "Sophia", Age = 2};

            child.Persist();

            List<Tuple<string, TransactionStatus>> transactions
                = (system.StorageProvider as StorageProviderMock).Transactions;

            Assert.NotEmpty(transactions);
            Assert.Equal(TransactionStatus.Committed, transactions[0].Item2);
        }

        [Fact]
        public void ShouldPersistFigureWithRelations()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new ChildFigure());
            system.RegisterEntity(new MotherFigure());
            system.RegisterEntity(new FatherFigure());
            
            var child = new ChildFigure {Name = "Sophia", Age = 2};
            var mom = new MotherFigure {Name = "Tatiana", Age = 32};
            var dad = new FatherFigure {Name = "Andrey", Age = 32};
            
            child.Mother.Connect(mom);
            child.Father.Connect(dad);

            mom.Persist();
        }
    }
}