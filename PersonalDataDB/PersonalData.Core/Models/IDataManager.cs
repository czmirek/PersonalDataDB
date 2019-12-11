namespace PersonalData.Core
{
    using System;
    public interface IDataManager
    {
        object? ID { get; }
        string Name { get; }
        string Address { get; }
        string Email { get; }
        string Phone { get; }
        string? RegistrationNumber { get; }
        string? PersonalDataRegistrationNumber { get; }
    }
}
