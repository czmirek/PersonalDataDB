namespace PersonalData.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class SchemaValidator
    {
        public void Validate(ISchema schema)
        {
            if (!schema.Tables.Any())
                throw new InitializationException("At least one table must be defined in the schema");

            IEnumerable<string> nonUniqueTables = schema.Tables.GroupBy(t => t.ID)
                                                               .Where(t => t.Count() > 1)
                                                               .Select(t => t.Key);

            if (nonUniqueTables.Any())
            {
                string tableNames = String.Join(", ", nonUniqueTables);
                throw new InitializationException($"Tables {tableNames} are not unique");
            }


            IEnumerable<string> tableIDs = schema.Tables.Select(t => t.ID);

            foreach (ITableDefinition table in schema.Tables)
            {
                if(!table.Columns.Any())
                    throw new InitializationException($"Table {table.ID} does not define any columns");

                IEnumerable<string> nonUniqueColumns = table.Columns.GroupBy(c => c.ID)
                                                                    .Where(c => c.Count() > 1)
                                                                    .Select(c => c.Key);

                if(nonUniqueColumns.Any())
                {
                    string columnNames = String.Join(", ", nonUniqueColumns);
                    throw new InitializationException($"Columns {columnNames} in table {table.ID} are not unique");
                }

                if (table.Columns.Any(c => c.ID.Equals(Constants.OwnerIDColumnIdentifier, StringComparison.InvariantCultureIgnoreCase)))
                    throw new InitializationException($"Table {table.ID} must not define reserved column identifier {Constants.OwnerIDColumnIdentifier}");

                if (table.Columns.Any(c => c.ID.Equals(Constants.TableIDColumnIdentifier, StringComparison.InvariantCultureIgnoreCase)))
                    throw new InitializationException($"Table {table.ID} must not define reserved column identifier {Constants.TableIDColumnIdentifier}");

                foreach (IColumnDefinition fkColumn in table.Columns.Where(c => c.ColumnType == ColumnType.ForeignKey))
                {
                    if(fkColumn.ForeignKeyReferenceTableID == null)
                        throw new InitializationException($"Foreign key column {fkColumn.ID} does not contain a table reference");

                    if (!tableIDs.Contains(fkColumn.ForeignKeyReferenceTableID))
                        throw new InitializationException($"Foreign key column {fkColumn.ID} refers to a non-existing table {fkColumn.ForeignKeyReferenceTableID}");
                }
            }
        }
    }
}