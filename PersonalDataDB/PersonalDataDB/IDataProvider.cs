using System.Collections.Generic;

namespace PersonalDataDB
{
    public interface IDataProvider
    {
        object Insert(string tableName, IDictionary<string, object?> data);
        void UpdateRow(string tableName, object rowKey, IDictionary<string, object?> data);
        IDictionary<string, object?> ReadRow(string tableName, object rowKey);
        IDictionary<string, object?> ReadRow(string tableName, object rowKey, IEnumerable<string> requiredColumns);
        object? ReadValue(string tableName, object rowKey, string column);

    }
}