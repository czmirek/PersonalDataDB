namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;
    using System;

    public class PersonalDataLog 
    {
        [Key]
        public int ID { get; }
        public int WorkUserID { get; set; }
        public int OwnerID { get; set; }
        public int? RowID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public DateTime Created { get; }
        public string Log { get; }
    }
}
