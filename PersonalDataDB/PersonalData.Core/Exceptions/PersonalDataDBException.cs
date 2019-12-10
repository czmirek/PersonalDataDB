namespace PersonalData.Core
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PersonalDataDBException : Exception
    {
        public PersonalDataDBException()
        {
        }

        public PersonalDataDBException(string message) : base(message)
        {
        }

        public PersonalDataDBException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PersonalDataDBException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}