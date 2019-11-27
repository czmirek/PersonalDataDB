using System;
using System.Collections.Generic;

namespace PersonalDataDB
{
    public class PersonalDataDB
    {
        private IDataProvider dataProvider;
        internal PersonalDataDB(IDataProvider dataProvider, IEnumerable<ITableDefinition> dataStructure)
        {
            this.dataProvider = dataProvider;
        }

        public object Insert(string tableName, IEnumerable<KeyValuePair<string, object?>> dictionary)
        {
            return dataProvider.Insert(tableName, dictionary);
        }

        public IEnumerable<KeyValuePair<string, object?>> ReadRow(string tableName, object rowKey)
        {
            return dataProvider.ReadRow(tableName, rowKey);
        }
    }
}