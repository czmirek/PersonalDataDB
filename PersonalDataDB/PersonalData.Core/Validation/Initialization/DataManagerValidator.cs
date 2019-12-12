namespace PersonalData.Core
{
    using System;
    internal class DataManagerValidator
    {
        private readonly ValidationMode mode;

        public DataManagerValidator(ValidationMode mode)
        {
            this.mode = mode;
        }
        public void Validate(IDataManager dataManager)
        {
            if (mode == ValidationMode.Insert)
            {
                if (String.IsNullOrEmpty(dataManager.Name))
                    throw new ValidationException("Data manager name must not be null or empty");

                if (String.IsNullOrEmpty(dataManager.Address))
                    throw new ValidationException("Data manager address must not be null or empty");
            }

            if (String.IsNullOrEmpty(dataManager.Email))
                throw new ValidationException("Data manager email must not be null or empty");

            if (String.IsNullOrEmpty(dataManager.Phone))
                throw new ValidationException("Data manager phone must not be null or empty");
        }

        public enum ValidationMode
        {
            Insert,
            Update
        }
    }
}
