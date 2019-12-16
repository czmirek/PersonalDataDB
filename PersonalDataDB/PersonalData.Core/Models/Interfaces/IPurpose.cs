namespace PersonalData.Core
{
    public interface IPurpose
    {
        object ID { get; }
        string Name { get; }
        string Description { get; }
        PurposeType Type { get; }
        Scope PurposeScope { get; }
    }
}