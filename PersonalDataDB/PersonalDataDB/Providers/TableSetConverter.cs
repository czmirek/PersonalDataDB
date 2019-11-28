namespace PersonalDataDB
{
    using System;
    using System.Data;
    internal class TableSetConverter
    {
        public DataSet Convert(ITableSet tableSet)
        {
            DataSet dataSet = new DataSet();

            foreach (ITableDefinition table in tableSet.Tables)
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
            return dataSet;
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
