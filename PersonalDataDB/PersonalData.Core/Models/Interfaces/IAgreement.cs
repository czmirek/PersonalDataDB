using System.Collections.Generic;

namespace PersonalData.Core
{
    public interface IAgreement
    {
        object ID { get; }
        object DataManagerID { get; }
        IEnumerable<object> PurposeIDs { get; }
    }
}