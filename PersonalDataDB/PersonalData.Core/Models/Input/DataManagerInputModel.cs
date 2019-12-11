namespace PersonalData.Core
{
    using System;
    public class DataManagerInputModel : IDataManager
    {
        public DataManagerInputModel(string name, string address, string email, string phone, string? registrationNumber = null, string? personalDataRegistrationNumber = null)
        {
            Name = name;
            Address = address;
            Email = email;
            Phone = phone;
            RegistrationNumber = registrationNumber;
            PersonalDataRegistrationNumber = personalDataRegistrationNumber;
        }

        public object? ID { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string? RegistrationNumber { get; set; }

        public string? PersonalDataRegistrationNumber { get; set; }
    }
}