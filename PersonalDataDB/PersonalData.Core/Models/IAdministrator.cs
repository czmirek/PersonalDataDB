namespace PersonalData.Core
{
    public interface IAdministrator : IResponsiblePerson
    {
        object? ID { get; }
    }
}