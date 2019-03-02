namespace PersonalDataDB
{
    using System.Collections.Generic;
    using System.Linq;

    public class Agreement
    {
        public int ID { get; set; }
        public Scope Scope { get; set; }
        public IEnumerable<AgreementProof> Proofs { get; set; } = Enumerable.Empty<AgreementProof>();
        public IEnumerable<LocalizedText> LocalizedText { get; set; } = Enumerable.Empty<LocalizedText>();
    }
}