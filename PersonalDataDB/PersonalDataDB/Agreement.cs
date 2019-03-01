namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class Agreement 
    {
        [Key]
        public int ID { get; set; }
        public int? OwnerID { get; set; }
        public int? RowID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
    }
}
