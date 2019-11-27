using System;
using System.Runtime.Serialization;

namespace PersonalDataDB
{
    [Serializable]
    internal class BuilderException : Exception
    {
        public BuilderException()
        {
        }

        public BuilderException(string? message) : base(message)
        {
        }

        public BuilderException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BuilderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}