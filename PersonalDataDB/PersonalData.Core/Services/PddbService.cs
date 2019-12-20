namespace PersonalData.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public abstract class PddbService
    {
        protected readonly IDataProvider dataProvider;
        internal PddbService(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        protected void CheckAdministratorId(object administratorId)
        {
            if (!dataProvider.CheckAdministratorId(administratorId))
                throw new PersonalDataDBException($"Administrator ID \"{administratorId.ToString()}\" does not exist");
        }

        protected void CheckUserId(object userId)
        {
            if (!dataProvider.CheckUserId(userId))
                throw new PersonalDataDBException($"User ID \"{userId.ToString()}\" does not exist");
        }

        protected void CheckAccessibility(PurposeType purposeType, object ownerId, string tableId, IEnumerable<object> rowIds, IEnumerable<string> columnIds)
        {
            CheckAccessibility
            (
                purposeType: purposeType,
                ownerId: ownerId,
                tableIds: new string[] { tableId },
                rowIds: rowIds.Select(r => (tableId, r)),
                columnIds: columnIds.Select(c => (tableId, c))
            );
        }

        protected void CheckAccessibility(PurposeType purposeType, object ownerId, string tableId, object rowId, string columnId)
        {
            CheckAccessibility
            (
                purposeType: purposeType,
                ownerId: ownerId,
                tableIds: new string[] { tableId },
                rowIds: new (string tableId, object rowId)[] { (tableId, rowId) },
                columnIds: new (string tableId, string columnId)[] { (tableId, columnId) }
            );
        }

        protected void CheckAccessibility(PurposeType purposeType, object ownerId, string tableId, object rowId, IEnumerable<string> columnIds)
        {
            CheckAccessibility
            (
                purposeType: purposeType,
                ownerId: ownerId,
                tableIds: new string[] { tableId },
                rowIds: new (string tableId, object rowId)[] { (tableId, rowId) },
                columnIds: columnIds.Select(c => (tableId, c))
            );
        }

        protected void CheckAccessibility
        (
            PurposeType purposeType,
            object ownerId,
            IEnumerable<string> tableIds,
            IEnumerable<(string tableId, object rowId)> rowIds,
            IEnumerable<(string tableId, string columnId)> columnIds
        )
        {
            IEnumerable<IAgreement> agreements = dataProvider.ListAgreementsForOwner(ownerId);

            if (!agreements.Any())
                throw new PersonalDataDBException("Unable to access personal data, no agreement found.");

            IEnumerable<object> purposeIDs = agreements.SelectMany(a => a.PurposeIDs);
            IEnumerable<IPurpose> purposes = dataProvider.ListPurposes()
                                                         .Where(p => purposeIDs.Contains(p.ID) && p.Type == purposeType);

            if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Database))
            {
                return;
            }

            foreach (string tableId in tableIds)
            {
                if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Tables
                                   && p.PurposeScope.TableIDs.Contains(tableId)))
                {
                    continue;
                }

                IEnumerable<object> tableRows = rowIds.Where(r => r.tableId == tableId).Select(r => r.rowId);
                IEnumerable<string> tableColumns = columnIds.Where(r => r.tableId == tableId).Select(r => r.columnId);

                if(purposeType == PurposeType.Writing && !rowIds.Any() && tableColumns.Any())
                {
                    foreach (string columnId in tableColumns)
                    {
                        if (!purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Columns
                                           && p.PurposeScope.ColumnIDs.Any(c => c.TableId == tableId
                                                                            && c.ColumnId == columnId)))
                        {
                            throw new PersonalDataDBException("Unable to write personal data, no relevant agreement found.");
                        }
                    }
                }

                foreach (object rowId in tableRows)
                {
                    if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Rows
                                       && p.PurposeScope.RowIDs.Any(c => c.TableID == tableId
                                                                      && c.RowID.Equals(rowId))))
                    {
                        continue;
                    }


                    foreach (string columnId in tableColumns)
                    {
                        if (purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Columns
                                           && p.PurposeScope.ColumnIDs.Any(c => c.TableId == tableId
                                                                            && c.ColumnId == columnId)))
                        {
                            continue;
                        }

                        if (!purposes.Any(p => p.PurposeScope.ScopeType == ScopeType.Cells
                                            && p.PurposeScope.CellIDs.Any(c => c.TableID == tableId
                                                                           && c.RowID.Equals(rowId)
                                                                           && c.ColumnID == columnId)))
                        {
                            if(purposeType == PurposeType.HoldingAndReading)
                                throw new PersonalDataDBException("Unable to access personal data, no relevant agreement found.");
                            else
                                throw new PersonalDataDBException("Unable to write personal data, no relevant agreement found.");
                        }
                    }
                }
            }
        }

        protected void WriteAdminLog(object administratorId, string text)
        {
            dataProvider.InsertAdministratorLog(new AdministratorLogInternalModel(DateTime.Now, administratorId, text));
        }
        protected void WriteOwnerLog(object ownerId, string text)
        {
            dataProvider.InsertOwnerLog(new OwnerLogInternalModel(DateTime.Now, ownerId, text));
        }

        protected void WriteUserLog(object userId, string text)
        {
            dataProvider.InsertUserLog(new UserLogInternalModel(DateTime.Now, userId, text));
        }
        protected void CheckRowId(string tableId, object rowId)
        {
            if (!dataProvider.CheckRowId(tableId, rowId))
                throw new PersonalDataDBException($"Row with ID \"{rowId.ToString()}\" does not exist in table \"{tableId}\".");
        }

        protected void CheckColumnId(string tableId, string columnId)
        {
            if (!dataProvider.CheckColumnId(tableId, columnId))
                throw new PersonalDataDBException($"Column \"{columnId}\" in table \"{tableId}\" does not exist");
        }

        protected void CheckTableId(string tableId)
        {
            if (!dataProvider.CheckTableId(tableId))
                throw new PersonalDataDBException($"Table ID \"{tableId}\" does not exist");
        }

        protected void CheckOwnerRestrictionExplanationId(object ownerRestrictionExplanationId)
        {
            if (!dataProvider.CheckOwnerRestrictionExplanationId(ownerRestrictionExplanationId))
                throw new PersonalDataDBException($"Owner restriction explanation ID \"{ownerRestrictionExplanationId.ToString()}\" does not exist");
        }

        protected void CheckOwnerRestrictionId(object ownerRestrictionId)
        {
            if (!dataProvider.CheckOwnerRestrictionId(ownerRestrictionId))
                throw new PersonalDataDBException($"Owner restriction ID \"{ownerRestrictionId.ToString()}\" does not exist");
        }

        protected void CheckAgreementTemplateId(object agreementTemplateId)
        {
            if (!dataProvider.CheckDataManagerId(agreementTemplateId))
                throw new PersonalDataDBException($"Agreement template ID \"{agreementTemplateId.ToString()}\" does not exist");
        }

        protected void CheckDataManagerId(object dataManagerId)
        {
            if (!dataProvider.CheckDataManagerId(dataManagerId))
                throw new PersonalDataDBException($"Data Manager ID \"{dataManagerId.ToString()}\" does not exist");
        }

        protected void CheckPurposeId(object purposeId)
        {
            if (!dataProvider.CheckPurposeId(purposeId))
                throw new PersonalDataDBException($"Purpose ID \"{purposeId.ToString()}\" does not exist");
        }
        protected void CheckOwnerId(object ownerId)
        {
            if (!dataProvider.CheckOwnerId(ownerId))
                throw new PersonalDataDBException($"Owner ID \"{ownerId.ToString()}\" does not exist");
        }
    }
}