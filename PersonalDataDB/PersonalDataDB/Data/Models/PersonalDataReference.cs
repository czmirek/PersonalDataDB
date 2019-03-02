namespace PersonalDataDB
{
    using System;
    using Dapper.Contrib.Extensions;
    
    public class PersonalDataReference 
    {
        [Key]
        public int ID { get; set; }
        public DateTime Created { get; }
        public int? OwnerID { get; set; }
        public int? RowID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public string Location { get; set; }
        public string Reference { get; set; }
        public string Url { get; set; }
        public string File { get; set; }
        public string Email { get; set; }
        public string Print { get; set; }
        public string Letter { get; set; }
    }
}