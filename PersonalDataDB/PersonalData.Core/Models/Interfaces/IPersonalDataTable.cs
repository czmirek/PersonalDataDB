using System.Collections.Generic;

namespace PersonalData.Core
{
    public interface IPersonalDataTable
    {
        object OwnerId { get; }
        string TableId { get; }
        IEnumerable<IPersonalDataRow> PersonalDataRows { get; }
    }
}