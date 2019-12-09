using System;

namespace PersonalData.Core
{
    public interface IDataManager
    {
        string Name { get; }
        string Address { get; }
        string Email { get; }
        string Phone { get; }
        string? RegistrationNumber { get; }
        string? PersonalDataRegistrationNumber { get; }
    }
}
