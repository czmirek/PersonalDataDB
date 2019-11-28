namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;

    public class DefaultTableBuilder : ITableSet, ITableBuilder
    {
        public ITableDefinition this[string tableName] => Tables[tableName];
        internal Dictionary<string, DefaultTable> Tables { get; private set; } = new Dictionary<string, DefaultTable>();
        public void Add(string tableName, Action<IColumnBuilder> columnBuilder)
        {
            if (Tables.ContainsKey(tableName))
                throw new TableBuilderException($"Duplicate table \"{tableName}\"");

            if(tableName.IsEmptyOrWhiteSpace())
                throw new TableBuilderException($"Table \"{tableName}\" must not be empty or contain whitespace.");

            DefaultTable newTable = new DefaultTable(tableName);
            columnBuilder.Invoke(newTable);

            Tables.Add(newTable.TableName, newTable);
        }
        IEnumerable<ITableDefinition> ITableSet.Tables => Tables.Values;
    }
}