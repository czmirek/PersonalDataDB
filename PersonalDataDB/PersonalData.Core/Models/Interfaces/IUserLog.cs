namespace PersonalData.Core
{
    using System;
    public interface IUserLog
    {
        object? ID { get; }
        DateTime Created { get; }
        object UserId { get; }
        string Text { get; }
    }
}