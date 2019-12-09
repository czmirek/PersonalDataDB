﻿namespace PersonalData.Core
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
                throw new PersonalDataDBException($"{personType} full name must not be null or empty");

            if (String.IsNullOrEmpty(responsiblePerson.Phone))
                throw new PersonalDataDBException($"{personType} phone must not be null or empty");

            if (String.IsNullOrEmpty(responsiblePerson.Email))
                throw new PersonalDataDBException($"{personType} email must not be null or empty");            
        }
    }
}
