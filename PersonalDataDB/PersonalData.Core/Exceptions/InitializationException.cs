namespace PersonalData.Core
{
    using System;

    [Serializable]
    public class InitializationException : PersonalDataDBException
    {
        public InitializationException() { }

        public InitializationException(string message) : base(message) {}

        public InitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}