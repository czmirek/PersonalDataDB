namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using System.Collections.Generic;
    using Xunit;

    public class InitializationTests
    {
        [Fact]
        public void Valid_Test()
        {
            var builder = PddbTestingInstance.GetFake();
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
        }


        [Fact]
        public void Missing_DataManager_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            builder.DataManager = null;
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Admin_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            builder.Administrator = null;
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Config_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            builder.Configuration = null;
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Schema_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            builder.Schema = null;
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_DataManager()
        {
            var builder = PddbTestingInstance.GetFake();
            ((DataManagerMock)builder.DataManager!).Name = "";
            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());

            Assert.Throws<InitializationException>(() =>
            {
                pddb.CreateDatabase(builder);
            });
            
        }

        [Fact]
        public void Invalid_DataManager_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            ((DataManagerMock)builder.DataManager!).Name = "";

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_Admin_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            ((AdministratorMock)builder.Administrator!).FullName = "";

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_Schema_No_Tables_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            ((SchemaMock)builder.Schema!).TableMocks = new List<TableDefinitionMock>();

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_Duplicate_Tables_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ID = sm.TableMocks[0].ID;

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_No_Column_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ColumnMocks.Clear();

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_Duplicate_Columns_Throws_Exception()
        {
            var builder = PddbTestingInstance.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ColumnMocks[0].ID = sm.TableMocks[1].ColumnMocks[1].ID;

            PersonalDataDB pddb = PersonalDataDB.Create(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }
    }
}
