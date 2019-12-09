namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using Xunit;

    public class InitializationTests
    {
        [Fact]
        public void Initialization_Valid_Test()
        {
            var builder = new InitializationDataBuilder
            {
                DataManager = new DataManagerMock()
                {
                    Name = "Some Company",
                    Address = "Street 1, Prague 10000, Czech Republic",
                    Email = "some@example.com",
                    Phone = "+420123456789",
                    RegistrationNumber = "123456789",
                    PersonalDataRegistrationNumber = "987654321"
                },
                Administrator = new ResponsiblePersonMock()
                {
                    FullName = "Karel Admin",
                    Email = "karel.admin@example.com",
                    Phone = "123456789",
                    OfficeNumber = "123",
                    InternalPhoneNumber = "10",
                    Supervisor = "Jaroslav Nadřízený"
                }
            };

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
        }

        [Fact]
        public void Initialization_Invalid_DataManaber()
        {
            var builder = new InitializationDataBuilder
            {
                DataManager = new DataManagerMock()
                {
                    Name = "",
                    Address = "Street 1, Prague 10000, Czech Republic",
                    Email = "some@example.com",
                    Phone = "+420123456789",
                    RegistrationNumber = "123456789",
                    PersonalDataRegistrationNumber = "987654321"
                },
                Administrator = new ResponsiblePersonMock()
                {
                    FullName = "Karel Admin",
                    Email = "karel.admin@example.com",
                    Phone = "123456789",
                    OfficeNumber = "123",
                    InternalPhoneNumber = "10",
                    Supervisor = "Jaroslav Nadřízený"
                }
            };

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());

            Assert.Throws<PersonalDataDBException>(() =>
            {
                pddb.CreateDatabase(builder);
            });
            
        }
    }
}
