using System.Collections.Generic;

namespace PersonalData.Core
{
    public interface ITableDefinition
    {
        string ID { get; }
        string Name { get; }
        string Description { get; }
        IEnumerable<IColumnDefinition> Columns { get; }
    }
}