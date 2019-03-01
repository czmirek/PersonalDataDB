namespace PersonalDataDB
{
    using System;
    using Dapper.Contrib.Extensions;

    public class WorkUser 
    {
        [Key]
        public int ID { get; set; }
        public int? ExternalID { get; set; }
        public bool Active { get; set; }
        public string FullName { get; set; }
        public DateTime Created { get; set; }
    }
}
