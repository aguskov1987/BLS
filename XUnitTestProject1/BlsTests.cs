using ChangeTracking;
using Xunit;

namespace BLS.Tests
{
    public class Pawn : BlsPawn
    {
        public virtual string Name { get; set; }
    }
    public class BlsTests
    {
        [Fact]
        public void ShouldSpawnNewPawn()
        {
            Bls bls = new Bls(null);
            Pawn pawn = bls.SpawnNew<Pawn>();
            
            Assert.NotNull(pawn);
        }

        [Fact]
        public void ShouldTrackChanges()
        {
            Bls bls = new Bls(null);
            Pawn pawn = bls.SpawnNew<Pawn>();

            pawn.Name = "Some Name";
            var traceable = pawn.CastToIChangeTrackable();
            
            Assert.True(traceable.IsChanged);
            Assert.Equal(ChangeStatus.Changed, traceable.ChangeTrackingStatus);
        }

        [Fact]
        public void ShouldRegisterPawns()
        {
            
        }
    }
}