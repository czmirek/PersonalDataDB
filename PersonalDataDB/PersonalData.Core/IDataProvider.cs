namespace PersonalData.Core
{
    public interface IDataProvider
    {
        bool IsDatabaseInitialized();
        void Initialize();
        object InsertManager(IDataManager dataManager);
        object InsertAdministrator(IResponsiblePerson administrator);
    }
}