using System.Collections.Generic;

namespace PersonalData.Core
{
    public interface IDataProvider
    {
        bool IsDatabaseInitialized();
        void Initialize();
        void InitializeSchema(ISchema schema);
        IEnumerable<IDataManager> ListDataManagers();
        object InsertDataManager(IDataManager dataManager);
        object InsertAdministrator(IAdministrator administrator);
        bool CheckAdministratorId(object administratorId);
        void SetConfiguration<T>(string key, T value) where T : struct;
        T GetConfiguration<T>(string key) where T : struct;
        void InsertAdministratorLog(IAdministratorLog newLog);
        IEnumerable<IAdministratorLog> ListAdministratorLogs();
        IEnumerable<IAdministrator> ListAdministrators();
    }
}