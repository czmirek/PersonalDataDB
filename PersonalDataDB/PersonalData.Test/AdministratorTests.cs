namespace PersonalData.Test
{
    using PersonalData.Core;
    using PersonalData.Provider.InMemory;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    public class AdministratorTests
    {
        [Fact]
        public void List_Administrators()
        {
            var builder = PddbTestingInstance.GetFake();
            PersonalDataDB pddb = new PersonalDataDB(new InMemoryDataProvider());
            pddb.CreateDatabase(builder);

            IEnumerable<IAdministrator> administrators = pddb.ListAdministrators();
            Assert.NotEmpty(administrators);
            Assert.Single(administrators);

            IAdministrator administrator = administrators.Single();
            Assert.Equal(builder.Administrator!.FullName, administrator.FullName);
            Assert.Equal(builder.Administrator!.Email, administrator.Email);
            Assert.Equal(builder.Administrator!.InternalPhoneNumber, administrator.InternalPhoneNumber);
            Assert.Equal(builder.Administrator!.OfficeNumber, administrator.OfficeNumber);
            Assert.Equal(builder.Administrator!.Phone, administrator.Phone);
            Assert.Equal(builder.Administrator!.Supervisor, administrator.Supervisor);
        }

        [Fact]
        public void Insert_Administrator()
        {
            PersonalDataDB pddb = PddbTestingInstance.GetPddbInstanceForTesting();

            IAdministrator admin = pddb.ListAdministrators().Single();
            object newAdminId = pddb.InsertAdministrator(admin.ID, new AdministratorInsertModel("Karel Varel", "123456789", "karel.varel@example.com", null, null, null));

            var administrators = pddb.ListAdministrators();

            Assert.Equal(2, administrators.Count());

            Assert.Contains(administrators, t => t.ID.Equals(newAdminId));

            IAdministrator newAdmin = administrators.FirstOrDefault(t => t.ID.Equals(newAdminId));
            Assert.Equal("Karel Varel", newAdmin.FullName);
            Assert.Equal("123456789", newAdmin.Phone);
            Assert.Equal("karel.varel@example.com", newAdmin.Email);
        }

        [Fact]
        public void Update_Administrator()
        {
            PersonalDataDB pddb = PddbTestingInstance.GetPddbInstanceForTesting();
            IAdministrator admin = pddb.ListAdministrators().Single();

            pddb.UpdateAdministrator(admin.ID, new AdministratorUpdateModel(admin.ID, "999", "dummy@example.com", "666", "satan", "super"));

            var administrators = pddb.ListAdministrators();

            Assert.Single(administrators);
            
            admin = pddb.ListAdministrators().Single();

            Assert.Equal("Karel Admin", admin.FullName);
            Assert.Equal("999", admin.Phone);
            Assert.Equal("dummy@example.com", admin.Email);
            Assert.Equal("666", admin.OfficeNumber);
            Assert.Equal("satan", admin.InternalPhoneNumber);
            Assert.Equal("super", admin.Supervisor);
        }
    }
}