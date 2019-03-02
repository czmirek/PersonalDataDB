namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class LimitingReasonText 
    {
        [Key]
        public int ID { get; set; }
        public int LimitingReasonID { get; set; }
        public int LanguageID { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}
