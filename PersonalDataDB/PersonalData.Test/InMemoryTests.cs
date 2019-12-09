namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using Xunit;
    public class InMemoryTests
    {
        private readonly InitializationDataBuilder builder;

        public InMemoryTests()
        {
            IDataManager dataManager = new DataManagerMock()
            {
                Name = "Some Company",
                Address = "Street 1, Prague 10000, Czech Republic",
                Email = "some@example.com",
                Phone = "+420123456789",
                RegistrationNumber = "123456789",
                PersonalDataRegistrationNumber = "987654321"
            };

            IResponsiblePerson responsiblePerson = new ResponsiblePersonMock()
            {
                FullName = "Karel Admin",
                Email = "karel.admin@example.com",
                Phone = "123456789",
                OfficeNumber = "123",
                InternalPhoneNumber = "10",
                Supervisor = "Jaroslav Nadřízený"
            };

            this.builder = new InitializationDataBuilder()
            {
                DataManager = dataManager,
                Administrator = responsiblePerson
            };
        }

        [Fact]
        public void Create_InMemory_Database()
        {
            
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
        }

        [Fact]
        public void Create_InMemory_Database_IfNotExist()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(builder);
        }

        [Fact]
        public void Create_InMemory_Database_IfNotExist_Twice()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabaseIfNotExist(builder);
            pddb.CreateDatabaseIfNotExist(builder);
        }

        [Fact]
        public void Create_InMemory_Database_Twice()
        {
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
            Assert.Throws<PersonalDataDBException>(() =>
            {
                pddb.CreateDatabase(builder);
            });
        }
    }
}
