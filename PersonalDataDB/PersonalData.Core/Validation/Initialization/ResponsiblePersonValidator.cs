namespace PersonalData.Core
{
    using System;
    internal class ResponsiblePersonValidator
    {
        private readonly string personType;
        private readonly ValidationMode validationMode;

        public ResponsiblePersonValidator(string personType, ValidationMode validationMode)
        {
            this.personType = personType ?? throw new ArgumentNullException(nameof(personType));
            this.validationMode = validationMode;
        }
        public void Validate(IResponsiblePerson responsiblePerson)
        {
            if (validationMode == ValidationMode.Insert)
            {
                if (String.IsNullOrEmpty(responsiblePerson.FullName))
                    throw new ValidationException($"{personType} full name must not be null or empty");
            }

            if (String.IsNullOrEmpty(responsiblePerson.Phone))
                throw new ValidationException($"{personType} phone must not be null or empty");

            if (String.IsNullOrEmpty(responsiblePerson.Email))
                throw new ValidationException($"{personType} email must not be null or empty");            
        }

        public enum ValidationMode
        {
            Insert,
            Update
        }
    }
}
