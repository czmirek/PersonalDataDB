namespace PersonalData.Core
{
    public interface IInitializationDataProvider
    {
        IDataManager? DataManager { get; }
        IAdministrator? Administrator { get; }
        IConfiguration? Configuration { get; }
        ISchema? Schema { get; }
    }
}
