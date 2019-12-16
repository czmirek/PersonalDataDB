namespace PersonalData.Core
{
    using System.Collections.Generic;
    public interface IAgreementTemplate
    {
        object ID { get; }
        object DataManagerID { get; }
        string Name { get; }
        string Content { get; }
        IEnumerable<object> PurposeIDs { get; }
    }
}