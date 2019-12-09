namespace PersonalData.Core
{
    public class InitializationDataBuilder : IInitializationDataProvider
    {
        public IDataManager? DataManager { get; set; }
        public IResponsiblePerson? Administrator { get; set; }
    }
}
