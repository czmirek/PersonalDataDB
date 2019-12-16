namespace PersonalData.Core
{
    using System;
    public interface IOwnerLog
    {
        object? ID { get; }
        DateTime Created { get; }
        object OwnerId { get; }
        string Text { get; }
    }
}