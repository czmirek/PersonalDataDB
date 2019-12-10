namespace PersonalData.Core
{
    using System;
    internal class ResponsiblePersonValidator
    {
        private readonly string personType;
        public ResponsiblePersonValidator(string personType)
        {
            this.personType = personType ?? throw new ArgumentNullException(nameof(personType));
        }
        public void Validate(IResponsiblePerson responsiblePerson)
        {
            if (String.IsNullOrEmpty(responsiblePerson.FullName))
                throw new ValidationException($"{personType} full name must not be null or empty");

            if (String.IsNullOrEmpty(responsiblePerson.Phone))
                throw new ValidationException($"{personType} phone must not be null or empty");

            if (String.IsNullOrEmpty(responsiblePerson.Email))
                throw new ValidationException($"{personType} email must not be null or empty");            
        }
    }
}
