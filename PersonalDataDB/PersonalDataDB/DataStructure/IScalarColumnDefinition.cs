namespace PersonalDataDB
{
    public interface IScalarColumnDefinition : INullableColumnDefinition
    {
        ColumnType ColumnType { get; }
    }
}