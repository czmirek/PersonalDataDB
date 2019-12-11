namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class DataManagerTests
    {
        [Fact]
        public void List_DataManagers()
        {
            var builder = DataBuilderFake.GetFake();
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
    }
}
