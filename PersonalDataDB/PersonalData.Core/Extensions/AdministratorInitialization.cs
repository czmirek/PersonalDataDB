namespace PersonalData.Core.Extensions
{
    using System;
    public class AdministratorInitialization : IResponsiblePerson
    {
        public string FullName { get; set; } = String.Empty;

        public string Phone { get; set; } = String.Empty;

        public string Email { get; set; } = String.Empty;

        public string? OfficeNumber { get; set; }

        public string? InternalPhoneNumber { get; set; }

        public string? Supervisor { get; set; }
    }
}