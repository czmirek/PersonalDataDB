namespace PersonalData.Core
{
    public interface IOwnerRestriction
    {
        object ID { get; }
        object OwnerID { get; }
        Scope Scope { get; }
        object? OwnerRestrictionExplanationID { get; }
        string? CustomExplanation { get; }
    }
}