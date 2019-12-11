namespace PersonalData.Core
{
    public class DefaultInitializationDataProvider : IInitializationDataProvider
    {
        public IDataManager? DataManager { get; set; }
        public IAdministrator? Administrator { get; set; }
        public IConfiguration? Configuration { get; set; }
        public ISchema? Schema { get; set; }

    }
}
