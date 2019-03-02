namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class Purpose 
    {
        [Key]
        public int ID { get; set; }
        public PurposeType Type { get; set; }
        public int? OwnerID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public int? RowID { get; set; }
    }
}
