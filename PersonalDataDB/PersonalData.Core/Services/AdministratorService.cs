namespace PersonalData.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class AdministratorService : PddbService
    {
        private readonly LockProvider lockProvider;
        private object dataAccessLock => lockProvider.GetLock();
        internal AdministratorService(IDataProvider dataProvider, LockProvider lockProvider) : base(dataProvider)
        {
            this.lockProvider = lockProvider;
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

            lock (dataAccessLock)
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

        public object InsertOwner(object administratorId)
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

        public void DeleteOwner(object administratorId, object ownerId)
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
                CheckTableId(model.ID);

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
                CheckTableId(tableId);

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

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableId(tableId);

                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);
                if (columns.Any(c => c.ID.Equals(model.ID, StringComparison.InvariantCulture)))
                    throw new PersonalDataDBException($"Column {model.ID} already exists in table {tableId}");

                if (model.ColumnType == ColumnType.ForeignKey)
                {
                    if (String.IsNullOrEmpty(model.ForeignKeyReferenceTableID))
                        throw new ArgumentNullException(nameof(model.ForeignKeyReferenceTableID));

                    CheckTableId(model.ForeignKeyReferenceTableID);
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

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableId(tableId);
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

            lock (dataAccessLock)
            {
                CheckAdministratorId(administratorId);
                CheckTableId(tableId);
                CheckColumnId(tableId, columnId);

                if (dataProvider.CheckColumnContainsExistingPersonalData(tableId, columnId))
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it contains existing personal data");

                IEnumerable<IPurpose> purposes = dataProvider.ListPurposes();
                if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Columns
                                   && p.PurposeScope.ColumnIDs.Any(pc => pc.TableId == tableId && pc.ColumnId == columnId)))
                {
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it is explicitly referenced in an existing purpose");
                }

                IEnumerable<IOwnerRestriction> restrictions = dataProvider.ListOwnerRestrictions();
                if (restrictions.Any(r => r.Scope.ScopeType == ScopeType.Columns
                                       && r.Scope.ColumnIDs.Any(pc => pc.TableId == tableId && pc.ColumnId == columnId)))
                {
                    throw new PersonalDataDBException($"Unable to delete column {columnId} in table {tableId} because it is explicitly referenced in an existing owner restriction");
                }

                dataProvider.DeleteColumn(tableId, columnId);
                WriteAdminLog(administratorId, $"Column \"{columnId}\" in table \"{tableId}\" has been deleted");
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
                CheckTableId(tableID);
                return dataProvider.GetTable(tableID);
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
                if (restrictions.Any(r => r.OwnerRestrictionExplanationID != null && r.OwnerRestrictionExplanationID.Equals(ownerRestrictionExplanationId)))
                    throw new PersonalDataDBException("Unable to delete owner restriction explanation, there are some owner restrictions referencing it");

                dataProvider.DeleteOwnerRestrictionExplanation(ownerRestrictionExplanationId);
                WriteAdminLog(administratorId, $"Owner restriction explanation with ID {ownerRestrictionExplanationId.ToString()} has been deleted");
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

            if (!model.PurposeIDs.Any())
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
    }
}
