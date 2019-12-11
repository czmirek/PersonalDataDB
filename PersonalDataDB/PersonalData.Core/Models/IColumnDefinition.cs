namespace PersonalData.Core
{
    public interface IColumnDefinition
    {
        string ID { get; }
        string? Name { get; }
        string? Description { get; }
        ColumnType ColumnType { get; }
        bool IsNullable { get; }
        string? ForeignKeyReferenceTableID { get; }
    }
}