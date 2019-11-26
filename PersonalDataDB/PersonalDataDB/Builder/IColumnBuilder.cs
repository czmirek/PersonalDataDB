namespace PersonalDataDB
{
    public interface IColumnBuilder
    {
        void SetKeyColumnName(string columnName);
        void AddScalar(string columName, ColumnType columnType, bool isNullable);
        void AddForeignKey(string columnName, string referencedTableName, bool isNullable);
    }
}