namespace PersonalData.Core
{
    using System;
    internal class DataManagerValidator
    {
        public void Validate(IDataManager dataManager)
        {
            if (String.IsNullOrEmpty(dataManager.Name))
                throw new PersonalDataDBException("Data manager name must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Address))
                throw new PersonalDataDBException("Data manager address must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Email))
                throw new PersonalDataDBException("Data manager email must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Phone))
                throw new PersonalDataDBException("Data manager phone must not be null or empty");
        }
    }
}
