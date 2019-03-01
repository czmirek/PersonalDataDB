namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class WorkUserText 
    {
        [Key]
        public int ID { get; set; }
        public int WorkUserID { get; set; }
        public int LanguageID { get; set; }
        public string Key { get; set; }
        public string Text { get; set; }
    }
}
