namespace PersonalDataDB
{
    using System;

    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
    }
}