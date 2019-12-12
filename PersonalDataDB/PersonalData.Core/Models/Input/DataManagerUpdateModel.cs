namespace PersonalData.Core
{
    using System;
    public sealed class DataManagerUpdateModel : IDataManager
    {
        public DataManagerUpdateModel(object id, string? registrationNumber = null, string? personalDataRegistrationNumber = null)
        {
            ID = id;
            RegistrationNumber = registrationNumber;
            PersonalDataRegistrationNumber = personalDataRegistrationNumber;
        }

        public object ID { get; private set; }
        public string Name => String.Empty;
        public string Address => String.Empty;
        public string Email => String.Empty;
        public string Phone => String.Empty;
        public string? RegistrationNumber { get; private set; }
        public string? PersonalDataRegistrationNumber { get; private set; }
    }
}