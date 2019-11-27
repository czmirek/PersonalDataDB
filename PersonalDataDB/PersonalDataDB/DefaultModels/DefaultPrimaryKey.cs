namespace PersonalDataDB
{
    internal class DefaultPrimaryKey : IPrimaryKeyDefinition
    {
        public string ColumnName { get; private set; }

        public DefaultPrimaryKey(string columnName)
        {
            if (columnName.IsEmptyOrWhiteSpace())
                throw new TableBuilderException($"Key \"{columnName}\" must not be empty or contain whitespace");

            ColumnName = columnName;
        }
    }
}