namespace PersonalData.Provider.InMemory
{
    using PersonalData.Core;
    using System;
    internal class DataManagerDataModel : IDataManager
    {
        public object? ID { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Address { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Phone { get; set; } = String.Empty;
        public string? RegistrationNumber { get; set; }
        public string? PersonalDataRegistrationNumber { get; set; }
    }
}
