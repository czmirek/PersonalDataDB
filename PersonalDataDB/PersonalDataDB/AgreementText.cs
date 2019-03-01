namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class AgreementText 
    {
        [Key]
        public int ID { get; set; }
        public int AgreementID { get; set; }
        public int LanguageID { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
    }
}
