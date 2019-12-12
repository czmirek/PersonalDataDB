namespace PersonalData.Core
{
    using System;
    using System.Collections.Generic;
    public class PersonalDataDB
    {
        private readonly IDataProvider dataProvider;
        private object taskRunner;
        private static readonly object initializationLock = new object();
        private static readonly object dataManagersLock = new object();
        private static readonly object administratorsLock = new object();
        private static readonly object usersLock = new object();
        public PersonalDataDB(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            this.taskRunner = new object();
        }
        public void CreateDatabase(IInitializationDataProvider initDataProvider)
        {
            lock (initializationLock)
            {
                bool isInitialized = dataProvider.IsDatabaseInitialized();

                if (isInitialized)
                    throw new PersonalDataDBException("Database has been already initialized");

                Initialize(initDataProvider);
            }
        }
        public void CreateDatabaseIfNotExist(IInitializationDataProvider initDataProvider)
        {
            lock (initializationLock)
            {
                bool isInitialized = dataProvider.IsDatabaseInitialized();

                if (!isInitialized)
                    Initialize(initDataProvider);
            }
        }
        public IEnumerable<IAdministrator> ListAdministrators()
        {
            lock(administratorsLock)
                return dataProvider.ListAdministrators();
        }
        public IEnumerable<IDataManager> ListDataManagers()
        {
            lock (dataManagersLock)
                return dataProvider.ListDataManagers();
        }
        public IEnumerable<IAdministratorLog> ListAdministratorLogs() => dataProvider.ListAdministratorLogs();
        public object InsertDataManager(object administratorId, DataManagerInsertModel newDataManager)
        {
            DataManagerValidator validator = new DataManagerValidator(DataManagerValidator.ValidationMode.Insert);
            validator.Validate(newDataManager);

            CheckAdministratorId(administratorId);

            object newId = new object();
            lock (dataManagersLock)
                newId = dataProvider.InsertDataManager(newDataManager);
            
            WriteAdminLog(administratorId, $"New data manager ID \"{newId.ToString()}\" was inserted by administrator ID \"{administratorId.ToString()}\".");
            return newId;
        }
        public object InsertAdministrator(object insertingAdministratorId, AdministratorInsertModel newAdministrator)
        {
            CheckAdministratorId(insertingAdministratorId);
            
            var validator = new ResponsiblePersonValidator("Administrator", ResponsiblePersonValidator.ValidationMode.Insert);
            validator.Validate(newAdministrator);
            
            object newId;
            lock (administratorsLock)
                newId = dataProvider.InsertAdministrator(newAdministrator);

            WriteAdminLog(insertingAdministratorId, $"Administrator with id \"{insertingAdministratorId.ToString()}\" created a new administrator with id \"{newId.ToString()}\"");
            
            return newId;
        }

        public IConfiguration GetConfiguration()
        {
            return new ConfigurationInternalModel()
            {
                AllowPurposeChoiceOnAgreementCreation = dataProvider.GetConfiguration<bool>(nameof(IConfiguration.AllowPurposeChoiceOnAgreementCreation))
            };
        }
        public void SetAllowPurposeChoiceOnAgreementCreation(bool newValue)
        {
            dataProvider.SetConfiguration(nameof(IConfiguration.AllowPurposeChoiceOnAgreementCreation), newValue);
        }

        public IEnumerable<IUser> ListUsers()
        {
            lock (usersLock)
                return dataProvider.ListUsers();
        }
        public object InsertUser(object administratorId, UserInsertModel model)
        {
            CheckAdministratorId(administratorId);

            object newId;
            lock (usersLock)
            {
                newId = dataProvider.InsertUser(model);
            }

            WriteAdminLog(administratorId, $"Administrator with ID {administratorId.ToString()} created a new user with ID \"{newId.ToString()}\".");
            return newId;
        }

        public void UpdateUser(object administratorId, UserUpdateModel model)
        {
            CheckAdministratorId(administratorId);
            CheckUserId(model.ID);

            lock(usersLock)
                dataProvider.UpdateUser(model);

            WriteAdminLog(administratorId, $"Administrator with ID {administratorId.ToString()} updated an existing user with ID \"{model.ID.ToString()}\".");
        }

        public void UpdateAdministrator(object updatingAdministratorId, AdministratorUpdateModel updatedAdministrator)
        {
            CheckAdministratorId(updatingAdministratorId);

            var validator = new ResponsiblePersonValidator("Administrator", ResponsiblePersonValidator.ValidationMode.Update);
            validator.Validate(updatedAdministrator);

            lock(administratorsLock)
                dataProvider.UpdateAdministrator(updatedAdministrator);
        }

        public object InsertOwnerByAdministrator(object administratorId) {
            throw new NotImplementedException();
        }
        public object InsertOwnerByUser(object userId) {
            throw new NotImplementedException();
        }
        public object InsertOwnerByOwner() {
            throw new NotImplementedException();
        }
        private void WriteAdminLog(object administratorId, string text)
        {
            dataProvider.InsertAdministratorLog(new AdministratorLogInternalModel(DateTime.Now, administratorId, text));
        }
        private void Initialize(IInitializationDataProvider initDataProvider)
        {
            InitializationValidator initializationValidator = new InitializationValidator();
            initializationValidator.Validate(initDataProvider);

            dataProvider.Initialize();
            dataProvider.InitializeSchema(initDataProvider.Schema!);
            dataProvider.InsertDataManager(initDataProvider.DataManager!);
            dataProvider.InsertAdministrator(initDataProvider.Administrator!);
            dataProvider.SetConfiguration(nameof(IConfiguration.AllowPurposeChoiceOnAgreementCreation), 
                                          initDataProvider.Configuration!.AllowPurposeChoiceOnAgreementCreation);
        }
        private void CheckAdministratorId(object administratorId)
        {
            lock (administratorsLock)
            {
                if (!dataProvider.CheckAdministratorId(administratorId))
                    throw new PersonalDataDBException($"Administrator ID \"{administratorId.ToString()}\" does not exist");
            }
        }

        private void CheckUserId(object userId)
        {
            lock (usersLock)
            {
                if (!dataProvider.CheckUserId(userId))
                    throw new PersonalDataDBException($"User ID \"{userId.ToString()}\" does not exist");
            }
        }

        /*
public void UpdateDataManager(object administratorId, DataManagerUpdateModel updatedDataManager)
{
    CheckAdministratorId(administratorId);
    CheckDataManagerId(updatedDataManager.ID);
    //todo: Správce lze změnit pouze pokud neexistuje šablona souhlasu ani souhlas, ke kterému je daný správce připojen
}
public void DeleteDataManager(object administratorId, object dataManagerId)
{
    CheckAdministratorId(administratorId);
    CheckDataManagerId(dataManagerId);
    //todo: správce lze smazat pouze pokud neexistuje šablona souhlasu ani souhlas, ke kterému je daný správce připojen
}

private void CheckDataManagerId(object dataManagerId)
{
    lock (dataManagersLock)
    {
        if (!dataProvider.CheckDataManagerId(dataManagerId))
            throw new PersonalDataDBException($"Data Manager ID \"{dataManagerId.ToString()}\" does not exist");
    }
}
*/
    }
}