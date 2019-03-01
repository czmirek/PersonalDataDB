namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class Language 
    {
        [Key]
        public int ID { get; set; }

        public string LanguageCode { get; set; }
    }
}
