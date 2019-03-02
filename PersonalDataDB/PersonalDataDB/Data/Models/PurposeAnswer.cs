namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class PurposeAnswer 
    {
        [Key]
        public int ID { get; set; }
        public int PurposeQuestionID { get; set; }
        public string Answer { get; set; }
    }
}