using System;

namespace PersonalDataDB
{
    internal class DefaultScalarColumn : IScalarColumnDefinition
    {
        public DefaultScalarColumn(string columnName, ColumnType columnType, bool isNullable)
        {
            if (columnName.IsEmptyOrWhiteSpace())
                throw new TableBuilderException($"Column \"{columnName}\" must not be empty or contain whitespace");

            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.IsNullable = isNullable;
        }

        public string ColumnName { get; private set; }
        public ColumnType ColumnType { get; private set; }
        public bool IsNullable { get; private set; }
    }
}