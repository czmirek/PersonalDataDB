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

        private void Initialize(IInitializationDataProvider initDataProvider)
        {
            InitializationValidator initializationValidator = new InitializationValidator();
            initializationValidator.Validate(initDataProvider);

            dataProvider.Initialize();
            dataProvider.InitializeSchema(initDataProvider.Schema!);
            dataProvider.InsertManager(initDataProvider.DataManager!);
            dataProvider.InsertAdministrator(initDataProvider.Administrator!);
            dataProvider.SetConfiguration(nameof(IConfiguration.AllowPurposeChoiceOnAgreementCreation), 
                                          initDataProvider.Configuration!.AllowPurposeChoiceOnAgreementCreation);
        }
    }    
}
