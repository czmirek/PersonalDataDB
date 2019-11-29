namespace PersonalDataDB
{
    public interface IForeignKeyDefinition : INullableColumnDefinition
    {
        string TableReference { get; }
    }
}