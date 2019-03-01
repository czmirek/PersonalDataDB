namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using Dapper.Contrib.Extensions;

    public class PersonalDataAdministration
    {
        private readonly IDbConnection db;
        private readonly SysLogger logger;

        internal PersonalDataAdministration(IDbConnection db, SysLogger logger)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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

        public void AddAgreement(Agreement newAgreement)
        {
            db.Insert(newAgreement);
            
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
    }
}