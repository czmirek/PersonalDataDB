namespace PersonalData.Core.Extensions
{
    public interface ITableBuilder
    {
        string? Name { get; set; }
        string? Description { get; set; }
        void AddColumn(string columName, ColumnType columnType, bool isNullable, string? name = null, string? description = null);
        void AddForeignKeyColumn(string columnName, string referencedTableName, bool isNullable, string? name = null, string? description = null);
    }
}