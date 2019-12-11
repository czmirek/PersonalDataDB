namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DataManagerTests
    {
        [Fact]
        public void List_DataManagers()
        {
            var builder = PddbTestingInstance.GetFake();
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);

            IEnumerable<IDataManager> dataManagers = pddb.ListDataManagers();
            Assert.NotEmpty(dataManagers);
            Assert.Single(dataManagers);

            IDataManager dataManager = dataManagers.Single();
            Assert.Equal(builder.DataManager!.Name, dataManager.Name);
            Assert.Equal(builder.DataManager!.Phone, dataManager.Phone);
            Assert.Equal(builder.DataManager!.Email, dataManager.Email);
            Assert.Equal(builder.DataManager!.RegistrationNumber, dataManager.RegistrationNumber);
            Assert.Equal(builder.DataManager!.PersonalDataRegistrationNumber, dataManager.PersonalDataRegistrationNumber);

        }

        [Fact]
        public void Insert_DataManager()
        {
            var pddb = PddbTestingInstance.GetPddbInstanceForTesting();
            object adminID = pddb.ListAdministrators().Single().ID!;

            object newDataManagerId = pddb.InsertDataManager(adminID, new DataManagerInputModel("Another company", "another address", "another.email@example.com", "123456789", "999", "666"));

            IEnumerable<IDataManager> dataManagers = pddb.ListDataManagers();
            Assert.Equal(2, dataManagers.Count());
            Assert.Single(dataManagers, d => d.ID!.Equals(newDataManagerId));

            IEnumerable<IAdministratorLog> adminLog = pddb.ListAdministratorLogs();
            Assert.Single(adminLog);
        }

        [Fact]
        public void Insert_Invalid_DataManager()
        {
            var pddb = PddbTestingInstance.GetPddbInstanceForTesting();
            object adminID = pddb.ListAdministrators().Single().ID!;
            
            Assert.Throws<ValidationException>(() =>
            {
                pddb.InsertDataManager(adminID, new DataManagerInputModel("", "", "", "", null, null));
            });

            Assert.Throws<PersonalDataDBException>(() =>
            {
                pddb.InsertDataManager(Guid.NewGuid(), new DataManagerInputModel("a", "b", "c", "d", null, null));
            });

        }
    }
}
