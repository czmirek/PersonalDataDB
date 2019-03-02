namespace PersonalDataDB.Domain
{
    using System;

    public class AgreementProof
    {
        public AgreementProof(DateTime created, string proof, int? ownerID)
        {
            if (String.IsNullOrWhiteSpace(proof))
                throw new ArgumentNullException("Proof content cannot be empty");

            Created = created;
            OwnerID = ownerID;
            Proof = proof;
        }

        public int? OwnerID { get; }
        public DateTime Created { get; }
        public string Proof { get; }
    }
}