namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultTableBuilder : ITableBuilder
    {
        internal List<DefaultTable> Tables { get; private set; } = new List<DefaultTable>();
        public void Add(string tableName, Action<IColumnBuilder> columnBuilder)
        {
            if (Tables.Any(tbl => tbl.TableName == tableName))
                throw new TableBuilderException($"Duplicate table \"{tableName}\"");

            if(tableName.IsEmptyOrWhiteSpace())
                throw new TableBuilderException($"Table \"{tableName}\" must not be empty or contain whitespace.");

            DefaultTable newTable = new DefaultTable(tableName);
            columnBuilder.Invoke(newTable);

            Tables.Add(newTable);
        }
    }
}