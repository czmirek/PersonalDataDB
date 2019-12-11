namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using Xunit;
    public class InMemoryTests
    {
        
        [Fact]
        public void Create()
        {
            
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(PddbTestingInstance.GetFake());
        }

        [Fact]
        public void Create_IfNotExist()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(PddbTestingInstance.GetFake());
        }

        [Fact]
        public void Create_IfNotExist_Twice()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(PddbTestingInstance.GetFake());
            pddb.CreateDatabaseIfNotExist(PddbTestingInstance.GetFake());
        }

        [Fact]
        public void Create_Twice_Throws_Exception()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(PddbTestingInstance.GetFake());
            Assert.Throws<PersonalDataDBException>(() =>
            {
                pddb.CreateDatabase(PddbTestingInstance.GetFake());
            });
        }
    }
}
