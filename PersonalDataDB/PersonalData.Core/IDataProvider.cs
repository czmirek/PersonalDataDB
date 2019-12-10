namespace PersonalData.Core
{
    public interface IDataProvider
    {
        bool IsDatabaseInitialized();
        void Initialize();
        void InitializeSchema(ISchema schema);
        object InsertManager(IDataManager dataManager);
        object InsertAdministrator(IResponsiblePerson administrator);
        void SetConfiguration<T>(string key, T value) where T : struct;
        T GetConfiguration<T>(string key) where T : struct;
    }
}