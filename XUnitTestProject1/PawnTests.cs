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
    }
}