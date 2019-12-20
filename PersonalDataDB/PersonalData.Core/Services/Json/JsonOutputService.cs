namespace PersonalData.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    internal class JsonOutputService
    {
        private readonly IDataProvider dataProvider;
        public JsonOutputService(IDataProvider dataProvider)
        {
            this.dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        }

        internal string CreateJson(IEnumerable<IPersonalDataTable> personalDataTables)
        {
            List<PersonalDataTableJsonModel> tables = new List<PersonalDataTableJsonModel>();
            foreach (var table in personalDataTables)
            {
                ITableDefinition tableDefinition = dataProvider.GetTable(table.TableId);
                IEnumerable<IColumnDefinition> columns = dataProvider.ListColumns(table.TableId);

                PersonalDataTableJsonModel tableJson = new PersonalDataTableJsonModel()
                {
                    Identifier = tableDefinition.ID,
                    Name = tableDefinition.Name ?? "",
                    Description = tableDefinition.Description ?? "",
                };

                if (columns.Any())
                {
                    tableJson.Columns = columns.Select(c => new ColumnJsonModel()
                    {
                        Identifier = c.ID,
                        Name = c.Name ?? "",
                        Description = c.Description ?? ""
                    }).ToList();
                }

                foreach (IPersonalDataRow row in table.PersonalDataRows)
                {
                    var jsonCells = row.PersonalDataCells.Select(c => new KeyValuePair<string, CellJsonModel>
                    (
                        key: c.ColumnId,
                        value: new CellJsonModel()
                        {
                            IsDefined = c.IsDefined,
                            Value = c.Value
                        }
                    ));
                    var jsonRow = new Dictionary<string, CellJsonModel>(jsonCells);
                    tableJson.Rows.Add(jsonRow);
                }


                tables.Add(tableJson);
            }

            return JsonSerializer.Serialize(tables, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        }
    }
}