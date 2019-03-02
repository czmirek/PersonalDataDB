namespace PersonalDataDB.Domain
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class PersonalDataTable
    {
        public string Name { get; }

        [JsonProperty("Columns")]
        private List<string> columns;
        
        [JsonIgnore]
        public IEnumerable<string> Columns { get => columns.AsReadOnly(); }

        public PersonalDataTable(string name, IEnumerable<string> columns)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("Personal data table name must not be empty.");

            if(columns.Any(c=>String.IsNullOrEmpty(c)))
                throw new ArgumentNullException("Column name must not be empty in the personal data table.");

            this.columns = columns.ToList();
        }
    }
}