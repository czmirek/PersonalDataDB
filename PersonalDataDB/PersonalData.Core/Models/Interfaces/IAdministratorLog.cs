namespace PersonalData.Core
{
    using System;
    public interface IAdministratorLog
    {
        object? ID { get; }
        DateTime Created { get; }
        object AdministratorId { get; }
        string Text { get; }
    }
}
