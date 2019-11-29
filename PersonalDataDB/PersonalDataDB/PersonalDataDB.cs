namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PersonalDataDB
    {
        private readonly ITableSet tableSet;
        private IDataProvider dataProvider;

        internal PersonalDataDB(IDataProvider dataProvider, ITableSet tableSet)
        {
            this.dataProvider = dataProvider;
            this.tableSet = tableSet;
        }

        public object Insert(string tableName, IDictionary<string, object?> data)
        {
            ValidateInput(tableName, data);
            ValidateDataWrite(tableName, data, isInsert: true);
            return dataProvider.Insert(tableName, data);
        }

        public IDictionary<string, object?> ReadRow(string tableName, object rowKey)
        {
            ValidateTable(tableName);
            return dataProvider.ReadRow(tableName, rowKey);
        }

        public IDictionary<string, object?> ReadRow(string tableName, object rowKey, IEnumerable<string> requiredColumns)
        {
            ValidateColumns(tableName, requiredColumns);
            return dataProvider.ReadRow(tableName, rowKey, requiredColumns);
        }

        public object? ReadValue (string tableName, object rowKey, string column)
        {
            ValidateColumns(tableName, new string[] { column });
            return dataProvider.ReadValue(tableName, rowKey, column);
        }

        public void Update(string tableName, object rowKey, string column, object? value)
            => Update(tableName, rowKey, new Dictionary<string, object?>() { { column, value } });

        public void Update(string tableName, object rowKey, IDictionary<string, object?> data)
        {
            ValidateInput(tableName, data);
            ValidateDataWrite(tableName, data);
            dataProvider.UpdateRow(tableName, rowKey, data);
        }

        private void ValidateTable(string tableName)
        {
            if (!tableSet.Tables.Any(t => t.TableName.Equals(tableName, StringComparison.InvariantCulture)))
                throw new PersonalDataDBException($"Table \"{tableName}\" does not exist (table names are case sensitive).");
        }

        private void ValidateInput(string tableName, IDictionary<string, object?> inputData)
        {
            ValidateTable(tableName);

            IEnumerable<IColumnDefinition> columns = tableSet[tableName].AllColumns;
            foreach (string key in inputData.Keys)
            {
                IColumnDefinition? colDef = columns.FirstOrDefault(t => t.ColumnName.Equals(key, StringComparison.InvariantCulture));
                if (colDef == null)
                    throw new PersonalDataDBException($"Column \"{key}\" in table \"{tableName}\" does not exist (column names are case sensitive).");

                if (colDef is IScalarColumnDefinition scalarDef)
                {
                    if (!scalarDef.IsNullable && inputData[key] == null)
                        throw new PersonalDataDBException($"Column \"{key}\" cannot be null");
                }
            }
        }
        private void ValidateColumns(string tableName, IEnumerable<string> requiredColumns)
        {
            ValidateTable(tableName);

            IEnumerable<IColumnDefinition> columns = tableSet[tableName].AllColumns;

            foreach (string requiredColumn in requiredColumns)
            {
                if (!columns.Any(c => c.ColumnName.Equals(requiredColumn, StringComparison.InvariantCulture)))
                    throw new PersonalDataDBException($"Column \"{requiredColumn}\" in table \"{tableName}\" does not exist");
            }
        }
        private void ValidateDataWrite(string tableName, IDictionary<string, object?> data, bool isInsert = false)
        {
            ITableDefinition tableDef = tableSet[tableName];
            
            if (data.ContainsKey(tableDef.PrimaryKey.ColumnName))
                throw new NotSupportedException("Explicit primary key values are not yet supported");

            IEnumerable<INullableColumnDefinition> columns = tableDef.AllColumns
                                                             .Except(new IColumnDefinition[] { tableSet[tableName].PrimaryKey })
                                                             .Cast<INullableColumnDefinition>();

            foreach (KeyValuePair<string,object?> kvp in data)
            {
                //kontrola že data mají související sloupec
                INullableColumnDefinition? foundColumn = columns.FirstOrDefault(c => c.ColumnName.Equals(kvp.Key, StringComparison.InvariantCulture));
                if (foundColumn == null)
                    throw new PersonalDataDBException($"Column \"{kvp.Key}\" does not exist in table \"{tableName}\"");

                //kontrola, že vkládaná/aktualizovaná data mohou obsahovat null
                if (!foundColumn.IsNullable && data[foundColumn.ColumnName] == null)
                    throw new PersonalDataDBException($"Column \"{foundColumn.ColumnName}\" cannot contain null");
            }

            if (isInsert)
            {
                foreach (INullableColumnDefinition colDef in columns)
                {
                    //kontrola, že sloupec má související data
                    if (!data.ContainsKey(colDef.ColumnName))
                        throw new PersonalDataDBException($"Column \"{colDef.ColumnName}\" must be included in the inserted data.");
                }
            }
        }
    }
}