namespace PersonalDataDB.Data.Models
{
    using System;

    public class AgreementProofRepositoryModel
    {
        public int ID { get; set; }
        public int AgreementID { get; set; }
        public int? OwnerID { get; set; }
        public DateTime Created { get; set; }
        public string Proof { get; set; }
    }
}
