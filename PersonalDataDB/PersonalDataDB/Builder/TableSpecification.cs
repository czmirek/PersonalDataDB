namespace PersonalDataDB
{
    using System.Collections.Generic;

    internal class TableSpecification : IColumnBuilder
    {
        public string TableName { get; private set; }
        public string Key { get; private set; }
        internal List<ScalarColumnSpecification> ScalarColumns { get; private set; } = new List<ScalarColumnSpecification>();
        internal List<ForeignKeyColumnSpecification> ForeignKeys { get; private set; } = new List<ForeignKeyColumnSpecification>();
        internal TableSpecification(string tableName)
        {
            TableName = tableName;
            Key = tableName + "ID";
        }

        public void SetKeyColumnName(string columnName)
        {
            Key = columnName;
        }

        public void AddScalar(string columName, ColumnType columnType, bool isNullable)
        {
            ScalarColumns.Add(new ScalarColumnSpecification(columName, columnType, isNullable));
        }

        public void AddForeignKey(string columnName, string referencedTableName, bool isNullable)
        {
            ForeignKeys.Add(new ForeignKeyColumnSpecification(columnName, referencedTableName, isNullable));
        }
    }
}