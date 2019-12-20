namespace PersonalData.Core
{
    using PersonalData.Core.Services;
    using System;
    using System.Collections.Generic;

    public class PersonalDataDB
    {
        public AdministratorService Administrator { get; private set; }
        public UserService User { get; private set; }
        public OwnerService Owner { get; private set; }
        

        private readonly IDataProvider dataProvider;
        private readonly LockProvider lockProvider;
        private object dataAccessLock => lockProvider.GetLock();
        private static PersonalDataDB? singleton = null;

        public static PersonalDataDB Create(IDataProvider dataProvider)
        {
            if (singleton == null)
                singleton = new PersonalDataDB(dataProvider);

            return singleton;
        }

        private PersonalDataDB(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            this.lockProvider = new LockProvider();

            this.Administrator = new AdministratorService(dataProvider, lockProvider);
            this.User  = new UserService(dataProvider, lockProvider);
            this.Owner = new OwnerService(dataProvider, lockProvider);
        }

        public void CreateDatabase(IInitializationDataProvider initDataProvider)
        {
            if (initDataProvider is null)
                throw new ArgumentNullException(nameof(initDataProvider));

            lock (dataAccessLock)
            {
                bool isInitialized = dataProvider.IsDatabaseInitialized();

                if (isInitialized)
                    throw new PersonalDataDBException("Database has been already initialized");

                Initialize(initDataProvider);
            }
        }
        public void CreateDatabaseIfNotExist(IInitializationDataProvider initDataProvider)
        {
            if (initDataProvider is null)
                throw new ArgumentNullException(nameof(initDataProvider));

            lock (dataAccessLock)
            {
                bool isInitialized = dataProvider.IsDatabaseInitialized();

                if (!isInitialized)
                    Initialize(initDataProvider);
            }
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