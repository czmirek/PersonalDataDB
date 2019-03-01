namespace PersonalDataDB
{
    using System.Collections.Generic;
    using Dapper.Contrib.Extensions;

    public class LimitingReason 
    {
        [Key]
        public int ID { get; set; }
        public LimitingReasonType Type { get; set; }
        public ICollection<LimitingReasonText> Texts { get; set; }

    }
}
