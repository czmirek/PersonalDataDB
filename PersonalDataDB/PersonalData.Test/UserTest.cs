namespace PersonalData.Test
{
    using PersonalData.Core;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class UserTest
    {
        [Fact]
        public void List_And_Insert_Users()
        {
            PersonalDataDB pddb = PddbTestingInstance.GetPddbInstanceForTesting();
            var users = pddb.ListUsers();

            Assert.Empty(users);

            IAdministrator admin = pddb.ListAdministrators().Single();
            object newId = pddb.InsertUser(admin.ID, new UserInsertModel("Karel", "123", "email@example.com", "5", "6", "Some Supervisor"));
            users = pddb.ListUsers();
            Assert.Single(users);
            Assert.Equal(newId, users.Single().ID);

            var insertedUser = users.Single();

            Assert.Equal("Karel", insertedUser.FullName);
            Assert.Equal("123", insertedUser.Phone);
            Assert.Equal("email@example.com", insertedUser.Email);
            Assert.Equal("5", insertedUser.OfficeNumber);
            Assert.Equal("6", insertedUser.InternalPhoneNumber);
            Assert.Equal("Some Supervisor", insertedUser.Supervisor);
        }

        [Fact]
        public void Update_User()
        {
            PersonalDataDB pddb = PddbTestingInstance.GetPddbInstanceForTesting();
            var users = pddb.ListUsers();

            Assert.Empty(users);

            IAdministrator admin = pddb.ListAdministrators().Single();
            object newId = pddb.InsertUser(admin.ID, new UserInsertModel("Karel", "123", "email@example.com", "5", "6", "Some Supervisor"));
            pddb.UpdateUser(admin.ID, new UserUpdateModel(newId, "99999", "anotheremail@email.com", null, null, null));
            
            var insertedUser = pddb.ListUsers().Single();

            Assert.Equal("Karel", insertedUser.FullName);
            Assert.Equal("99999", insertedUser.Phone);
            Assert.Equal("anotheremail@email.com", insertedUser.Email);
            Assert.Null(insertedUser.OfficeNumber);
            Assert.Null(insertedUser.InternalPhoneNumber);
            Assert.Null(insertedUser.Supervisor);
        }
    }
}
