namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class Owner 
    {
        [Key]
        public int ID { get; set; }
        public int? ExternalID { get; set; }
    }
}
