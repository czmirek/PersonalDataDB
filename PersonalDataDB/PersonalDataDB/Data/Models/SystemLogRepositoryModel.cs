namespace PersonalDataDB.Data.Models
{
    using Dapper.Contrib.Extensions;
    using System;

    public class SystemLogRepositoryModel
    {
        [Key]
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public string Log { get; set; }
    }
}