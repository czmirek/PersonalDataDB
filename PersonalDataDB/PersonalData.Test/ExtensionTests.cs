namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Core.Extensions;
    using PersonalData.Provider.InMemory;
    using Xunit;
    public class ExtensionTests
    {
        [Fact]
        public void Create()
        {
            var builder = new DefaultInitializationDataProvider()
                .UseDataManager(new DataManagerInitialization()
                {
                    Name = "SomeCompany",
                    Address = "Street 1",
                    Email = "company@example.com",
                    Phone = "123456789",
                    RegistrationNumber = "123",
                    PersonalDataRegistrationNumber = "456"
                })
                .UseAdministrator(new AdministratorInitialization()
                {
                    FullName = "Karel Admin",
                    Phone = "123456789",
                    Email = "karel.admin@example.com",
                    OfficeNumber = "123",
                    InternalPhoneNumber = "456",
                    Supervisor = "Charles Supervisor"
                })
                .UseConfiguration(new ConfigurationInitialization()
                {
                    AllowPurposeChoiceOnAgreementCreation = false
                })
                .UseSchema((b) =>
                {
                    b.AddTable("Persons", tb =>
                    {
                        tb.Name = "Persons table";
                        tb.Description = "Contains personal data of people";
                        tb.AddColumn("FullName", ColumnType.String, false);
                        tb.AddColumn("Phone", ColumnType.String, true);
                    });
                    b.AddTable("GPSPositions", tb =>
                    {
                        tb.Name = "GPS positions";
                        tb.Description = "Tracking data of people";
                        tb.AddForeignKeyColumn("PersonID", "Persons", false);
                        tb.AddColumn("Latitude", ColumnType.Double, false);
                        tb.AddColumn("Longitude", ColumnType.Double, false);
                    });
                });
            
            var pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
        }       
    }
}