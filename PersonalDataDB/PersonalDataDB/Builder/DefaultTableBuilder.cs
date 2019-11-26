namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultTableBuilder : ITableBuilder
    {
        internal List<TableSpecification> Tables { get; private set; } = new List<TableSpecification>();
        public void Add(string tableName, Action<IColumnBuilder> columnBuilder)
        {
            TableSpecification newTable = new TableSpecification(tableName);
            columnBuilder.Invoke(newTable);

            Tables.Add(newTable);
        }
    }
}