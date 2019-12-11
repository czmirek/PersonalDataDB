namespace PersonalData.Core
{
    using System;

    [Serializable]
    public class ValidationException : PersonalDataDBException
    {
        public ValidationException() { }

        public ValidationException(string message) : base(message) {}
    }
}