namespace PersonalDataDB
{
    using AutoMapper;
    using PersonalDataDB.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PersonalDataAdministration
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        internal PersonalDataAdministration(IRepository repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException();
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<Language> GetLanguages()
        {
            FullRepositoryCheck();

            IEnumerable<LanguageRepositoryModel> languages = repository.GetLanguages();
            return languages.Select(l => mapper.Map<LanguageRepositoryModel, Language>(l));
        }

        public void AddLanguage(Language newLanguage)
        {
            if (newLanguage.ID != 0)
                throw new InvalidOperationException($"ID must be zero when adding a new language.");

            if(String.IsNullOrEmpty(newLanguage.LanguageCode))
                throw new InvalidOperationException($"Language code must be set.");

            FullRepositoryCheck();

            IEnumerable<LanguageRepositoryModel> languages = repository.GetLanguages();

            if (languages.Any(l => l.LanguageCode.Equals(newLanguage.LanguageCode, StringComparison.InvariantCultureIgnoreCase)))
                throw new InvalidOperationException($"Language with language code {newLanguage.LanguageCode} already exists");

            var languageRepositoryModel = mapper.Map<Language, LanguageRepositoryModel>(newLanguage);
            repository.AddLanguage(languageRepositoryModel);

            Syslog($"Administrator added a new language \"{newLanguage.LanguageCode}\" (ID: {languageRepositoryModel.ID}).");

            newLanguage.ID = languageRepositoryModel.ID;
        }

        public Configuration GetConfiguration()
        {
            InitializationCheck();

            ConfigurationRepositoryModel dbConfig = repository.GetConfiguration();
            return mapper.Map<ConfigurationRepositoryModel, Configuration>(dbConfig);
        }

        public void SetConfiguration(Configuration configuration)
        {
            InitializationCheck();

            if (String.IsNullOrWhiteSpace(configuration.PersonalDataResponsibleContact))
                throw new InvalidOperationException("Personal data responsible contact must not be empty.");

            var dbModel = mapper.Map<Configuration, ConfigurationRepositoryModel>(configuration);
            repository.SaveConfiguration(dbModel);
        }

        public IEnumerable<Agreement> GetAgreements()
        {
            FullRepositoryCheck();
            IEnumerable<AgreementRepositoryModel> agreements = repository.GetAgreements();
            return mapper.Map<IEnumerable<Agreement>>(agreements);
        }

        public void AddAgreement(Agreement newAgreement)
        {
            FullRepositoryCheck();

            IEnumerable<LanguageRepositoryModel> languages = repository.GetLanguages();

            if (!languages.Any())
                throw new InvalidOperationException("An agreement must contain at least one text for all languages in the database but there are no languages specified in the database.");

            if (newAgreement.ID != 0)
                throw new InvalidOperationException($"ID must be zero when adding a new agreement.");

            if (newAgreement.LocalizedText == null || !newAgreement.LocalizedText.Any())
                throw new InvalidOperationException("An agreement must contain at least one text for all languages");

            foreach (LanguageRepositoryModel lang in languages)
            {
                if (newAgreement.LocalizedText.Any(l => l.LanguageID == lang.ID))
                    throw new InvalidOperationException($"Please specify at least a single text for language in the new agreement \"{lang.LanguageCode}\" (ID: {lang.ID}).");
            }

            int requiredProofs = repository.GetConfiguration<int>(nameof(Configuration.NumberOfRequiredAgreementProofs));

            if (requiredProofs > 0 && newAgreement.Proofs.Count() < requiredProofs)
                throw new InvalidOperationException($"At least {requiredProofs} proofs are required in the agreement.");

            AgreementRepositoryModel dbModel = mapper.Map<Agreement, AgreementRepositoryModel>(newAgreement);
            repository.AddAgreement(dbModel);

            if (newAgreement.Scope.OwnerID == null)
                Syslog($"New global agreement was added (ID: {dbModel.ID}).");
            else
                Syslog($"New owner specific agreement was added (ID: {dbModel.ID}).");
        }

        public void RemoveAgreement(int agreementID)
        {
            FullRepositoryCheck();

            IEnumerable<AgreementRepositoryModel> agreements = repository.GetAgreements();

            if (!agreements.Any(a => a.ID == agreementID))
                throw new InvalidOperationException($"Agreement with ID {agreementID} does not exist.");

            repository.RemoveAgreement(agreementID);

            Syslog($"Agreement ID {agreementID} was removed.");
        }


        public void AddAgreementProof(int agreementID, AgreementProof newProof)
        {
            FullRepositoryCheck();

            IEnumerable<AgreementRepositoryModel> agreements = repository.GetAgreements();

            if (!agreements.Any(a => a.ID == agreementID))
                throw new InvalidOperationException($"Agreement with ID {agreementID} does not exist.");

            AgreementProofRepositoryModel dbModel = mapper.Map<AgreementProofRepositoryModel>(newProof);
            dbModel.AgreementID = agreementID;
            repository.AddAgreementProof(dbModel);
            newProof.ID = dbModel.ID;

            Syslog($"New agreement proof was added (ID:{newProof.ID})");
        }

        public void RemoveAgreementProof(int agreementProofID)
        {
            FullRepositoryCheck();

            IEnumerable<AgreementProofRepositoryModel> proofs = repository.GetAgreementProofs();
            var dbProof = proofs.FirstOrDefault(a => a.ID == agreementProofID);

            if (dbProof == null)
                throw new InvalidOperationException($"Agreement proof with ID {agreementProofID} does not exist.");

            int proofCountInAgreement = repository.GetAgreementProofs().Where(p => p.AgreementID == dbProof.AgreementID).Count();
            int requiredProofs = repository.GetConfiguration<int>(nameof(ConfigurationRepositoryModel.NumberOfRequiredAgreementProofs));

            if(requiredProofs <= proofCountInAgreement)
                throw new InvalidOperationException("Unable to remove agreement. At least ")

            repository.RemoveAgreementProof(agreementProofID);
        }
        /*

        #region Metadata read
        public IEnumerable<SystemLog> GetSystemLogs() => db.GetAll<SystemLog>();
        public IEnumerable<Agreement> GetAgreements() => db.GetAll<Agreement>();
        public IEnumerable<AgreementProof> GetAgreementProofs() => db.GetAll<AgreementProof>();
        public IEnumerable<AgreementText> GetAgreementTexts() => db.GetAll<AgreementText>();
        public IEnumerable<Language> GetLanguages() => db.GetAll<Language>();
        public IEnumerable<LimitingReason> GetLimitingReasons() => db.GetAll<LimitingReason>();
        public IEnumerable<LimitingReasonText> GetLimitingReasonTexts() => db.GetAll<LimitingReasonText>();
        public IEnumerable<Owner> GetOwners() => db.GetAll<Owner>();
        public IEnumerable<PersonalDataLog> GetPersonalDataLogs() => db.GetAll<PersonalDataLog>();
        public IEnumerable<PersonalDataReference> GetPersonalDataReferences() => db.GetAll<PersonalDataReference>();
        public IEnumerable<Purpose> GetPurposes() => db.GetAll<Purpose>();
        public IEnumerable<PurposeAnswer> GetPurposeAnswers() => db.GetAll<PurposeAnswer>();
        public IEnumerable<PurposeQuestion> GetPurposeQuestions() => db.GetAll<PurposeQuestion>();
        public IEnumerable<WorkUser> GetWorkUsers() => db.GetAll<WorkUser>();
        public IEnumerable<WorkUserText> GetWorkUserTexts() => db.GetAll<WorkUserText>();

        public Agreement GetAgreement(int id) => db.Get<Agreement>(id);
        public AgreementProof GetAgreementProof(int id) => db.Get<AgreementProof>(id);
        public AgreementText GetAgreementText(int id) => db.Get<AgreementText>(id);
        public Language GetLanguage(int id) => db.Get<Language>(id);
        public LimitingReason GetLimitingReason(int id) => db.Get<LimitingReason>(id);
        public LimitingReasonText GetLimitingReasonText(int id) => db.Get<LimitingReasonText>(id);
        public Owner GetOwner(int id) => db.Get<Owner>(id);
        public PersonalDataLog GetPersonalDataLog(int id) => db.Get<PersonalDataLog>(id);
        public PersonalDataReference GetPersonalDataReference(int id) => db.Get<PersonalDataReference>(id);
        public Purpose GetPurpose(int id) => db.Get<Purpose>(id);
        public PurposeAnswer GetPurposeAnswer(int id) => db.Get<PurposeAnswer>(id);
        public PurposeQuestion GetPurposeQuestion(int id) => db.Get<PurposeQuestion>(id);
        public WorkUser GetWorkUser(int id) => db.Get<WorkUser>(id);
        public WorkUserText GetWorkUserText(int id) => db.Get<WorkUserText>(id);
        #endregion
        */
        public void AddAgreement(AgreementRepositoryModel newAgreement)
        {
            //db.Insert(newAgreement);
            
        }
        /*public void AddAgreementProof(int id) => db.Add<AgreementProof>(id);
        public void AddAgreementText(int id) => db.Add<AgreementText>(id);
        public void AddLanguage(int id) => db.Add<Language>(id);
        public void AddLimitingReason(int id) => db.Add<LimitingReason>(id);
        public void AddLimitingReasonText(int id) => db.Add<LimitingReasonText>(id);
        public void AddOwner(int id) => db.Add<Owner>(id);
        public void AddPersonalDataLog(int id) => db.Add<PersonalDataLog>(id);
        public void AddPersonalDataReference(int id) => db.Add<PersonalDataReference>(id);
        public void AddPurpose(int id) => db.Add<Purpose>(id);
        public void AddPurposeAnswer(int id) => db.Add<PurposeAnswer>(id);
        public void AddPurposeQuestion(int id) => db.Add<PurposeQuestion>(id);
        public void AddWorkUser(int id) => db.Add<WorkUser>(id);
        public void AddWorkUserText(int id) => db.Add<WorkUserText>(id);*/

        void CreatePersonalDataColumn(string columnName)
        {
            
        }

        void CreatePersonalDataColumn(string tableName, string columnName) { }

        void RemovePersonalDataColumn(string columnName) { }
        void RemovePersonalDataColumn(string tableName, string columnName) { }

        void RenamePersonalDataColumn(string oldColumnName, string newColumnName) { }
        void RenamePersonalDataColumn(string tableName, string oldColumnName, string newColumnName) { }

        void CreatePersonalDataTable(string tableName) { }
        void RenamePersonalDataTable(string oldTableName, string newTableName) { }
        void RemovePersonalDataTable(string tableName) { }

        // odstraní expirovaná data a data bez účelu nebo s expirovaným účelem
        // cronová operace
        void RemoveInvalidData() { }

        private Scope CreateScopeFromDbValues(int? ownerID, int? rowID, string table, string column)
        {
            if (ownerID == null)
                return Scope.GlobalScope;

            if (rowID == null && table == null && column == null)
                return Scope.CreateOwnerScope(ownerID.Value);

            if (rowID == null && table == null && column != null)
                return Scope.CreateColumnScope(ownerID.Value, column);

            if (rowID == null && table != null && column != null)
                return Scope.CreateColumnScope(ownerID.Value, table, column);

            if (rowID != null && table != null && column != null)
                return Scope.CreateCellScope(ownerID.Value, table, column, rowID.Value);

            throw new InvalidOperationException("Invalid scope data");
        }

        private void InitializationCheck()
        {
            bool repositoryInitialized = repository.IsRepositoryInitialized();

            if (!repositoryInitialized)
                throw new InvalidOperationException("The personal data repository has not been initialized yet.");
        }

        public IDateTimeProvider DateTimeProvider { get; private set; }

        private void FullRepositoryCheck()
        {
            InitializationCheck();

            bool configIsSet = repository.GetConfiguration<bool>(nameof(ConfigurationRepositoryModel.ConfigurationIsSet));

            if (!configIsSet)
                throw new InvalidOperationException("The personal data repository has not yet been configured. Please configure the database by calling SetConfiguration.");
        }
        private void Syslog(string logText)
        {
            repository.AddSystemLog(new SystemLogRepositoryModel()
            {
                ID = 0,
                Log = logText,
                Time = DateTimeProvider.Now
            });
        }
    }
}