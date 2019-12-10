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
            pddb.CreateDatabase(DataBuilderFake.GetFake());
        }

        [Fact]
        public void Create_IfNotExist()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(DataBuilderFake.GetFake());
        }

        [Fact]
        public void Create_IfNotExist_Twice()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(DataBuilderFake.GetFake());
            pddb.CreateDatabaseIfNotExist(DataBuilderFake.GetFake());
        }

        [Fact]
        public void Create_Twice_Throws_Exception()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(DataBuilderFake.GetFake());
            Assert.Throws<PersonalDataDBException>(() =>
            {
                pddb.CreateDatabase(DataBuilderFake.GetFake());
            });
        }
    }
}
