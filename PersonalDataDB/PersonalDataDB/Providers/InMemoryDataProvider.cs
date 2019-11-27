namespace PersonalDataDB
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    public class InMemoryDataProvider : IDataProvider
    {
        private DataSet? dataSet = null;
        public void UseStructure(IEnumerable<ITableDefinition> tables)
        {
            dataSet = new DataSet();

            foreach (ITableDefinition table in tables)
            {
                DataTable dataTable = new DataTable(table.TableName);
                DataColumn pkColumn = new DataColumn()
                {
                    DataType = typeof(int),
                    ColumnName = table.PrimaryKey.ColumnName,
                    AutoIncrement = true,
                    AutoIncrementSeed = 1,
                    ReadOnly = true,
                    Unique = true
                };

                dataTable.Columns.Add(pkColumn);

                foreach (IScalarColumnDefinition sc in table.ScalarColumns)
                {
                    DataColumn scData = new DataColumn()
                    {
                        ColumnName = sc.ColumnName,
                        DataType = GetCodeType(sc.ColumnType),
                        AllowDBNull = sc.IsNullable
                    };
                    dataTable.Columns.Add(scData);
                }

                foreach (IForeignKeyDefinition fk in table.ForeignKeys)
                {
                    DataColumn fkData = new DataColumn()
                    {
                        ColumnName = fk.ColumnName,
                        DataType = typeof(int),
                        AllowDBNull = fk.IsNullable,
                    };
                    dataTable.Columns.Add(fkData);
                }

                dataSet.Tables.Add(dataTable);
            }
        }

        public object Insert(string tableName, IEnumerable<KeyValuePair<string, object?>> dictionary)
        {
            if (dataSet == null)
                throw new InvalidOperationException("InMemoryDataProvider must be initialized first with UseStructure");

            DataTable tbl = dataSet.Tables[tableName];

            DataRow dataRow = tbl.NewRow();
            foreach (KeyValuePair<string, object?> row in dictionary)
                dataRow[row.Key] = row.Value ?? DBNull.Value;

            tbl.Rows.Add(dataRow);

            return dataRow[0];
        }

        public IEnumerable<KeyValuePair<string, object?>> ReadRow(string tableName, object rowKey)
        {
            if (dataSet == null)
                throw new InvalidOperationException("InMemoryDataProvider must be initialized first with UseStructure");

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

                object? value = null;
                if (!(foundRows[0][dc] is DBNull))
                    value = foundRows[0][dc];

                returnDict.Add(dc.ColumnName, value);
            }

            return returnDict;
        }

        private Type GetCodeType(ColumnType columnType) 
            => columnType switch
            {
                ColumnType.Bool => typeof(bool),
                ColumnType.DateTime => typeof(DateTime),
                ColumnType.Decimal => typeof(decimal),
                ColumnType.Double => typeof(double),
                ColumnType.Integer => typeof(int),
                ColumnType.String => typeof(string),
                _ => throw new NotImplementedException($"Unknown column type {columnType}")
            };
    }
}