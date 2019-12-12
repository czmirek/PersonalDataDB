﻿using System.Collections.Generic;

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
        void SetConfiguration<T>(string key, T value) where T : struct;
        T GetConfiguration<T>(string key) where T : struct;
        void InsertAdministratorLog(IAdministratorLog newLog);
        IEnumerable<IAdministratorLog> ListAdministratorLogs();
        IEnumerable<IAdministrator> ListAdministrators();
        void UpdateAdministrator(IAdministrator updatedAdministrator);
        IEnumerable<IUser> ListUsers();
        object InsertUser(IUser model);
        void UpdateUser(IUser model);

        bool CheckDataManagerId(object dataManagerId);
        bool CheckAdministratorId(object administratorId);
        bool CheckUserId(object userId);
    }
}