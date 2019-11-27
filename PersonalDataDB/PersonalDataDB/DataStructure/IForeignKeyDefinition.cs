namespace PersonalDataDB
{
    public interface IForeignKeyDefinition
    {
        string ColumnName { get; }
        string TableReference { get; }
        bool IsNullable { get; }
    }
}