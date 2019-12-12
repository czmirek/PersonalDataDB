namespace PersonalData.Core
{
    public interface IUser : IResponsiblePerson
    {
        object ID { get; }
    }
}