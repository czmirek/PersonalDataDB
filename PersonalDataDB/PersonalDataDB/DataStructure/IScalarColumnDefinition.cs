namespace PersonalDataDB
{
    public interface IScalarColumnDefinition : IColumnDefinition
    {
        ColumnType ColumnType { get; }
        bool IsNullable { get; }
    }
}