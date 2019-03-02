namespace PersonalDataDB.Data.Models
{
    using Dapper.Contrib.Extensions;

    public class AgreementTextRepositoryModel 
    {
        [Key]
        public int ID { get; set; }
        public int AgreementID { get; set; }
        public int LanguageID { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}
