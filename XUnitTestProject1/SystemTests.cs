using System.Collections.Generic;
using BLS.Storage_Providers;
using BLS.Tests.Mocks_and_Doubles;
using BLS.Utilities;
using Xunit;

namespace BLS.Tests
{
    public class BlSystemTests
    {
        // some internal classes to test the system:
        class SchoolDistrict : BlEntity
        {
            public SchoolDistrict()
            {
                // recursive relation
                ChildDistricts = new RelatesToMany<SchoolDistrict>(this);
                ParentDistrict = new RelatesToOne<SchoolDistrict>(this);

                // child relations
                ElementarySchools = new RelatesToMany<ElementarySchool>(this);
                HighSchools = new RelatesToMany<HighSchool>(this);
            }

            [BlFullTextSearchable]
            public string Name { get; set; }
            
            public RelatesToMany<SchoolDistrict> ChildDistricts { get; }
            public RelatesToMany<ElementarySchool> ElementarySchools { get; }
            public BlConnected<HighSchool> HighSchools { get; }

            public BlConnected<SchoolDistrict> ParentDistrict { get; }

            public List<ElementarySchool> GetAllElementarySchools()
            {
                return ElementarySchools.Find().GetAll();
            }
        }

        class School : BlEntity
        {
            public School()
            {
                ParentDistrict = new RelatesToOne<SchoolDistrict>(this);
            }

            [BlFullTextSearchable]
            public string Name { get; set; }

            [BlSoftDeletable]
            public bool Active { get; set; }

            public BlConnected<SchoolDistrict> ParentDistrict { get; }
        }

        class HighSchool : School { }
        class ElementarySchool : School { }

        class Student : BlEntity
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        [Fact]
        public void ShouldCreateABlSystem()
        {
            var system = new BlSystem(new StorageProviderMock());

            Assert.NotNull(system);
        }

        [Fact]
        public void ShouldRegisterAnEntity()
        {
            var system = new BlSystem(new StorageProviderMock());
            system.RegisterEntity(new SchoolDistrict());
            system.RegisterEntity(new School());
        }
    }
}
