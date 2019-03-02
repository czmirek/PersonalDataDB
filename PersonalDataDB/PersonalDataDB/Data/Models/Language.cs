namespace PersonalDataDB.Data.Models
{
    using Dapper.Contrib.Extensions;

    public class LanguageRepositoryModel
    {
        [Key]
        public int ID { get; set; }

        public string LanguageCode { get; set; }
    }
}
