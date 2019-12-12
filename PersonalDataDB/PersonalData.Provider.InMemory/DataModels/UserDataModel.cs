namespace PersonalData.Provider.InMemory
{
    using PersonalData.Core;
    using System;

    internal class UserDataModel : IUser
    {
        public object ID { get; set; } = new object();

        public string FullName { get; set; } = String.Empty;

        public string Phone { get; set; } = String.Empty;

        public string Email { get; set; } = String.Empty;

        public string? OfficeNumber { get; set; }

        public string? InternalPhoneNumber { get; set; }

        public string? Supervisor { get; set; }
    }
}