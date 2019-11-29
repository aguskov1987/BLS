using System;
using System.Collections.Generic;
using System.Globalization;
using BLS.Functional;
using BLS.PropertyValidation;
using Xunit;

// ReSharper disable UnusedMember.Local

namespace BLS.Tests
{
    public class BlGraphResolvingContainersTests
    {
        #region Test helpers

        class Client : BlsPawn
        {
            // privates are allowed and should not affect the output of the bl graph compilation
            private string _value;

            // strings can have full text and length restriction attributes
            [FullTextSearchable]
            [StringLengthRestriction(MinCharacters = 2, MaxCharacters = 100)]
            public string FirstName { get; set; }

            [FullTextSearchable] public string LastName { get; set; }

            // numeric properties can have numeric restrictions. If no limit is specified
            // min is set to 0 and max is set to int.MaxValue
            [NumberRestriction] public int TotalNumberOfOrders { get; set; }

            [NumberRestriction(Maximum = 10)] public int NumberOfActiveOrders { get; set; }

            // properties can be used as soft delete flags if marked with the
            // attribute. Only one per pawn is allowed
            [UsedForSoftDeletes] public bool Active { get; set; }

            // collections can have minimum and maximum count restriction if the attribute is applied
            [DateRestriction(Latest = "2020-01-01")]
            public DateTime MaturityDate { get; set; }

            // methods are allowed and should not affect the compilation
            public void SetTheValue(string v)
            {
                _value = v;
            }

            public string GetTheValue()
            {
                return _value;
            }
        }

        class PawnWithTwoSoftDeleteFlags : BlsPawn
        {
            [UsedForSoftDeletes] public bool Flag1 { get; set; }
            [UsedForSoftDeletes] public bool Flag2 { get; set; }
        }

        class PawnWithFtsOnNonStringProp : BlsPawn
        {
            [FullTextSearchable] public int Prop { get; set; }
        }

        class PawnWithStringPropAndNumericRestriction : BlsPawn
        {
            [NumberRestriction] public string Prop { get; set; }
        }

        class PawnWithStringPropAndDateRestriction : BlsPawn
        {
            [DateRestriction(Latest = "2020-01-01")]
            public string Prop { get; set; }
        }

        class PawnWithIntPropAndStringRestriction : BlsPawn
        {
            [StringLengthRestriction] public int Prop { get; set; }
        }

        class PawnWithFloatPropAndStringRestriction : BlsPawn
        {
            [StringLengthRestriction] public float Prop { get; set; }
        }

        class PawnWithCollectionPropAndNumericRestriction : BlsPawn
        {
            [NumberRestriction(Minimum = 1, Maximum = 100)]
            public List<int> Prop { get; set; }
        }

        BlGraph BuildGraph()
        {
            var graph = new BlGraph();
            graph.RegisterPawns(new BlsPawn[]
            {
                new Client(),
                new PawnWithTwoSoftDeleteFlags(),
                new PawnWithFtsOnNonStringProp(),
                new PawnWithStringPropAndNumericRestriction(),
                new PawnWithStringPropAndDateRestriction(),
                new PawnWithIntPropAndStringRestriction(),
                new PawnWithFloatPropAndStringRestriction(),
                new PawnWithCollectionPropAndNumericRestriction()
            });
            return graph;
        }

        #endregion

        [Fact]
        public void should_fail_to_bind_if_second_soft_delete_flag_is_found()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<DuplicateSoftDeletionFlagError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithTwoSoftDeleteFlags());
            });
        }

        [Fact]
        public void should_fail_to_bind_ftx_if_not_string_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<InvalidFullTextSearchAttributeError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithFtsOnNonStringProp());
            });
        }

        [Fact]
        public void should_fail_to_bind_numeric_restriction_to_string_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<InvalidRestrictiveAttributeError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithStringPropAndNumericRestriction());
            });
        }

        [Fact]
        public void should_fail_to_bind_collection_restriction_to_string_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<InvalidRestrictiveAttributeError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithStringPropAndDateRestriction());
            });
        }

        [Fact]
        public void should_fail_to_bind_string_restriction_to_int_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<InvalidRestrictiveAttributeError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithIntPropAndStringRestriction());
            });
        }

        [Fact]
        public void should_fail_to_bind_string_restriction_to_float_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<InvalidRestrictiveAttributeError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithFloatPropAndStringRestriction());
            });
        }

        [Fact]
        public void should_fail_to_bind_numeric_restriction_to_collection_prop()
        {
            // Setup
            var graph = BuildGraph();

            // Act & Assert
            Assert.Throws<DisallowedPawnPropertyError>(() =>
            {
                graph.ResolveContainerMetadataFromPawnSubClass(new PawnWithCollectionPropAndNumericRestriction());
            });
        }

        [Fact]
        public void should_resolve_pawn_metadata()
        {
            // Setup
            var graph = BuildGraph();

            // Act
            graph.ResolveContainerMetadataFromPawnSubClass(new Client());

            // Assert
            Assert.NotEmpty(graph.CompiledCollections);

            var compiledCollection = graph.CompiledCollections[0];
            Assert.Equal("Client", compiledCollection.BlContainerName);
            Assert.Equal("Client", compiledCollection.StorageContainerName);

            Assert.NotEmpty(compiledCollection.Properties);

            Assert.Collection(compiledCollection.Properties, prop =>
            {
                Assert.Equal("FirstName", prop.Name);
                Assert.Equal(typeof(string), prop.PropType);
                Assert.True(prop.IsSearchable);
                Assert.Equal(2, prop.MinChar);
                Assert.Equal(100, prop.MaxChar);
            }, prop =>
            {
                Assert.Equal("LastName", prop.Name);
                Assert.Equal(typeof(string), prop.PropType);
                Assert.True(prop.IsSearchable);
            }, prop =>
            {
                Assert.Equal("TotalNumberOfOrders", prop.Name);
                Assert.Equal(typeof(int), prop.PropType);
                Assert.Equal(0, prop.MinValue);
                Assert.Equal(float.MaxValue, prop.MaxValue);
            }, prop =>
            {
                Assert.Equal("NumberOfActiveOrders", prop.Name);
                Assert.Equal(typeof(int), prop.PropType);
                Assert.Equal(0, prop.MinValue);
                Assert.Equal(10, prop.MaxValue);
            }, prop =>
            {
                Assert.Equal("Active", prop.Name);
                Assert.Equal(typeof(bool), prop.PropType);
                Assert.True(prop.IsSoftDeleteProp);
            }, prop =>
            {
                Assert.Equal("MaturityDate", prop.Name);
                Assert.Equal(typeof(DateTime), prop.PropType);
                Assert.Equal("01/01/2020 00:00:00", prop.LatestDate.ToString(CultureInfo.InvariantCulture));
            }, prop =>
            {
                Assert.Equal("Created", prop.Name);
                Assert.Equal(typeof(DateTime), prop.PropType);
            }, prop =>
            {
                Assert.Equal("LastTimeModified", prop.Name);
                Assert.Equal(typeof(DateTime), prop.PropType);
            });
        }
    }
}