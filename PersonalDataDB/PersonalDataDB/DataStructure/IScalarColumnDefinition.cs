namespace PersonalDataDB
{
    public interface IScalarColumnDefinition
    {
        string ColumnName { get; }
        ColumnType ColumnType { get; }
        bool IsNullable { get; }
    }
}