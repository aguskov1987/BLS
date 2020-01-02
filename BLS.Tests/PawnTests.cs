using System.Collections.Generic;
using ChangeTracking;
using Xunit;

namespace BLS.Tests
{
    public class PawnTests
    {
        [Fact]
        public void ShouldTrackChanges()
        {
            // Setup
            var pn = new BasicPawn();
            var basicPawn = pn.AsTrackable();
            
            // Act
            basicPawn.Name = "Some Name";
            var traceable = basicPawn.CastToIChangeTrackable();
            
            // Assert
            Assert.True(traceable.IsChanged);
            Assert.Equal(ChangeStatus.Changed, traceable.ChangeTrackingStatus);
        }

        [Fact]
        public void should_add_to_set_two_distinct_pawns_with_no_ids()
        {
            // Setup
            var set = new HashSet<BlsPawn>();
            BasicPawn p1 = new BasicPawn {Name = "p1"};
            BasicPawn p2 = new BasicPawn {Name = "p2"};

            // Act
            set.Add(p1);
            set.Add(p2);

            // Assert
            Assert.Equal(2, set.Count);
        }
        
        [Fact]
        public void should_add_to_set_two_distinct_pawns_with_one_id()
        {
            // Setup
            var set = new HashSet<BlsPawn>();
            BasicPawn p1 = new BasicPawn {Name = "p1"};
            BasicPawn p2 = new BasicPawn {Name = "p2"};
            p2.SetId("id_of_p2");

            // Act
            set.Add(p1);
            set.Add(p2);

            // Assert
            Assert.Equal(2, set.Count);
        }
        
        [Fact]
        public void should_add_to_set_one_distinct_pawns_from_two_same_ids()
        {
            // Setup
            var set = new HashSet<BlsPawn>();
            BasicPawn p1 = new BasicPawn {Name = "p1"};
            p1.SetId("id_of_p");
            BasicPawn p2 = new BasicPawn {Name = "p2"};
            p2.SetId("id_of_p");

            // Act
            set.Add(p1);
            set.Add(p2);

            // Assert
            Assert.Single(set);
        }
        
        [Fact]
        public void should_add_to_set_one_distinct_pawns_from_two_instances_with_id()
        {
            // Setup
            var set = new HashSet<BlsPawn>();
            BasicPawn p1 = new BasicPawn {Name = "p1"};
            p1.SetId("id_of_p");

            // Act
            set.Add(p1);
            set.Add(p1);

            // Assert
            Assert.Single(set);
        }
        
        [Fact]
        public void should_add_to_set_one_distinct_pawns_from_two_instances_no_ids()
        {
            // Setup
            var set = new HashSet<BlsPawn>();
            BasicPawn p1 = new BasicPawn {Name = "p1"};

            // Act
            set.Add(p1);
            set.Add(p1);

            // Assert
            Assert.Single(set);
        }
    }
}