namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class InMemoryDataProvider : IDataProvider
    {
        private DataSet dataSet;

        public InMemoryDataProvider(ITableSet tableSet)
        {
            var converter = new TableSetConverter();
            this.dataSet = converter.Convert(tableSet);
        }
        
        public object Insert(string tableName, IDictionary<string, object?> data)
        {
            DataTable tbl = dataSet.Tables[tableName];

            DataRow dataRow = tbl.NewRow();
            foreach (KeyValuePair<string, object?> row in data)
                dataRow[row.Key] = row.Value ?? DBNull.Value;

            tbl.Rows.Add(dataRow);

            return dataRow[0];
        }
        public void UpdateRow(string tableName, object rowKey, IDictionary<string, object?> data)
        {
            DataTable dt = dataSet.Tables[tableName];
            string pkName = dt.Columns[0].ColumnName;

            DataRow[] foundRows = dt.Select($"{pkName} = {rowKey}");

            if (foundRows.Length == 0)
                return;

            foreach (KeyValuePair<string, object?> updateData in data)
                foundRows[0][updateData.Key] = updateData.Value;
        }


        public IDictionary<string, object?> ReadRow(string tableName, object rowKey)
        {
            DataTable dt = dataSet.Tables[tableName];
            string pkName = dt.Columns[0].ColumnName;
            DataRow[] foundRows = dt.Select($"{pkName} = {rowKey}");

            var returnDict = new Dictionary<string, object?>();
            
            if (foundRows.Length == 0)
                return returnDict;


            foreach (DataColumn? dc in dt.Columns)
            {
                if (dc == null)
                    continue;

                returnDict.Add(dc.ColumnName, GetDbValue(foundRows[0][dc]));
            }

            return returnDict;
        }


        public IDictionary<string, object?> ReadRow(string tableName, object rowKey, IEnumerable<string> requiredColumns)
        {
            DataTable dt = dataSet.Tables[tableName];
            string pkName = dt.Columns[0].ColumnName;
            var returnDictionary = new Dictionary<string, object?>();
            
            DataRow[] foundRows = dt.Select($"{pkName} = {rowKey}");

            foreach (DataColumn? dc in dt.Columns)
            {
                if (dc == null)
                    continue;

                if (requiredColumns.Contains(dc.ColumnName))
                    returnDictionary.Add(dc.ColumnName, GetDbValue(foundRows[0][dc]));
            }

            return returnDictionary;
        }

        public object? ReadValue(string tableName, object rowKey, string column)
        {
            DataTable dt = dataSet.Tables[tableName];
            string pkName = dt.Columns[0].ColumnName;
            DataRow[] foundRows = dt.Select($"{pkName} = {rowKey}");

            return GetDbValue(foundRows[0][column]);
        }

        private object? GetDbValue(object dbValue)
        {
            if (dbValue is DBNull)
                return null;
            else
                return dbValue;
        }
    }
}