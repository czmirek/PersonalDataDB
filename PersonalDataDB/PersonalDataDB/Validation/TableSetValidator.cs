namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    public class TableSetValidator
    {
        private ITableSet tableSet;

        public TableSetValidator(ITableSet tableSet)
        {
            this.tableSet = tableSet;
        }

        public void ValidateTable(string tableName)
        {
            if (!tableSet.Tables.Any(t => t.TableName.Equals(tableName, StringComparison.InvariantCulture)))
                throw new PersonalDataDBException($"Table \"{tableName}\" does not exist (table names are case sensitive).");
        }

        public void ValidateInput(string tableName, IDictionary<string, object?> inputData)
        {
            ValidateTable(tableName);

            IEnumerable<IColumnDefinition> columns = tableSet[tableName].AllColumns;
            foreach (string key in inputData.Keys)
            {
                IColumnDefinition? colDef = columns.FirstOrDefault(t => t.ColumnName.Equals(key, StringComparison.InvariantCulture));
                if (colDef == null)
                    throw new PersonalDataDBException($"Column \"{key}\" in table \"{tableName}\" does not exist (column names are case sensitive).");

                if(colDef is IScalarColumnDefinition scalarDef)
                {
                    if (!scalarDef.IsNullable && inputData[key] == null)
                        throw new PersonalDataDBException($"Column \"{key}\" cannot be null");
                }
            }
        }
    }
}
