using System;

namespace PersonalDataDB
{
    internal class ScalarColumnSpecification
    {
        public ScalarColumnSpecification(string columnName, ColumnType columnType, bool isNullable)
        {
            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.IsNullable = isNullable;
        }

        internal string ColumnName { get; private set; }
        internal ColumnType ColumnType { get; private set; }
        internal bool IsNullable { get; private set; }
    }
}