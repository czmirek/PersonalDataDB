namespace PersonalData.Core
{
    public sealed class AdministratorInsertModel : IAdministrator
    {
        public AdministratorInsertModel(string fullName, string phone, string email, string? officeNumber = null, string? internalPhoneNumber = null, string? supervisor = null)
        {
            this.FullName = fullName;
            this.Phone = phone;
            this.Email = email;
            this.OfficeNumber = officeNumber;
            this.InternalPhoneNumber = internalPhoneNumber;
            this.Supervisor = supervisor;
        }

        object IAdministrator.ID => new object();

        public string FullName { get; private set; }

        public string Phone { get; private set; }

        public string Email { get; private set; }

        public string? OfficeNumber { get; private set; }

        public string? InternalPhoneNumber { get; private set; }

        public string? Supervisor { get; private set; }
    }
}