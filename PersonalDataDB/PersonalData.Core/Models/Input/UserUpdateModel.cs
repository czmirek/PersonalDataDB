namespace PersonalData.Core
{
    using System;
    public class UserUpdateModel : IUser
    {
        public UserUpdateModel(object id, string phone, string email, string? officeNumber = null, string? internalPhoneNumber = null, string? supervisor = null)
        {
            this.ID = id;
            this.Phone = phone;
            this.Email = email;
            this.OfficeNumber = officeNumber;
            this.InternalPhoneNumber = internalPhoneNumber;
            this.Supervisor = supervisor;
        }

        public object ID { get; private set; }

        string IResponsiblePerson.FullName => String.Empty;

        public string Phone { get; private set; }

        public string Email { get; private set; }

        public string? OfficeNumber { get; private set; }

        public string? InternalPhoneNumber { get; private set; }

        public string? Supervisor { get; private set; }
    }
}