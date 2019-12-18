using System.Collections.Generic;

namespace PersonalData.Core
{
    public interface IPersonalDataRow
    {
        object Id { get; }
        object OwnerId { get; }
        string TableId { get; }
        IEnumerable<IPersonalDataCell> PersonalDataCells { get; }
    }
}