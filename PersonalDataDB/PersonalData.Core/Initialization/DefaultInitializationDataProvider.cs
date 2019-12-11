namespace PersonalData.Core
{
    public class DefaultInitializationDataProvider : IInitializationDataProvider
    {
        public IDataManager? DataManager { get; set; }
        public IResponsiblePerson? Administrator { get; set; }
        public IConfiguration? Configuration { get; set; }
        public ISchema? Schema { get; set; }

    }
}
