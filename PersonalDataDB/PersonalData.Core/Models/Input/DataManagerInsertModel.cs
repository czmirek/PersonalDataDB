namespace PersonalData.Core
{
    using System;
    public sealed class DataManagerInsertModel : IDataManager
    {
        public DataManagerInsertModel(string name, string address, string email, string phone, string? registrationNumber = null, string? personalDataRegistrationNumber = null)
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
            RegistrationNumber = registrationNumber;
            PersonalDataRegistrationNumber = personalDataRegistrationNumber;
        }

        object IDataManager.ID => new object();

        public string Name { get; private set; }

        public string Address { get; private set; }

        public string Email { get; private set; }

        public string Phone { get; private set; }

        public string? RegistrationNumber { get; private set; }

        public string? PersonalDataRegistrationNumber { get; private set; }
    }
}