namespace PersonalData.Core
{
    using System;
    internal class DataManagerValidator
    {
        public void Validate(IDataManager dataManager)
        {
            if (String.IsNullOrEmpty(dataManager.Name))
                throw new InitializationException("Data manager name must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Address))
                throw new InitializationException("Data manager address must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Email))
                throw new InitializationException("Data manager email must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Phone))
                throw new InitializationException("Data manager phone must not be null or empty");
        }
    }
}
