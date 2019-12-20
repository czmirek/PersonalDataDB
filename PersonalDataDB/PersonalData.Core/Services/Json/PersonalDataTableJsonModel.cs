namespace PersonalData.Core.Services
{
    using System;
    using System.Collections.Generic;

    internal class PersonalDataTableJsonModel
    {
        public string OwnerId { get; set; } = String.Empty;
        public string Identifier { get; set; } = String.Empty;
        public string? Name { get; set; } = String.Empty;
        public string? Description  { get; set; } = String.Empty;
        public List<ColumnJsonModel> Columns { get; set; } = new List<ColumnJsonModel>();
        public List<Dictionary<string, CellJsonModel>> Rows { get; set; } = new List<Dictionary<string, CellJsonModel>>();
    }
}