namespace PersonalDataDB
{
    using System.Collections.Generic;

    public class PersonalDataDB
    {
        private IDataProvider dataProvider;
        private readonly ITableSet tableSet;

        internal PersonalDataDB(IDataProvider dataProvider, ITableSet tableSet)
        {
            this.dataProvider = dataProvider;
            this.tableSet = tableSet;
        }

        public object Insert(string tableName, IDictionary<string, object?> data)
        {
            //todo: použít validátor
            return dataProvider.Insert(tableName, data);
        }

        public IDictionary<string, object?> ReadRow(string tableName, object rowKey)
        {
            return dataProvider.ReadRow(tableName, rowKey);
        }

        public IDictionary<string, object?> ReadRow(string tableName, object rowKey, IEnumerable<string> requiredColumns)
        {
            return dataProvider.ReadRow(tableName, rowKey, requiredColumns);
        }

        public object? ReadValue (string tableName, object rowKey, string column)
        {
            return dataProvider.ReadValue(tableName, rowKey, column);
        }

        public void Update(string tableName, object rowKey, string column, object? value)
            => Update(tableName, rowKey, new Dictionary<string, object?>() { { column, value } });

        public void Update(string tableName, object rowKey, IDictionary<string, object?> data)
            => dataProvider.UpdateRow(tableName, rowKey, data);
    }
}