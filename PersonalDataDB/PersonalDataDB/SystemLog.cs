namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;
    using System;

    public class SystemLog
    {
        [Key]
        public int ID { get; set; }
        public DateTime Time { get; set; }
        public string Log { get; set; }
    }
}