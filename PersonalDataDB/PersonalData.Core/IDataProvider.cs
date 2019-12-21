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
        object InsertOwner();
        void InsertUserLog(IUserLog log);
        void InsertOwnerLog(IOwnerLog log);
        IEnumerable<IPurpose> ListPurposes();
        object InsertPurpose(IPurpose model);
        IEnumerable<IAgreementTemplate> ListAgreementTemplates();
        IEnumerable<IAgreement> ListAgreements();
        IEnumerable<IAgreement> ListAgreementsForOwner(object ownerId);
        void UpdateDataManager(IDataManager updatedDataManager);
        void DeleteDataManager(object removedDataManagerId);
        bool CheckOwnerId(object ownerId);

        /// <summary>
        /// Returns true if there in any single cell with an existing personal data.
        /// Non-existing personal data cells are excluded.
        /// </summary>
        /// <param name="ownerId">ID of the personal data owner</param>
        bool PersonalDataExistsForOwner(object ownerId);
        void DeleteOwner(object ownerId);


        /// <summary>
        /// Lists all owner restrictions
        /// </summary>
        /// <returns></returns>
        IEnumerable<IOwnerRestriction> ListOwnerRestrictions();
        /// <summary>
        /// Lists owner restrictions for specific owner
        /// </summary>
        /// <param name="ownerId">ID of the owner</param>
        IEnumerable<IOwnerRestriction> ListOwnerRestrictions(object ownerId);
        void ClearAllPersonalDataForOwner(object ownerId);
        void ClearOwnerLog(object ownerId);
        bool CheckPurposeId(object purposeId);
        IEnumerable<IAgreement> ListAgreementsByPurpose(object purposeId);
        void UpdatePurpose(IPurpose model);
        void DeletePurpose(object purposeId);
        object InsertAgreementTemplate(IAgreementTemplate agreementTemplate);
        void UpdateAgreementTemplate(IAgreementTemplate agreementTemplate);
        void DeleteAgreementTemplate(object agreementTemplateId);
        object InsertOwnerRestriction(IOwnerRestriction model);
        bool CheckOwnerRestrictionId(object ownerRestrictionId);
        void DeleteOwnerRestriction(object ownerRestrictionId);
        IOwnerRestriction GetOwnerRestriction(object ownerRestrictionId);
        IEnumerable<IOwnerRestrictionExplanation> ListOwnerRestrictionExplanations();
        object InsertOwnerRestrictionExplanation(OwnerRestrictionExplanationInsertModel model);
        bool CheckOwnerRestrictionExplanationId(object ownerRestrictionExplanationId);
        IOwnerRestrictionExplanation GetOwnerRestrictionExplanation(object ownerRestrictionExplanationId);
        void DeleteOwnerRestrictionExplanation(object ownerRestrictionExplanationId);
        IEnumerable<ITableDefinition> ListTables();
        bool CheckTableId(string tableId);
        ITableDefinition GetTable(string tableID);
        void InsertTable(ITableDefinition model);
        void UpdateTable(ITableDefinition model);
        bool CheckTableContainsExistingPersonalData(string tableID);
        void DeleteTable(string tableID);
        IEnumerable<IColumnDefinition> ListColumns(string tableId);
        void InsertColumn(string tableId, IColumnDefinition model);
        bool CheckColumnId(string tableId, string columnId);
        void UpdateColumn(string tableId, IColumnDefinition model);
        bool CheckColumnContainsExistingPersonalData(string tableId, string columnId);
        void DeleteColumn(string tableId, string columnId);
        bool CheckRowId(string tableId, object rowId);
        object GetOwnerForRowId(string tableId, object rowId);
        IPersonalDataCell ReadPersonalDataCell(string tableId, string columnId, object rowId);
        IColumnDefinition GetColumn(string tableId, string columnId);
        IEnumerable<IPersonalDataCell> ReadPersonalDataCells(string tableId, object rowId, IEnumerable<string> columnIDs);
        IPersonalDataRow ReadPersonalDataRow(string tableId, object rowId);
        IEnumerable<object> GetRowIdsForOwnerId(string tableId, object ownerId);
        IEnumerable<IPersonalDataRow> ReadPersonalDataRowsByOwner(string tableId, object ownerId);
        IEnumerable<IPersonalDataTable> ReadAllPersonalDataByOwner(object ownerId);
        object InsertPersonalDataRow(string tableId, object ownerId, IDictionary<string, object> data);
        void UpdatePersonalDataRow(object ownerId, string tableId, object rowId, IDictionary<string, object> data);
    }
}