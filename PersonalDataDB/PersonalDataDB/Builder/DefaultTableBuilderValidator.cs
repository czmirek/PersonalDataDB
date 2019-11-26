namespace PersonalDataDB
{
    using System;
    using System.Linq;
    internal class DefaultTableBuilderValidator
    {
        private readonly DefaultTableBuilder defaultTableBuilder;
        public DefaultTableBuilderValidator(DefaultTableBuilder defaultTableBuilder)
        {
            this.defaultTableBuilder = defaultTableBuilder;
        }

        internal void Validate()
        {
            ValidateTables();
        }

        private void ValidateTables()
        {
            foreach (TableSpecification tbl in defaultTableBuilder.Tables)
            {
                //TODO: exclude whitespace in table name
                if (String.IsNullOrWhiteSpace(tbl.TableName))
                    throw new TableBuilderException("Table name must not be empty or contain only whitespace.");

                if (defaultTableBuilder.Tables.Count(c => c.TableName == tbl.TableName) > 1)
                    throw new TableBuilderException($"Duplicate table specification \"{tbl.TableName}\".");

                //TODO: exclude whitespace in column name
                if (String.IsNullOrEmpty(tbl.Key))
                    throw new TableBuilderException($"Table key from table \"{tbl.TableName}\" must not be empty");

                if (tbl.ScalarColumns.Any(c => c.ColumnName == tbl.Key))
                    throw new TableBuilderException($"Key column name {tbl.Key} is not unique among all columns (scalars and foreign keys) in the table \"{tbl.TableName}\".");

                if (tbl.ForeignKeys.Any(c => c.ColumnName == tbl.Key))
                    throw new TableBuilderException($"Key column name {tbl.Key} is not unique among all columns (scalars and foreign keys) in the table \"{tbl.TableName}\".");

                foreach (ScalarColumnSpecification column in tbl.ScalarColumns)
                {
                    //TODO: exclude whitespace in all column names
                    //if (String.IsNullOrWhiteSpace(column.ColumnName))
                }
            }
        }
        private void ValidateColumns()
        {
            throw new NotImplementedException();
        }
    }
}