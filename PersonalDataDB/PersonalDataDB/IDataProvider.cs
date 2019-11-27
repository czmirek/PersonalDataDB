using System.Collections.Generic;

namespace PersonalDataDB
{
    public interface IDataProvider
    {
        void UseStructure(IEnumerable<ITableDefinition> tables);
        object Insert(string tableName, IEnumerable<KeyValuePair<string, object?>> dictionary);
        IEnumerable<KeyValuePair<string, object?>> ReadRow(string tableName, object rowKey);
    }
}