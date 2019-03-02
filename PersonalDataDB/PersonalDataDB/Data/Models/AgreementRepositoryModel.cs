namespace PersonalDataDB.Data.Models
{
    using Dapper.Contrib.Extensions;
    using System.Collections.Generic;

    public class AgreementRepositoryModel
    {
        [Key]
        public int ID { get; set; }
        public int? OwnerID { get; set; }
        public int? RowID { get; set; }
        public string Table { get; set; }
        public string Column { get; set; }
        public IEnumerable<AgreementProofRepositoryModel> Proofs { get; set; }
        public IEnumerable<AgreementTextRepositoryModel> Texts { get; set; }
    }
}
