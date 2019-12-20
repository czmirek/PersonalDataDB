using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalData.Core.Services
{
    public class UserService : PddbService
    {
        private readonly LockProvider lockProvider;
        private object dataAccessLock => lockProvider.GetLock();
        internal UserService(IDataProvider dataProvider, LockProvider lockProvider) : base(dataProvider)
        {
            this.lockProvider = lockProvider;
        }

        public object InsertOwner(object userId)
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

        public void DeleteOwner(object userId, object ownerId)
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

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableId(tableId);
                CheckColumnId(tableId, columnId);
                CheckRowId(tableId, rowId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                CheckAccessibility(PurposeType.HoldingAndReading, ownerId, tableId, rowId, columnId);

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
                CheckTableId(tableId);
                CheckRowId(tableId, rowId);

                foreach (string columnId in columnIDs)
                    CheckColumnId(tableId, columnId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                CheckAccessibility(PurposeType.HoldingAndReading, ownerId, tableId, rowId, columnIDs);

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

                if (row.Any(r => r.IsDefined))
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

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableId(tableId);
                CheckRowId(tableId, rowId);

                object ownerId = dataProvider.GetOwnerForRowId(tableId, rowId);
                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);

                CheckAccessibility(PurposeType.HoldingAndReading, ownerId, tableId, rowId, columns.Select(c => c.ID));

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

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckTableId(tableId);
                CheckOwnerId(ownerId);

                IEnumerable<string> columnIds = dataProvider.ListColumns(tableId).Select(c => c.ID);
                IEnumerable<object> rowIds = dataProvider.GetRowIdsForOwnerId(tableId, ownerId);

                CheckAccessibility(PurposeType.HoldingAndReading, ownerId, tableId, rowIds, columnIds);

                IEnumerable<IPersonalDataRow> personalDataRows = dataProvider.ReadPersonalDataRowsByOwner(tableId, ownerId);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;

                if (personalDataRows.Any(p => p.PersonalDataCells.Any(c => c.IsDefined)))
                    WriteOwnerLog(ownerId, $"We accessed your personal data in the table \"{tableLogName}\" in all rows and columns");

                WriteUserLog(userId, $"Whole table \"{tableId}\" of owner ID \"{ownerId.ToString()}\" has been accessed");

                return personalDataRows;
            }
        }

        public IEnumerable<IPersonalDataTable> ReadPersonalDataAll(object userId, object ownerId)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckOwnerId(ownerId);


                IEnumerable<string> tableIds = dataProvider.ListTables().Where(t =>
                {
                    return dataProvider.CheckTableContainsExistingPersonalData(t.ID);
                }).Select(t => t.ID);

                IEnumerable<(string tableId, object rowId)> rowIDs = tableIds.SelectMany(tableId =>
                {
                    return dataProvider.GetRowIdsForOwnerId(tableId, ownerId)
                                       .Select(rowId => (tableId, rowId));
                });

                IEnumerable<(string tableId, string columnId)> columnIds = tableIds.SelectMany(tableId =>
                {
                    return dataProvider.ListColumns(tableId)
                                       .Where(c => dataProvider.CheckColumnContainsExistingPersonalData(tableId, c.ID))
                                       .Select(c => (tableId, c.ID));
                });

                CheckAccessibility(PurposeType.HoldingAndReading, ownerId, tableIds, rowIDs, columnIds);

                if (tableIds.Any())
                    WriteOwnerLog(ownerId, $"We accessed all your personal data.");

                WriteUserLog(userId, $"All personal data of owner ID \"{ownerId.ToString()}\" has been accessed");

                return dataProvider.ReadAllPersonalDataByOwner(ownerId);
            }
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

        public object InsertPersonalDataRow(object userId, object ownerId, string tableId, IDictionary<string, object> data)
        {
            if (userId is null)
                throw new ArgumentNullException(nameof(userId));

            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"{nameof(tableId)} must not be null or empty", nameof(tableId));

            if (data is null)
                throw new ArgumentNullException(nameof(data));

            lock (dataAccessLock)
            {
                CheckUserId(userId);
                CheckOwnerId(ownerId);
                CheckTableId(tableId);

                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);
                if (columns.Any(c => !data.ContainsKey(c.ID)))
                    throw new PersonalDataDBException($"All columns must be provided in the {nameof(data)} parameter");

                if (data.Keys.Any(k => !columns.Any(c => k == c.ID)))
                    throw new PersonalDataDBException($"{nameof(data)} parameter contains a column which is not defined in the schema");

                CheckAccessibility(PurposeType.Writing, ownerId, tableId, new object[] { }, columns.Select(c => c.ID));

                object newId = dataProvider.InsertPersonalDataRow(tableId, ownerId, data);

                WriteUserLog(userId, $"You inserted a new row in the personal data table \"{tableId}\" for the owner \"{ownerId}\"");
                WriteOwnerLog(ownerId, $"We inserted a new row of your personal data in the table \"{tableId}\" with ID {newId.ToString()}.");
                return newId;
            }
        }
    }
}
