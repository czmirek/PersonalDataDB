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
            var builder = DataBuilderFake.GetFake();
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);
        }


        [Fact]
        public void Missing_DataManager_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            builder.DataManager = null;
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Admin_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            builder.Administrator = null;
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Config_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            builder.Configuration = null;
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Missing_Schema_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            builder.Schema = null;
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_DataManager()
        {
            var builder = DataBuilderFake.GetFake();
            ((DataManagerMock)builder.DataManager!).Name = "";
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());

            Assert.Throws<InitializationException>(() =>
            {
                pddb.CreateDatabase(builder);
            });
            
        }

        [Fact]
        public void Invalid_DataManager_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            ((DataManagerMock)builder.DataManager!).Name = "";

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_Admin_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            ((ResponsiblePersonMock)builder.Administrator!).FullName = "";

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));

        }

        [Fact]
        public void Invalid_Schema_No_Tables_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            ((SchemaMock)builder.Schema!).TableMocks = new List<TableDefinitionMock>();

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_Duplicate_Tables_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ID = sm.TableMocks[0].ID;

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_No_Column_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ColumnMocks.Clear();

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }

        [Fact]
        public void Invalid_Schema_Duplicate_Columns_Throws_Exception()
        {
            var builder = DataBuilderFake.GetFake();
            SchemaMock sm = ((SchemaMock)builder.Schema!);
            sm.TableMocks[1].ColumnMocks[0].ID = sm.TableMocks[1].ColumnMocks[1].ID;

            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            Assert.Throws<InitializationException>(() => pddb.CreateDatabase(builder));
        }
    }
}
