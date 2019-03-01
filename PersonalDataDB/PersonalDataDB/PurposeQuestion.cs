namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class PurposeQuestion 
    {
        [Key]
        public int ID { get; set; }
        public int PurposeID { get; set; }
        public int LanguageID { get; set; }
        public int? DefaultPurposeAnswerID { get; set; }
        public int Order { get; set; }
        public string QuestionFromOwnerPerspective { get; set; }
    }
}
