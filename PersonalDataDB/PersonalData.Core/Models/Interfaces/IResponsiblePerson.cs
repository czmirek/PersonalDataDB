namespace PersonalData.Core
{
    public interface IResponsiblePerson
    {
        string FullName { get; }
        string Phone { get; }
        string Email { get; }
        string? OfficeNumber { get; }
        string? InternalPhoneNumber { get; }
        string? Supervisor { get; }
    }
}
