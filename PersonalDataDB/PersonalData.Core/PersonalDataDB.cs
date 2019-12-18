namespace PersonalData.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class PersonalDataDB
    {
        private readonly IDataProvider dataProvider;
        
        private static readonly object dataAccessLock = new object();

        /// <summary>
        /// Lock for locking access to personal data of a specific owner.
        /// </summary>

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
        public IEnumerable<IAdministrator> ListAdministrators()
        {
            return dataProvider.ListAdministrators();
        }
        public IEnumerable<IDataManager> ListDataManagers()
        {
            return dataProvider.ListDataManagers();
        }

        public IEnumerable<IPurpose> ListPurposes()
        {
            return dataProvider.ListPurposes();
        }

        public object InsertPurpose(object administratorId, PurposeInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));
            
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);

                object newId = dataProvider.InsertPurpose(model);
                WriteAdminLog(administratorId, $"New purpose with ID {model.ID.ToString()} was created.");
                return newId;
            }
        }

        public void UpdatePurpose(object administratorId, PurposeUpdateModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));
            
            if (model is null)
                throw new ArgumentNullException(nameof(model));
            
            lock(dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckPurposeId(model.ID);

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsByPurpose(model.ID);
                if (agreements.Any())
                    throw new PersonalDataDBException($"Unable to update purpose ID \"{model.ID}\" because there are agreements referencing it.");

                dataProvider.UpdatePurpose(model);
            }
        }

        public void DeletePurpose(object administratorId, object purposeId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (purposeId is null)
                throw new ArgumentNullException(nameof(purposeId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckPurposeId(purposeId);

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsByPurpose(purposeId);
                if (agreements.Any())
                    throw new PersonalDataDBException($"Unable to delete purpose ID \"{purposeId.ToString()}\" because there are agreements referencing it.");

                dataProvider.DeletePurpose(purposeId);
            }
        }

        


        public IEnumerable<IAdministratorLog> ListAdministratorLogs() => dataProvider.ListAdministratorLogs();
        public object InsertDataManager(object administratorId, DataManagerInsertModel newDataManager)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (newDataManager is null)
                throw new ArgumentNullException(nameof(newDataManager));

            DataManagerValidator validator = new DataManagerValidator(DataManagerValidator.ValidationMode.Insert);
            validator.Validate(newDataManager);

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                
                object newId = new object();
                newId = dataProvider.InsertDataManager(newDataManager);

                WriteAdminLog(administratorId, $"New data manager ID \"{newId.ToString()}\" was created.");
                return newId;
            }
        }

        public object InsertAdministrator(object insertingAdministratorId, AdministratorInsertModel newAdministrator)
        {
            if (insertingAdministratorId is null)
                throw new ArgumentNullException(nameof(insertingAdministratorId));
            
            if (newAdministrator is null)
                throw new ArgumentNullException(nameof(newAdministrator));

            lock (dataAccessLock)
            {
                CheckAdministratorId(insertingAdministratorId);

                var validator = new ResponsiblePersonValidator("Administrator", ResponsiblePersonValidator.ValidationMode.Insert);
                validator.Validate(newAdministrator);

                object newId;
                newId = dataProvider.InsertAdministrator(newAdministrator);

                WriteAdminLog(insertingAdministratorId, $"New administrator with ID \"{newId.ToString()}\" was created");

                return newId;
            }
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
            return dataProvider.ListUsers();
        }
        public object InsertUser(object administratorId, UserInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);

                object newId;
                newId = dataProvider.InsertUser(model);

                WriteAdminLog(administratorId, $"New user with ID \"{newId.ToString()}\" was created.");
                return newId;
            }
        }

        public void UpdateUser(object administratorId, UserUpdateModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckUserId(model.ID);

                dataProvider.UpdateUser(model);

                WriteAdminLog(administratorId, $"User with ID \"{model.ID.ToString()}\" was updated.");
            }
        }

        public void UpdateAdministrator(object updatingAdministratorId, AdministratorUpdateModel updatedAdministrator)
        {
            if (updatingAdministratorId is null)
                throw new ArgumentNullException(nameof(updatingAdministratorId));

            if (updatedAdministrator is null)
                throw new ArgumentNullException(nameof(updatedAdministrator));

            lock (dataAccessLock)
            {
                CheckAdministratorId(updatingAdministratorId);

                var validator = new ResponsiblePersonValidator("Administrator", ResponsiblePersonValidator.ValidationMode.Update);
                validator.Validate(updatedAdministrator);

                dataProvider.UpdateAdministrator(updatedAdministrator);
            }
        }

        public object InsertOwnerByAdministrator(object administratorId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                object newId = dataProvider.InsertOwner();
                WriteAdminLog(administratorId, $"New owner with ID {newId.ToString()} was created");
                return newId;
            }
        }
        public object InsertOwnerByUser(object userId) 
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                object newId = dataProvider.InsertOwner();
                WriteUserLog(userId, $"New owner with ID {newId.ToString()} was created");
                return newId;
            }
        }

        public object InsertOwnerByOwner()
        {
            object newId = dataProvider.InsertOwner();
            WriteOwnerLog(newId, $"New owner was created on his own volition");
            return newId;
        }

        public void DeleteOwnerByAdministrator(object administratorId, object ownerId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock (dataAccessLock)
            {

                CheckAdministratorId(administratorId);
                CheckOwnerId(ownerId);

                if (dataProvider.PersonalDataExistsForOwner(ownerId))
                    throw new PersonalDataDBException("Unable to delete owner, there are still some personal data attached to it.");

                dataProvider.DeleteOwner(ownerId);
                WriteAdminLog(administratorId, $"Owner with ID {ownerId.ToString()} has been deleted");
            }
        }

        public void DeleteOwnerByUser(object userId, object ownerId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckOwnerId(ownerId);

                if (dataProvider.PersonalDataExistsForOwner(ownerId))
                    throw new PersonalDataDBException("Unable to delete owner, there are still some personal data attached to it.");

                dataProvider.DeleteOwner(ownerId);
                WriteUserLog(userId, $"Owner with ID {ownerId.ToString()} has been deleted");
            }
        }

        public void DeleteOwnerByOwner(object ownerId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            
            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);

                IEnumerable<IOwnerRestriction> ownerRestrictions = dataProvider.ListOwnerRestrictions(ownerId);

                if (ownerRestrictions.Any())
                    throw new PersonalDataDBException("Unable to remove owner on his own volition because there are owner restrictions attached.");

                dataProvider.ClearAllPersonalDataForOwner(ownerId);
                dataProvider.DeleteOwner(ownerId);
                WriteOwnerLog(ownerId, $"Owner was deleted on his own volition");   
            }
        }

        public void UpdateDataManager(object administratorId, DataManagerUpdateModel updatedDataManager)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (updatedDataManager is null)
                throw new ArgumentNullException(nameof(updatedDataManager));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckDataManagerId(updatedDataManager.ID);

                IEnumerable<IAgreementTemplate> agreementTemplates = dataProvider.ListAgreementTemplates();
                if (agreementTemplates.Any(at => at.DataManagerID.Equals(updatedDataManager.ID)))
                    throw new PersonalDataDBException($"Data Manager ID {updatedDataManager.ID.ToString()} cannot be updated because there are referencing agreement templates.");

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreements();
                if (agreements.Any(a => a.DataManagerID.Equals(updatedDataManager.ID)))
                    throw new PersonalDataDBException($"Data Manager ID {updatedDataManager.ID.ToString()} cannot be updated because there are referencing agreements.");

                dataProvider.UpdateDataManager(updatedDataManager);
                WriteAdminLog(administratorId, $"Data manager with ID {updatedDataManager.ID.ToString()} was updated");
            }
        }

        public void DeleteDataManager(object administratorId, object removedDataManagerId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (removedDataManagerId is null)
                throw new ArgumentNullException(nameof(removedDataManagerId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckDataManagerId(removedDataManagerId);

                IEnumerable<IAgreementTemplate> agreementTemplates = dataProvider.ListAgreementTemplates();
                if (agreementTemplates.Any(at => at.DataManagerID.Equals(removedDataManagerId)))
                    throw new PersonalDataDBException($"Data Manager ID {removedDataManagerId.ToString()} cannot be removed because there are referencing agreement templates.");

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreements();
                if (agreements.Any(a => a.DataManagerID.Equals(removedDataManagerId)))
                    throw new PersonalDataDBException($"Data Manager ID {removedDataManagerId.ToString()} cannot be removed because there are referencing agreements.");

                dataProvider.DeleteDataManager(removedDataManagerId);
                WriteAdminLog(administratorId, $"Data manager with ID {removedDataManagerId.ToString()} was deleted");
            }
        }

        public IEnumerable<IAgreementTemplate> ListAgreementTemplates()
        {
            return dataProvider.ListAgreementTemplates();
        }

        public object InsertAgreementTemplate(object administratorId, AgreementTemplateInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if (!model.PurposeIDs.Any())
                throw new PersonalDataDBException("Agreement template must be connected to at least one purpose");

            object newId = new object();

            lock (dataAccessLock)
            {
                foreach (object purposeId in model.PurposeIDs)
                    CheckPurposeId(purposeId);

                CheckAdministratorId(administratorId);

                newId = dataProvider.InsertAgreementTemplate(model);
                WriteAdminLog(administratorId, $"New agreement template with ID {newId.ToString()} was created");
            }

            return newId;
        }

        public void UpdateAgreementTemplate(object administratorId, AgreementTemplateUpdateModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            if(!model.PurposeIDs.Any())
                throw new PersonalDataDBException("Agreement template must be connected to at least one purpose");

            lock (dataAccessLock)
            {
                foreach (object purposeId in model.PurposeIDs)
                    CheckPurposeId(purposeId);

                CheckAdministratorId(administratorId);

                dataProvider.UpdateAgreementTemplate(model);
                WriteAdminLog(administratorId, $"Agreement template with ID {model.ID.ToString()} was updated");
            }
        }

        public void DeleteAgreementTemplate(object administratorId, object agreementTemplateId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (agreementTemplateId is null)
                throw new ArgumentNullException(nameof(agreementTemplateId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckAgreementTemplateId(agreementTemplateId);
                dataProvider.DeleteAgreementTemplate(agreementTemplateId);
                WriteAdminLog(administratorId, $"Agreement template with ID {agreementTemplateId.ToString()} has been deleted");
            }
        }

        public IEnumerable<IOwnerRestriction> ListOwnerRestrictions()
        {
            return dataProvider.ListOwnerRestrictions();
        }


        public IEnumerable<IOwnerRestriction> ListOwnerRestrictions(object ownerId)
        {
            return dataProvider.ListOwnerRestrictions(ownerId);
        }

        public object InsertOwnerRestriction(object userId, OwnerRestrictionInsertModel model)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            object newId = new object();
            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckOwnerId(model.OwnerID);

                if (model.OwnerRestrictionExplanationID == null && model.CustomExplanation == null)
                    throw new ArgumentException("Either owner restriction or custom explanation must be not null when creating a new owner restriction.");

                if (model.OwnerRestrictionExplanationID != null && model.CustomExplanation != null)
                    throw new ArgumentException("Either owner restriction or custom explanation must be chosen when creating a new owner restriction.");

                newId = dataProvider.InsertOwnerRestriction(model);

                WriteUserLog(userId, $"New owner restriction ID {newId.ToString()} was created");
                WriteOwnerLog(model.OwnerID, $"New owner restriction ID {newId.ToString()} was created");
            }

            return newId;
        }

        public void DeleteOwnerRestriction(object userId, object ownerRestrictionId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerRestrictionId is null)
                throw new ArgumentNullException(nameof(ownerRestrictionId));

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckOwnerRestrictionId(ownerRestrictionId);
                
                IOwnerRestriction ownerRestriction = dataProvider.GetOwnerRestriction(ownerRestrictionId);

                dataProvider.DeleteOwnerRestriction(ownerRestrictionId);
                
                WriteUserLog(userId, $"Owner restriction ID {ownerRestrictionId.ToString()} was deleted");
                WriteOwnerLog(ownerRestriction.OwnerID, $"Owner restriction ID {ownerRestrictionId.ToString()} was deleted");
            }
        }

        public IEnumerable<IOwnerRestrictionExplanation> ListOwnerRestrictionExplanations()
        {
            return dataProvider.ListOwnerRestrictionExplanations();
        }

        public object CreateOwnerRestrictionExplanation(object administratorId, OwnerRestrictionExplanationInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            object newId = new object();
            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                newId = dataProvider.InsertOwnerRestrictionExplanation(model);
                WriteAdminLog(administratorId, $"New owner restriction with ID {newId.ToString()} has been created");
            }
            return newId;
        }

        public void DeleteOwnerRestrictionExplanation(object administratorId, object ownerRestrictionExplanationId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (ownerRestrictionExplanationId is null)
                throw new ArgumentNullException(nameof(ownerRestrictionExplanationId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckOwnerRestrictionExplanationId(ownerRestrictionExplanationId);
                
                IEnumerable<IOwnerRestriction> restrictions = dataProvider.ListOwnerRestrictions();
                if (restrictions.Any(r => r.OwnerRestrictionExplanationID !=null && r.OwnerRestrictionExplanationID.Equals(ownerRestrictionExplanationId)))
                    throw new PersonalDataDBException("Unable to delete owner restriction explanation, there are some owner restrictions referencing it");
                
                dataProvider.DeleteOwnerRestrictionExplanation(ownerRestrictionExplanationId);
                WriteAdminLog(administratorId, $"Owner restriction explanation with ID {ownerRestrictionExplanationId.ToString()} has been deleted");
            }
        }

        public IEnumerable<ITableDefinition> ListTables()
        {
            return dataProvider.ListTables();
        }

        public ITableDefinition GetTable(string tableID)
        {
            if (tableID is null)
                throw new ArgumentNullException(nameof(tableID));

            lock (dataAccessLock)
            {
                CheckTableID(tableID);
                return dataProvider.GetTable(tableID);
            }
        }

        public void InsertTable(object administratorId, TableDefinitionInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));


            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                dataProvider.InsertTable(model);

                WriteAdminLog(administratorId, $"New personal data table {model.ID} was added to the schema");
            }
        }

        public void UpdateTable(object administratorId, TableDefinitionUpdateModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableID(model.ID);
                
                dataProvider.UpdateTable(model);

                WriteAdminLog(administratorId, $"Personal data table {model.ID} has ");
            }
        }

        public void DeleteTable(object administratorId, string tableId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentNullException(nameof(tableId));

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableID(tableId);
                
                if (dataProvider.CheckTableContainsExistingPersonalData(tableId))
                    throw new PersonalDataDBException($"Unable to delete table {tableId}, it contains existing personal data.");

                IEnumerable<IPurpose> purposes = dataProvider.ListPurposes();
                if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Tables && p.PurposeScope.TableIDs.Contains(tableId)))
                    throw new PersonalDataDBException($"Unable to delete table {tableId}, there are purposes with scope explicitly specified for this table");

                IEnumerable<IOwnerRestriction> restrictions = dataProvider.ListOwnerRestrictions();
                if (restrictions.Any(r => r.Scope.ScopeType == ScopeType.Tables && r.Scope.TableIDs.Contains(tableId)))
                    throw new PersonalDataDBException($"Unable to delete table {tableId}, there are owner restrictions with scope explicitly specified for this table");

                dataProvider.DeleteTable(tableId);
                WriteAdminLog(administratorId, $"Table {tableId} has been deleted");
            }
        }

        public void AddColumn(object administratorId, string tableId, ColumnDefinitionInsertModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentNullException(nameof(tableId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock(dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableID(tableId);

                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);
                if (columns.Any(c => c.ID.Equals(model.ID, StringComparison.InvariantCulture)))
                    throw new PersonalDataDBException($"Column {model.ID} already exists in table {tableId}");

                if(model.ColumnType == ColumnType.ForeignKey)
                {
                    if (String.IsNullOrEmpty(model.ForeignKeyReferenceTableID))
                        throw new ArgumentNullException(nameof(model.ForeignKeyReferenceTableID));

                    CheckTableID(model.ForeignKeyReferenceTableID);
                }

                dataProvider.InsertColumn(tableId, model);

                WriteAdminLog(administratorId, $"New column \"{model.ID}\" was added to the table \"{tableId}\".");
            }
        }

        public void UpdateColumn(object administratorId, string tableId, ColumnDefinitionUpdateModel model)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentNullException(nameof(tableId));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            lock(dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableID(tableId);
                CheckColumnId(tableId, model.ID);

                dataProvider.UpdateColumn(tableId, model);
                WriteAdminLog(administratorId, $"Column {model.ID} in table {tableId} has been updated");
            }
        }

        public void DeleteColumn(object administratorId, string tableId, string columnId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentNullException(nameof(tableId));

            if (String.IsNullOrEmpty(columnId))
                throw new ArgumentNullException(nameof(columnId));

            lock(dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableID(tableId);
                CheckColumnId(tableId, columnId);

                if (dataProvider.CheckColumnContainsExistingPersonalData(tableId, columnId))
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it contains existing personal data");

                IEnumerable<IPurpose> purposes = dataProvider.ListPurposes();
                if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Columns
                                   && p.PurposeScope.ColumnIDs.Any(pc => pc.TableId == tableId && pc.ColumnId == columnId)))
                {
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it is explicitly referenced in an existing purpose");
                }

                IEnumerable <IOwnerRestriction> restrictions = dataProvider.ListOwnerRestrictions();
                if (restrictions.Any(r => r.Scope.ScopeType == ScopeType.Columns
                                       && r.Scope.ColumnIDs.Any(pc => pc.TableId == tableId && pc.ColumnId == columnId)))
                {
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it is explicitly referenced in an existing owner restriction");
                }

                dataProvider.DeleteColumn(tableId, columnId);
                WriteAdminLog(administratorId, $"Column \"{columnId}\" in table \"{tableId}\" has been deleted");
            }
        }

        public IPersonalDataCell ReadPersonalDataCell(object userId, string tableId, object rowId, string columnId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException("Parameter tableId must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));

            if (String.IsNullOrEmpty(columnId))
                throw new ArgumentException("Parameter columnId must not be null or empty", nameof(columnId));
          
            lock(dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableID(tableId);
                CheckColumnId(tableId, columnId);
                CheckRowId(tableId, rowId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                CheckHoldingAndReadingAccessibility(ownerId, tableId, rowId, columnId);

                IPersonalDataCell personalDataCell = dataProvider.ReadPersonalDataCell(tableId, columnId, rowId);

                if (personalDataCell.IsDefined)
                {
                    ITableDefinition table = dataProvider.GetTable(tableId);
                    IColumnDefinition column = dataProvider.GetColumn(tableId, columnId);
                    string tableLogName = table.Name ?? table.ID;
                    string columnLogName = column.Name ?? column.ID;
                    WriteOwnerLog(ownerId, $"Your personal data has been read. We accessed your data in the the column \"{columnLogName}\" in the table \"{tableLogName}\"");
                }

                WriteUserLog(userId, $"Personal data cell of owner ID {ownerId} has been accessed: table {tableId}, column {columnId}, row {rowId.ToString()}");

                return personalDataCell;
            }
        }

        public IEnumerable<IPersonalDataCell> ReadPersonalDataCells(object userId, string tableId, object rowId, IEnumerable<string> columnIDs)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException("Parameter tableId must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));

            if (columnIDs == null)
                throw new ArgumentNullException(nameof(userId));

            if (!columnIDs.Any())
                throw new ArgumentException("You must define at least a single column ID.", nameof(columnIDs));

            if (columnIDs.GroupBy(c => c).Count() == columnIDs.Count())
                throw new ArgumentException($"There are duplicate column IDs", nameof(columnIDs));

            if (columnIDs.Count() == 1)
                return new IPersonalDataCell[] { ReadPersonalDataCell(userId, tableId, rowId, columnIDs.First()) };

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableID(tableId);
                CheckRowId(tableId, rowId);

                foreach (string columnId in columnIDs)
                    CheckColumnId(tableId, columnId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                CheckHoldingAndReadingAccessibility(ownerId, tableId, rowId, columnIDs);

                IEnumerable<IPersonalDataCell> row = dataProvider.ReadPersonalDataCells(tableId, rowId, columnIDs);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;
                string ownerColumnsLog = String.Join(", ", row.Where(r => r.IsDefined).Select(r =>
                  {
                      IColumnDefinition colDef = dataProvider.GetColumn(tableId, r.ColumnId);
                      return colDef.Name ?? colDef.ID;
                  }));

                string userColumnsLog = String.Join(", ", row.Select(r =>
                {
                    IColumnDefinition colDef = dataProvider.GetColumn(tableId, r.ColumnId);
                    return colDef.Name ?? colDef.ID;
                }));

                if (row.Any(r=>r.IsDefined))
                    WriteOwnerLog(ownerId, $"We read your data in the the columns \"{ownerColumnsLog}\" in the table \"{tableLogName}\"");
                
                WriteUserLog(userId, $"Multiple cells of owner ID \"{ownerId.ToString()}\" has been accessed: table \"{tableId}\", row \"{rowId.ToString()}\", columns \"{userColumnsLog}\"");
                return row;
            }
        }

        public IPersonalDataRow ReadPersonalDataRow(object userId, string tableId, object rowId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"Parameter {nameof(tableId)} must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));
            
            lock(dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableID(tableId);
                CheckRowId(tableId, rowId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);
                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);

                CheckHoldingAndReadingAccessibility(ownerId, tableId, rowId, columns.Select(c => c.ID));

                IPersonalDataRow row = dataProvider.ReadPersonalDataRow(tableId, rowId);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;

                if (row.PersonalDataCells.Any(r => r.IsDefined))
                    WriteOwnerLog(ownerId, $"We accessed your personal data and read the whole row with ID \"{rowId.ToString()}\" in the table \"{tableLogName}\"");

                WriteUserLog(userId, $"Whole row of owner ID \"{ownerId.ToString()}\" has been accessed in table \"{tableId}\"");

                return row;
            }
        }

        public IEnumerable<IPersonalDataRow> ReadPersonalDataTable(object userId, string tableId, object ownerId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"Parameter {nameof(tableId)} must not be null or empty", nameof(tableId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock(dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableID(tableId);
                CheckOwnerId(ownerId);

                IEnumerable<string> columnIds = dataProvider.ListColumns(tableId).Select(c=>c.ID);
                IEnumerable<object> rowIds = dataProvider.GetRowIdsForOwnerId(tableId, ownerId);

                CheckHoldingAndReadingAccessibility(ownerId, tableId, rowIds, columnIds);

                IEnumerable<IPersonalDataRow> personalDataRows = dataProvider.ReadPersonalDataCellByOwner(tableId, ownerId);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;

                if (personalDataRows.Any(p => p.PersonalDataCells.Any(c => c.IsDefined)))
                    WriteOwnerLog(ownerId, $"We accessed your personal data in the table \"{tableLogName}\" in all rows and columns");

                WriteUserLog(userId, $"Whole table \"{tableId}\" of owner ID \"{ownerId.ToString()}\" has been accessed");

                return personalDataRows;
            }
        }

        //TODO
        public IEnumerable<IPersonalDataTable> ReadPersonalDataAll(object userId, object ownerId)
        {
            throw new NotImplementedException();
        }
        private void CheckRowId(string tableId, object rowId)
        {
            if (!dataProvider.CheckRowId(tableId, rowId))
                throw new PersonalDataDBException($"Row with ID \"{rowId.ToString()}\" does not exist in table \"{tableId}\".");
        }

        private void CheckColumnId(string tableId, string columnId)
        {
            if (!dataProvider.CheckColumnId(tableId, columnId))
                throw new PersonalDataDBException($"Column \"{columnId}\" in table \"{tableId}\" does not exist");
        }

        private void CheckTableID(string tableId)
        {
            if (!dataProvider.CheckTableId(tableId))
                throw new PersonalDataDBException($"Table ID \"{tableId}\" does not exist");
        }

        private void CheckOwnerRestrictionExplanationId(object ownerRestrictionExplanationId)
        {
            if (!dataProvider.CheckOwnerRestrictionExplanationId(ownerRestrictionExplanationId))
                throw new PersonalDataDBException($"Owner restriction explanation ID \"{ownerRestrictionExplanationId.ToString()}\" does not exist");
        }

        private void CheckOwnerRestrictionId(object ownerRestrictionId)
        {
            if (!dataProvider.CheckOwnerRestrictionId(ownerRestrictionId))
                throw new PersonalDataDBException($"Owner restriction ID \"{ownerRestrictionId.ToString()}\" does not exist");
        }

        private void CheckAgreementTemplateId(object agreementTemplateId)
        {
            if (!dataProvider.CheckDataManagerId(agreementTemplateId))
                throw new PersonalDataDBException($"Agreement template ID \"{agreementTemplateId.ToString()}\" does not exist");
        }

        private void CheckDataManagerId(object dataManagerId)
        {
            if (!dataProvider.CheckDataManagerId(dataManagerId))
                throw new PersonalDataDBException($"Data Manager ID \"{dataManagerId.ToString()}\" does not exist");
        }

        private void CheckPurposeId(object purposeId)
        {
            if (!dataProvider.CheckPurposeId(purposeId))
                throw new PersonalDataDBException($"Purpose ID \"{purposeId.ToString()}\" does not exist");
        }
        private void CheckOwnerId(object ownerId)
        {
            if (!dataProvider.CheckOwnerId(ownerId))
                throw new PersonalDataDBException($"Owner ID \"{ownerId.ToString()}\" does not exist");
        }

        private void WriteOwnerLog(object ownerId, string text)
        {
            dataProvider.InsertOwnerLog(new OwnerLogInternalModel(DateTime.Now, ownerId, text));
        }

        private void WriteUserLog(object userId, string text)
        {
            dataProvider.InsertUserLog(new UserLogInternalModel(DateTime.Now, userId, text));
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
            if (!dataProvider.CheckAdministratorId(administratorId))
                throw new PersonalDataDBException($"Administrator ID \"{administratorId.ToString()}\" does not exist");
        }

        private void CheckUserId(object userId)
        {
            if (!dataProvider.CheckUserId(userId))
                throw new PersonalDataDBException($"User ID \"{userId.ToString()}\" does not exist");
        }

        private void CheckHoldingAndReadingAccessibility(object ownerId, string tableId, IEnumerable<object> rowIds, IEnumerable<string> columnIds)
        {
            CheckHoldingAndReadingAccessibility
            (
                ownerId: ownerId,
                tableIds: new string[] { tableId },
                rowIds: rowIds.Select(r => (tableId, r)),
                columnIds: columnIds.Select(c => (tableId, c))
            );
        }

        private void CheckHoldingAndReadingAccessibility(object ownerId, string tableId, object rowId, string columnId)
        {
            CheckHoldingAndReadingAccessibility
            (
                ownerId:    ownerId,
                tableIds:   new string[] { tableId },
                rowIds:     new (string tableId, object rowId)[] { (tableId, rowId) },
                columnIds:  new (string tableId, string columnId)[] { (tableId, columnId) }
            );
        }

        private void CheckHoldingAndReadingAccessibility(object ownerId, string tableId, object rowId, IEnumerable<string> columnIds)
        {
            CheckHoldingAndReadingAccessibility
            (
                ownerId: ownerId,
                tableIds: new string[] { tableId },
                rowIds: new (string tableId, object rowId)[] { (tableId, rowId) },
                columnIds: columnIds.Select(c => (tableId, c))
            );
        }

        private void CheckHoldingAndReadingAccessibility(object ownerId,
            IEnumerable<string> tableIds,
            IEnumerable<(string tableId, object rowId)> rowIds,
            IEnumerable<(string tableId, string columnId)> columnIds)
        {
            IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsForOwner(ownerId);

            if (!agreements.Any())
                throw new PersonalDataDBException("Unable to access personal data, no agreement found.");

            IEnumerable<object> purposeIDs = agreements.SelectMany(a => a.PurposeIDs);
            IEnumerable<IPurpose> purposes = dataProvider.ListPurposes()
                                                         .Where(p => purposeIDs.Contains(p.ID));

            if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Database && p.Type == PurposeType.HoldingAndReading))
                return;

            foreach (string tableId in tableIds)
            {
                if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Tables
                                   && p.Type == PurposeType.HoldingAndReading
                                   && p.PurposeScope.TableIDs.Contains(tableId)))
                {
                    continue;
                }

                IEnumerable<object> tableRows = rowIds.Where(r => r.tableId == tableId).Select(r => r.rowId);
                IEnumerable<string> tableColumns = columnIds.Where(r => r.tableId == tableId).Select(r => r.columnId);


                foreach (object rowId in tableRows)
                {
                    if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Rows
                                       && p.Type == PurposeType.HoldingAndReading
                                       && p.PurposeScope.RowIDs.Any(c => c.TableID == tableId
                                                                      && c.RowID.Equals(rowId))))
                    {
                        continue;
                    }


                    foreach (string columnId in tableColumns)
                    {
                        if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Columns
                                           && p.Type == PurposeType.HoldingAndReading
                                           && p.PurposeScope.ColumnIDs.Any(c => c.TableId == tableId
                                                                            && c.ColumnId == columnId)))
                        {
                            continue;
                        }

                        if (!purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Cells
                                           && p.Type == PurposeType.HoldingAndReading
                                           && p.PurposeScope.CellIDs.Any(c => c.TableID == tableId
                                                                           && c.RowID.Equals(rowId)
                                                                           && c.ColumnID == columnId)))
                        {
                            throw new PersonalDataDBException("Unable to access personal data, no relevant agreement found.");
                        }
                    }
                }

            }
        }
    }
}