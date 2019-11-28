namespace PersonalDataDB
{
    public interface IForeignKeyDefinition : IColumnDefinition
    {
        string TableReference { get; }
        bool IsNullable { get; }
    }
}