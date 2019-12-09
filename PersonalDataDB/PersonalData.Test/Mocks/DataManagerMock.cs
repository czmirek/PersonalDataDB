namespace PersonalData.Test
{
    using PersonalData.Core;
    using System;

    public class DataManagerMock : IDataManager
    {
        public string Name { get; set; } = String.Empty;

        public string Address { get; set; } = String.Empty;

        public string Email { get; set; } = String.Empty;

        public string Phone { get; set; } = String.Empty;

        public string? RegistrationNumber { get; set; }

        public string? PersonalDataRegistrationNumber { get; set; }
    }
}
