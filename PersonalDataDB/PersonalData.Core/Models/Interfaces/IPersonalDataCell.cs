namespace PersonalData.Core
{
    public interface IPersonalDataCell
    {
        string ColumnId { get; }
        bool IsDefined { get; }
        object? Value { get; }
    }
}