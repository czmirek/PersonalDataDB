namespace PersonalData.Core
{
    public interface IInitializationDataProvider
    {
        IDataManager? DataManager { get; }
        IResponsiblePerson? Administrator { get; }
        IConfiguration? Configuration { get; }
        ISchema? Schema { get; }
    }
}
