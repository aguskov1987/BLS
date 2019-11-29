using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using ChangeTracking;
using Moq;
using Xunit;

namespace BLS.Tests
{
    public class RelationTests
    {
        [Fact]
        public void should_find_related_pawns_in_bls_buffer_no_filter_no_soft_delete()
        {
            // Setup
            var cursor = new StorageCursor<Lawyer>();
            var storedFirm = new LawFirm {Name = "LLP"};

            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock.Setup(pr => pr.GetById<LawFirm>(
                "law_firm_id",
                "LawFirm"))
                .Returns(storedFirm);
            

            var bls = new Bls(storageProviderMock.Object);
            bls.RegisterBlPawns(new LawFirm(), new Lawyer(), new Assistant(), new Matter(), new Client());

            // Act
            LawFirm firm = bls.GetById<LawFirm>("law_firm_id");
            Lawyer lawyer = bls.SpawnNew<Lawyer>();
            lawyer.FirstName = "George";
            cursor.BlsInMemoryCursorBuffer.Add(lawyer);
            firm.Lawyers.Connect(lawyer);

            StorageCursor<Lawyer> cr = firm.Lawyers.Find();
            List<Lawyer> pawns = cr.GetAll();

            // Assert
            Assert.NotEmpty(pawns);
            Assert.Equal("George", pawns[0].FirstName);
        }

        [Fact]
        public void should_find_related_pawns_in_bls_buffer_with_filter_no_soft_delete()
        {
            // Setup
            var cursor = new StorageCursor<Lawyer>();
            var storedFirm = new LawFirm {Name = "LLP"};

            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock.Setup(pr => pr.GetById<LawFirm>(
                "law_firm_id",
                "LawFirm"))
                .Returns(storedFirm);
            

            var bls = new Bls(storageProviderMock.Object);
            bls.RegisterBlPawns(new LawFirm(), new Lawyer(), new Assistant(), new Matter(), new Client());
            
            // Act
            LawFirm firm = bls.GetById<LawFirm>("law_firm_id");
            
            Lawyer lawyer = bls.SpawnNew<Lawyer>();
            lawyer.FirstName = "George";
            cursor.BlsInMemoryCursorBuffer.Add(lawyer);
            firm.Lawyers.Connect(lawyer);
            
            Lawyer lawyer2 = bls.SpawnNew<Lawyer>();
            lawyer2.FirstName = "Mark";
            cursor.BlsInMemoryCursorBuffer.Add(lawyer2);
            firm.Lawyers.Connect(lawyer2);
            
            Lawyer lawyer3 = bls.SpawnNew<Lawyer>();
            lawyer2.FirstName = "Robert";
            cursor.BlsInMemoryCursorBuffer.Add(lawyer3);
            firm.Lawyers.Connect(lawyer3);
            
            StorageCursor<Lawyer> cr = firm.Lawyers.Find(l => l.FirstName == "George");
            List<Lawyer> pawns = cr.GetAll();
            
            // Assert
            Assert.NotEmpty(pawns);
            Assert.Equal("George", pawns[0].FirstName);
        }

        [Fact]
        public void should_not_find_pawn_in_bls_buffer_if_it_has_been_disconnected()
        {
            // Setup
            var cursor = new StorageCursor<Lawyer>();
            var storedFirm = new LawFirm {Name = "LLP"};

            var storageProviderMock = new Mock<IBlStorageProvider>();
            storageProviderMock.Setup(pr => pr.GetById<LawFirm>(
                    "law_firm_id",
                    "LawFirm"))
                .Returns(storedFirm);
            

            var bls = new Bls(storageProviderMock.Object);
            bls.RegisterBlPawns(new LawFirm(), new Lawyer(), new Assistant(), new Matter(), new Client());

            // Act
            LawFirm firm = bls.GetById<LawFirm>("law_firm_id");
            Lawyer lawyer = bls.SpawnNew<Lawyer>();
            lawyer.FirstName = "George";
            cursor.BlsInMemoryCursorBuffer.Add(lawyer);
            firm.Lawyers.Connect(lawyer);
            firm.Lawyers.Disconnect(lawyer);

            StorageCursor<Lawyer> cr = firm.Lawyers.Find();
            List<Lawyer> pawns = cr.GetAll();

            // Assert
            Assert.Empty(pawns);
        }
        
        [Fact]
        public void should_find_related_pawns_in_storage_no_filter_no_soft_delete()
        {
            // Setup
            var cursor = new StorageCursor<Lawyer>();
            var storedFirm = new LawFirm {Name = "LLP"};
            
            // setting the ID so it looks like the object is coming from storage
            storedFirm.SetId("law_firm_id");

            var storageProviderMock = new Mock<IBlStorageProvider>();

            storageProviderMock.Setup(pr => pr.GetByRelation<Lawyer>(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null,
                    200))
                .Returns(cursor)
                .Verifiable();
            
            storageProviderMock.Setup(pr => pr.GetById<LawFirm>(
                    "law_firm_id",
                    "LawFirm"))
                .Returns(storedFirm);
            

            var bls = new Bls(storageProviderMock.Object);
            bls.RegisterBlPawns(new LawFirm(), new Lawyer(), new Assistant(), new Matter(), new Client());
            
            // Act
            LawFirm firm = bls.GetById<LawFirm>("law_firm_id");
            Lawyer lawyer = bls.SpawnNew<Lawyer>();
            lawyer.FirstName = "George";
            firm.Lawyers.Connect(lawyer);
            
            var existingLawyerInStorage = new Lawyer();
            existingLawyerInStorage.SetId("lawyer_id");
            existingLawyerInStorage.FirstName = "Peter";
            var traceableLawyer = existingLawyerInStorage.AsTrackable();
            cursor.StorageObjectBuffer.Add(traceableLawyer);
            bls.ToUpdate.Add(traceableLawyer);

            StorageCursor<Lawyer> cr = firm.Lawyers.Find();
            storageProviderMock.Verify();
            
            List<Lawyer> pawns = cr.GetAll();

            // Assert
            
            Assert.NotEmpty(pawns);
            Assert.Equal("George", pawns[0].FirstName);
            Assert.Equal("Peter", pawns[1].FirstName);
        }
    }
}