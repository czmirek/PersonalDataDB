﻿namespace PersonalDataDB
{
    using System.Collections.Generic;
    using System.Linq;

    internal class DefaultTable : IColumnBuilder, ITableDefinition
    {
        public string TableName { get; private set; }
        public DefaultPrimaryKey Key { get; private set; }
        internal List<DefaultScalarColumn> ScalarColumns { get; private set; } = new List<DefaultScalarColumn>();
        internal List<DefaultForeignKey> ForeignKeys { get; private set; } = new List<DefaultForeignKey>();
        IPrimaryKeyDefinition ITableDefinition.PrimaryKey => Key;
        IEnumerable<IScalarColumnDefinition> ITableDefinition.ScalarColumns => ScalarColumns;

        IEnumerable<IForeignKeyDefinition> ITableDefinition.ForeignKeys => ForeignKeys;
        public IEnumerable<IColumnDefinition> AllColumns
        {
            get
            {
                yield return Key;
                
                foreach (var scalarColumn in ScalarColumns)
                    yield return scalarColumn;

                foreach (var fkColumn in ForeignKeys)
                    yield return fkColumn;
            }
        }

        public IColumnDefinition this[string columnName]
        {
            get
            {
                IColumnDefinition? foundColumn = AllColumns.FirstOrDefault(c => c.ColumnName.Equals(columnName, System.StringComparison.InvariantCulture));
                
                if (foundColumn == null)
                    throw new PersonalDataDBException($"Column {columnName} does not exist");

                return foundColumn;
            }
        }
        
        
        internal DefaultTable(string tableName)
        {
            TableName = tableName;
            Key = new DefaultPrimaryKey(tableName + "ID");
        }

        public void SetKeyColumnName(string columnName)
        {
            ValidateNewColumn(columnName, checkKey: false);
            Key = new DefaultPrimaryKey(columnName);
        }

        public void AddScalar(string columnName, ColumnType columnType, bool isNullable)
        {
            ValidateNewColumn(columnName);
            ScalarColumns.Add(new DefaultScalarColumn(columnName, columnType, isNullable));
        }

        public void AddForeignKey(string columnName, string referencedTableName, bool isNullable)
        {
            ValidateNewColumn(columnName);
            ForeignKeys.Add(new DefaultForeignKey(columnName, referencedTableName, isNullable));
        }

        private void ValidateNewColumn(string columnName, bool checkKey = true)
        {
            if (ScalarColumns.Any(sc => sc.ColumnName == columnName))
                throw new TableBuilderException($"Duplicate column \"{columnName}\" in table \"{TableName}\"");

            if (ForeignKeys.Any(fk => fk.ColumnName == columnName))
                throw new TableBuilderException($"Duplicate column \"{columnName}\" in table \"{TableName}\"");

            if (checkKey && Key.ColumnName == columnName)
                throw new TableBuilderException($"Column \"{columnName}\" has same name as key in table \"{TableName}\"");
        }
    }
}