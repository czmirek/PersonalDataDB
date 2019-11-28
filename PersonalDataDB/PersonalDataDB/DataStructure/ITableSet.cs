namespace PersonalDataDB
{
    using System.Collections.Generic;
    public interface ITableSet
    {
        IEnumerable<ITableDefinition> Tables { get; }
        ITableDefinition this[string tableName] { get; }
    }
}
