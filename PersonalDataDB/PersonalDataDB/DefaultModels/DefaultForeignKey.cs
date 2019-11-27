using System;

namespace PersonalDataDB
{
    internal class DefaultForeignKey : IForeignKeyDefinition
    {
        public DefaultForeignKey(string columnName, string tableReference, bool isNullable)
        {
            if (columnName.IsEmptyOrWhiteSpace())
                throw new TableBuilderException($"Column \"{columnName}\" must not be empty or contain whitespace");

            this.ColumnName = columnName;
            this.TableReference = tableReference;
            this.IsNullable = isNullable;
        }

        public string ColumnName { get; private set; }
        public string TableReference { get; private set; }
        public bool IsNullable { get; private set; }
    }
}