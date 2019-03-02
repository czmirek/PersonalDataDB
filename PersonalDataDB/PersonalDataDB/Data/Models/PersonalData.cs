namespace PersonalDataDB
{
    using System;

    public abstract class PersonalData
    {
        public int OwnerID { get; }
        public bool IsSet { get; }
        public string TableName { get; }
        public string ColumnName { get; }
        public object Value { get; }
    }

    public abstract class PersonalData<T> : PersonalData
    {
        public new T Value { get; }
    }

    public class PersonalDataByteArray : PersonalData<byte[]> { }
    public class PersonalDataInteger : PersonalData<int> { }
    public class PersonalDataDateTime : PersonalData<DateTime> { }
    public class PersonalDataDateTimeOffset : PersonalData<DateTimeOffset> { }
    public class PersonalDataDouble : PersonalData<double> { }
    public class PersonalDataString : PersonalData<string> { }
}