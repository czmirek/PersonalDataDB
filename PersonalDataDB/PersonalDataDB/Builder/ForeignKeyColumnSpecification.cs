using System;

namespace PersonalDataDB
{
    internal class ForeignKeyColumnSpecification
    {
        public ForeignKeyColumnSpecification(string columnName, string foreignTable, bool isNullable)
        {
            this.ColumnName = columnName;
            this.ForeignTable = foreignTable;
            this.IsNullable = isNullable;
        }

        internal string ColumnName { get; private set; }
        internal string ForeignTable { get; private set; }
        internal bool IsNullable { get; private set; }
    }
}