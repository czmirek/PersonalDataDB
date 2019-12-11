using System;
using System.Collections.Generic;

namespace PersonalData.Core
{
    public class PersonalDataDB
    {
        private readonly IDataProvider dataProvider;
        public PersonalDataDB(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public void CreateDatabase(IInitializationDataProvider initDataProvider)
        {
            bool isInitialized = dataProvider.IsDatabaseInitialized();
            
            if (isInitialized)
                throw new PersonalDataDBException("Database has been already initialized");

            Initialize(initDataProvider);
        }

        public void CreateDatabaseIfNotExist(IInitializationDataProvider initDataProvider)
        {
            bool isInitialized = dataProvider.IsDatabaseInitialized();

            if (!isInitialized)
                Initialize(initDataProvider);
        }

        public IEnumerable<IDataManager> ListDataManagers() => dataProvider.ListDataManagers();

        public object InsertDataManager(object administratorId, DataManagerInputModel newDataManager)
        {
            DataManagerValidator validator = new DataManagerValidator();
            validator.Validate(newDataManager);

            bool adminExists = dataProvider.CheckAdministratorId(administratorId);
            if (!adminExists)
                throw new PersonalDataDBException($"Administrator ID \"{administratorId.ToString()}\" does not exist");

            object newId = dataProvider.InsertDataManager(newDataManager);
            WriteAdminLog(administratorId, $"New data manager ID \"{newId.ToString()}\" was inserted by administrator ID \"{administratorId.ToString()}\".");
            return newId;
        }

        public IEnumerable<IAdministrator> ListAdministrators() => dataProvider.ListAdministrators();
        public IEnumerable<IAdministratorLog> ListAdministratorLogs() => dataProvider.ListAdministratorLogs();
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
    }    
}
