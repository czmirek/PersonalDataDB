namespace PersonalData.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OwnerService : PddbService
    {
        private readonly LockProvider lockProvider;
        private object dataAccessLock => lockProvider.GetLock();
        internal OwnerService(IDataProvider dataProvider, LockProvider lockProvider) : base(dataProvider)
        {
            this.lockProvider = lockProvider;
        }
        public object InsertOwner()
        {
            object newId = dataProvider.InsertOwner();
            WriteOwnerLog(newId, $"New owner was created on his own volition");
            return newId;
        }
        public void DeleteOwner(object ownerId)
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

        public IPersonalDataCell ReadPersonalDataCell(object ownerId, string tableId, object rowId, string columnId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException("Parameter tableId must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));

            if (String.IsNullOrEmpty(columnId))
                throw new ArgumentException("Parameter columnId must not be null or empty", nameof(columnId));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);
                CheckColumnId(tableId, columnId);
                CheckRowId(tableId, rowId);

                object trueOwnerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                if (!ownerId.Equals(trueOwnerId))
                    throw new PersonalDataDBException($"The requested row does not belong to the owner in the parameter {nameof(ownerId)}");

                IPersonalDataCell personalDataCell = dataProvider.ReadPersonalDataCell(tableId, columnId, rowId);
                WriteUserLog(ownerId, $"You accessed your personal data in the table cell: table {tableId}, column {columnId}, row {rowId.ToString()}");

                return personalDataCell;
            }
        }

        public IEnumerable<IPersonalDataCell> ReadPersonalDataCells(object ownerId, string tableId, object rowId, IEnumerable<string> columnIDs)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException("Parameter tableId must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));

            if (columnIDs == null)
                throw new ArgumentNullException(nameof(ownerId));

            if (!columnIDs.Any())
                throw new ArgumentException("You must define at least a single column ID.", nameof(columnIDs));

            if (columnIDs.GroupBy(c => c).Count() == columnIDs.Count())
                throw new ArgumentException($"There are duplicate column IDs", nameof(columnIDs));

            if (columnIDs.Count() == 1)
                return new IPersonalDataCell[] { ReadPersonalDataCell(ownerId, tableId, rowId, columnIDs.First()) };

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);
                CheckRowId(tableId, rowId);

                foreach (string columnId in columnIDs)
                    CheckColumnId(tableId, columnId);

                object trueOwnerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                if (!ownerId.Equals(trueOwnerId))
                    throw new PersonalDataDBException($"The requested row does not belong to the owner in the parameter {nameof(ownerId)}");

                IEnumerable<IPersonalDataCell> row = dataProvider.ReadPersonalDataCells(tableId, rowId, columnIDs);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;
                string ownerColumnsLog = String.Join(", ", row.Where(r => r.IsDefined).Select(r =>
                {
                    IColumnDefinition colDef = dataProvider.GetColumn(tableId, r.ColumnId);
                    return colDef.Name ?? colDef.ID;
                }));
                WriteOwnerLog(ownerId, $"You accessed your personal data in the the columns \"{ownerColumnsLog}\" in the table \"{tableLogName}\"");
                return row;
            }
        }

        public IPersonalDataRow ReadPersonalDataRow(object ownerId, string tableId, object rowId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"Parameter {nameof(tableId)} must not be null or empty", nameof(tableId));

            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);
                CheckRowId(tableId, rowId);

                object trueOwnerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                if (!ownerId.Equals(trueOwnerId))
                    throw new PersonalDataDBException($"The requested row does not belong to the owner in the parameter {nameof(ownerId)}");

                IPersonalDataRow row = dataProvider.ReadPersonalDataRow(tableId, rowId);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;

                WriteOwnerLog(ownerId, $"You accessed your personal data and read the whole row with ID \"{rowId.ToString()}\" in the table \"{tableLogName}\"");
                return row;
            }
        }
        public IEnumerable<IPersonalDataRow> ReadPersonalDataTable(object ownerId, string tableId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"Parameter {nameof(tableId)} must not be null or empty", nameof(tableId));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);

                IEnumerable<IPersonalDataRow> personalDataRows = dataProvider.ReadPersonalDataRowsByOwner(tableId, ownerId);

                ITableDefinition table = dataProvider.GetTable(tableId);
                string tableLogName = table.Name ?? table.ID;

                if (personalDataRows.Any(p => p.PersonalDataCells.Any(c => c.IsDefined)))
                    WriteOwnerLog(ownerId, $"You accessed all your personal data in the table \"{tableLogName}\".");

                return personalDataRows;
            }
        }

        public IEnumerable<IPersonalDataTable> ReadPersonalDataAll(object ownerId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                WriteOwnerLog(ownerId, $"You accessed all your personal data.");
                return dataProvider.ReadAllPersonalDataByOwner(ownerId);
            }
        }

        public string GetAllPersonalDataInJson(object ownerId)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                IEnumerable<IPersonalDataTable> personalDataTables = dataProvider.ReadAllPersonalDataByOwner(ownerId);

                JsonOutputService jsonService = new JsonOutputService(dataProvider);
                return jsonService.CreateJson(personalDataTables);
            }
        }

        public object InsertPersonalDataRow(object ownerId, string tableId, IDictionary<string, object> data)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"{nameof(tableId)} must not be null or empty", nameof(tableId));

            if (data is null)
                throw new ArgumentNullException(nameof(data));

            lock (dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);

                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);
                if (columns.Any(c => !data.ContainsKey(c.ID)))
                    throw new PersonalDataDBException($"All columns must be provided in the {nameof(data)} parameter");

                if (data.Keys.Any(k => !columns.Any(c => k == c.ID)))
                    throw new PersonalDataDBException($"{nameof(data)} parameter contains a column which is not defined in the schema");

                object newId = dataProvider.InsertPersonalDataRow(tableId, ownerId, data);

                WriteOwnerLog(ownerId, $"You inserted a new row of your personal data in the table \"{tableId}\" with ID {newId.ToString()}.");
                return newId;
            }
        }

        public void UpdatePersonalDataRow(object ownerId, string tableId, object rowId, IDictionary<string, object> data)
        {
            if (ownerId is null)
                throw new ArgumentNullException(nameof(ownerId));

            if (String.IsNullOrEmpty(tableId))
                throw new ArgumentException($"{nameof(tableId)} must not be null or empty", nameof(tableId));
        
            if (rowId is null)
                throw new ArgumentNullException(nameof(rowId));
            
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            if (data.Count == 0)
                throw new ArgumentException($"There must be at least a signle column defined in the parameter {nameof(data)}", nameof(data));

            lock(dataAccessLock)
            {
                CheckOwnerId(ownerId);
                CheckTableId(tableId);
                CheckRowId(tableId, rowId);
                
                object trueOwnerId = dataProvider.GetOwnerForRowId(tableId, rowId);

                if (!ownerId.Equals(trueOwnerId))
                    throw new PersonalDataDBException($"The requested row does not belong to the owner in the parameter {nameof(ownerId)}");

                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(tableId);
                if (data.Keys.Any(k => !columns.Any(c => k == c.ID)))
                    throw new PersonalDataDBException($"{nameof(data)} parameter contains a column which is not defined in the schema");

                dataProvider.UpdatePersonalDataRow(ownerId, tableId, rowId, data);
                WriteOwnerLog(ownerId, $"You updated a row of your personal data in the table \"{tableId}\" with ID {rowId.ToString()}.");
            }
        }

        //todo 13.4
    }
}