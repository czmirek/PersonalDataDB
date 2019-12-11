namespace PersonalData.Test
{
    using PersonalData.Core;
    using System.Collections.Generic;
    public class DataBuilderFake
    {
        public static DefaultInitializationDataProvider GetFake() => new DefaultInitializationDataProvider
        {
            DataManager = new DataManagerMock()
            {
                Name = "Some Company Ltd.",
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
            },
            Configuration = new ConfigurationMock()
            {
                AllowPurposeChoiceOnAgreementCreation = true
            },
            Schema = new SchemaMock()
            {
                TableMocks = new List<TableDefinitionMock>()
                {
                    new TableDefinitionMock()
                    {
                        ID = "Persons",
                        Name = "Personal data",
                        Description = "Table containing basic personal data",
                        ColumnMocks = new List<ColumnDefinitionMock>()
                        {
                            new ColumnDefinitionMock() {ID = "FullName", ColumnType = ColumnType.String,  IsNullable = false },
                            new ColumnDefinitionMock() {ID = "Birthday", ColumnType = ColumnType.DateTime, IsNullable = false },
                            new ColumnDefinitionMock() {ID = "NumberOfLimbs", ColumnType = ColumnType.Integer, IsNullable = false },
                            new ColumnDefinitionMock() {ID = "BankAccount", ColumnType = ColumnType.Decimal, IsNullable = false },
                            new ColumnDefinitionMock() {ID = "Latitude", ColumnType = ColumnType.Double, IsNullable = true }
                        }
                    },
                    new TableDefinitionMock()
                    {
                        ID = "GpsPosition",
                        Name = "Person tracking",
                        Description = "Table containing positional tracking data",
                        ColumnMocks = new List<ColumnDefinitionMock>()
                        {
                            new ColumnDefinitionMock() { ID = "PersonsID", ColumnType = ColumnType.ForeignKey, ForeignKeyReferenceTableID = "Persons" },
                            new ColumnDefinitionMock() { ID = "Longitude", ColumnType = ColumnType.Double, IsNullable = false },
                            new ColumnDefinitionMock() { ID = "Latitude", ColumnType = ColumnType.Double, IsNullable = false }
                        }
                    }
                }
            }
        };
    }
}