namespace PersonalDataDB
{
    using System;

    public interface IPersonalDataValue
    {
        bool IsSet { get; }
        object Value { get; }
        DateTimeOffset? ExpirationDate { get; }
    }
}