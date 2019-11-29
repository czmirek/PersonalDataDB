namespace PersonalDataDB
{
    public interface INullableColumnDefinition : IColumnDefinition
    {
        bool IsNullable { get; }
    }
}