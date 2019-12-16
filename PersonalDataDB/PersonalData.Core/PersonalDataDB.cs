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
        
        private static readonly object dataManagerLock = new object();
        private static readonly object agreementTemplateLock = new object();
        private static readonly object agreementLock = new object();

        /// <summary>
        /// Lock used during initialization process
        /// </summary>
        private static readonly object initializationLock = new object();
        private static readonly object administratorLock = new object();
        private static readonly object purposeLock = new object();
        private static readonly object userLock = new object();
        private static readonly object ownerLock = new object();
        private static readonly object ownerRestrictionLock = new object();
        private static readonly object ownerRestrictionExplanationLock = new object();
        private static readonly object schemaLock = new object();

        /// <summary>
        /// Lock for locking access to personal data of a specific owner.
        /// </summary>
        private static readonly ConcurrentDictionary<object, object> ownerPersonalDataLocks = new ConcurrentDictionary<object, object>();

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
            if (initDataProvider is null)
                throw new ArgumentNullException(nameof(initDataProvider));

            lock (initializationLock)
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

            lock (administratorLock)
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
            
            MultiLock(new [] { administratorLock, purposeLock, agreementLock }, () =>
            {
                CheckAdministratorId(administratorId);
                CheckPurposeId(model.ID);

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsByPurpose(model.ID);
                if (agreements.Any())
                    throw new PersonalDataDBException($"Unable to update purpose ID \"{model.ID}\" because there are agreements referencing it.");

                dataProvider.UpdatePurpose(model);
            });
        }

        public void DeletePurpose(object administratorId, object purposeId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (purposeId is null)
                throw new ArgumentNullException(nameof(purposeId));

            MultiLock(new[] { administratorLock, purposeLock, agreementLock }, () =>
            {
                CheckAdministratorId(administratorId);
                CheckPurposeId(purposeId);

                IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsByPurpose(purposeId);
                if (agreements.Any())
                    throw new PersonalDataDBException($"Unable to delete purpose ID \"{purposeId.ToString()}\" because there are agreements referencing it.");

                dataProvider.DeletePurpose(purposeId);
            });
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

            lock (administratorLock)
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

            lock (administratorLock)
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

            lock (administratorLock)
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

            MultiLock(new[] { administratorLock, userLock }, () =>
            {
                CheckAdministratorId(administratorId);
                CheckUserId(model.ID);

                dataProvider.UpdateUser(model);

                WriteAdminLog(administratorId, $"User with ID \"{model.ID.ToString()}\" was updated.");
            });
        }

        public void UpdateAdministrator(object updatingAdministratorId, AdministratorUpdateModel updatedAdministrator)
        {
            if (updatingAdministratorId is null)
                throw new ArgumentNullException(nameof(updatingAdministratorId));

            if (updatedAdministrator is null)
                throw new ArgumentNullException(nameof(updatedAdministrator));

            lock (administratorLock)
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

            lock (administratorLock)
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

            lock (userLock)
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

            var personalDataLock = ownerPersonalDataLocks.GetOrAdd(ownerId, new object());

            MultiLock(new[] { administratorLock, ownerLock, personalDataLock }, () =>
            {
                try
                {
                    CheckAdministratorId(administratorId);
                    CheckOwnerId(ownerId);

                    if (dataProvider.PersonalDataExistsForOwner(ownerId))
                        throw new PersonalDataDBException("Unable to delete owner, there are still some personal data attached to it.");

                    dataProvider.DeleteOwner(ownerId);
                    WriteAdminLog(administratorId, $"Owner with ID {ownerId.ToString()} has been deleted");
                }
                finally
                {
                    ownerPersonalDataLocks.TryRemove(ownerId, out _);
                }
            });
        }

        public void DeleteOwnerByUser(object userId, object ownerId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            var personalDataLock = ownerPersonalDataLocks.GetOrAdd(ownerId, new object());

            MultiLock(new[] { userLock, ownerLock, personalDataLock }, () =>
            {
                try
                {
                    CheckUserId(userId);
                    CheckOwnerId(ownerId);

                    if (dataProvider.PersonalDataExistsForOwner(ownerId))
                        throw new PersonalDataDBException("Unable to delete owner, there are still some personal data attached to it.");

                    dataProvider.DeleteOwner(ownerId);
                    WriteUserLog(userId, $"Owner with ID {ownerId.ToString()} has been deleted");
                }
                finally
                {
                    ownerPersonalDataLocks.TryRemove(ownerId, out _);
                }
            });
        }

        public void DeleteOwnerByOwner(object ownerId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            var personalDataLock = ownerPersonalDataLocks.GetOrAdd(ownerId, new object());

            MultiLock(new[] { ownerLock, personalDataLock }, () =>
            {
                try
                {
                    CheckOwnerId(ownerId);

                    IEnumerable<IOwnerRestriction> ownerRestrictions = dataProvider.ListOwnerRestrictions(ownerId);

                    if (ownerRestrictions.Any())
                        throw new PersonalDataDBException("Unable to remove owner on his own volition because there are relevant owner restrictions attached.");

                    dataProvider.ClearAllPersonalDataForOwner(ownerId);
                    dataProvider.DeleteOwner(ownerId);
                    WriteOwnerLog(ownerId, $"Owner was deleted on his own volition");   
                }
                finally
                {
                    ownerPersonalDataLocks.TryRemove(ownerId, out _);
                }
            });
        }

        public void UpdateDataManager(object administratorId, DataManagerUpdateModel updatedDataManager)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (updatedDataManager is null)
                throw new ArgumentNullException(nameof(updatedDataManager));

            MultiLock(new[] { administratorLock, dataManagerLock, agreementTemplateLock, agreementLock }, () =>
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
            });
        }

        public void DeleteDataManager(object administratorId, object removedDataManagerId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (removedDataManagerId is null)
                throw new ArgumentNullException(nameof(removedDataManagerId));

            MultiLock(new[] { administratorLock, dataManagerLock, agreementTemplateLock, agreementLock }, () =>
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
            });
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

            MultiLock(new[] { administratorLock, purposeLock }, () =>
            {
                foreach (object purposeId in model.PurposeIDs)
                    CheckPurposeId(purposeId);

                CheckAdministratorId(administratorId);

                newId = dataProvider.InsertAgreementTemplate(model);
                WriteAdminLog(administratorId, $"New agreement template with ID {newId.ToString()} was created");
            });

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

            MultiLock(new[] { administratorLock, purposeLock, agreementLock, agreementTemplateLock }, () =>
            {
                foreach (object purposeId in model.PurposeIDs)
                    CheckPurposeId(purposeId);

                CheckAdministratorId(administratorId);

                dataProvider.UpdateAgreementTemplate(model);
                WriteAdminLog(administratorId, $"Agreement template with ID {model.ID.ToString()} was updated");
            });
        }

        public void DeleteAgreementTemplate(object administratorId, object agreementTemplateId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (agreementTemplateId is null)
                throw new ArgumentNullException(nameof(agreementTemplateId));

            MultiLock(new[] { administratorLock, purposeLock, agreementLock, agreementTemplateLock }, () =>
            {
                CheckAdministratorId(administratorId);
                CheckAgreementTemplateId(agreementTemplateId);
                dataProvider.DeleteAgreementTemplate(agreementTemplateId);
                WriteAdminLog(administratorId, $"Agreement template with ID {agreementTemplateId.ToString()} has been deleted");
            });
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
            MultiLock(new[] { userLock, ownerRestrictionLock }, () =>
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
            });

            return newId;
        }

        public void DeleteOwnerRestriction(object userId, object ownerRestrictionId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerRestrictionId is null)
                throw new ArgumentNullException(nameof(ownerRestrictionId));

            MultiLock(new[] { userLock, ownerRestrictionLock }, () =>
            {
                CheckUserId(userId);
                CheckOwnerRestrictionId(ownerRestrictionId);
                
                IOwnerRestriction ownerRestriction = dataProvider.GetOwnerRestriction(ownerRestrictionId);

                dataProvider.DeleteOwnerRestriction(ownerRestrictionId);
                
                WriteUserLog(userId, $"Owner restriction ID {ownerRestrictionId.ToString()} was deleted");
                WriteOwnerLog(ownerRestriction.OwnerID, $"Owner restriction ID {ownerRestrictionId.ToString()} was deleted");
            });
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
            MultiLock(new[] { administratorLock, ownerRestrictionExplanationLock }, () =>
            {
                CheckAdministratorId(administratorId);
                newId = dataProvider.InsertOwnerRestrictionExplanation(model);
                WriteAdminLog(administratorId, $"New owner restriction with ID {newId.ToString()} has been created");
            });
            return newId;
        }

        public void DeleteOwnerRestrictionExplanation(object administratorId, object ownerRestrictionExplanationId)
        {
            if (administratorId is null)
                throw new ArgumentNullException(nameof(administratorId));

            if (ownerRestrictionExplanationId is null)
                throw new ArgumentNullException(nameof(ownerRestrictionExplanationId));

            MultiLock(new[] { administratorLock, ownerRestrictionLock, ownerRestrictionExplanationLock}, () =>
            {
                CheckAdministratorId(administratorId);
                CheckOwnerRestrictionExplanationId(ownerRestrictionExplanationId);
                
                IEnumerable<IOwnerRestriction> restrictions = dataProvider.ListOwnerRestrictions();
                if (restrictions.Any(r => r.OwnerRestrictionExplanationID !=null && r.OwnerRestrictionExplanationID.Equals(ownerRestrictionExplanationId)))
                    throw new PersonalDataDBException("Unable to delete owner restriction explanation, there are some owner restrictions referencing it");
                
                dataProvider.DeleteOwnerRestrictionExplanation(ownerRestrictionExplanationId);
                WriteAdminLog(administratorId, $"Owner restriction explanation with ID {ownerRestrictionExplanationId.ToString()} has been deleted");
            });
        }

        public IEnumerable<ITableDefinition> ListTables()
        {
            return dataProvider.ListTables();
        }

        public ITableDefinition GetTable(string tableID)
        {
            if (tableID is null)
                throw new ArgumentNullException(nameof(tableID));

            lock (schemaLock)
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


            MultiLock(new[] { administratorLock }, () =>
            {
                CheckAdministratorId(administratorId);
                dataProvider.InsertTable(model);
                WriteAdminLog(administratorId, $"New personal data table {model.ID} was added to the schema");
            });
        }

        //TODO
        //public void UpdateTable()

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

        private void MultiLock(object[] locks, Action action, int index = 0)
        {
            if (index < locks.Count())
            {
                lock (locks[index])
                {
                    MultiLock(locks, action, index + 1);
                }
            }
            else
            {
                action();
            }
        }
    }
}