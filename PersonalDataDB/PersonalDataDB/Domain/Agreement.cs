namespace PersonalDataDB.Domain
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Agreement
    {
        public Agreement(Scope scope, IEnumerable<LocalizedText> texts, IEnumerable<AgreementProof> proofs = null)
        {
            ID = Guid.NewGuid();
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));

            if (!texts.Any())
                throw new InvalidOperationException("Agreement must contain at least a single text for each language.");

            this.proofs = proofs?.ToList() ?? new List<AgreementProof>();
            this.texts = texts.ToList();
        }

        public Guid ID { get; }

        public Scope Scope { get; }

        [JsonProperty("Proofs")]
        private readonly List<AgreementProof> proofs = new List<AgreementProof>();

        [JsonProperty("Texts")]
        private readonly List<LocalizedText> texts = new List<LocalizedText>();

        [JsonIgnore]
        public IEnumerable<AgreementProof> Proofs { get => proofs.AsReadOnly(); }

        [JsonIgnore]
        public IEnumerable<LocalizedText> Texts { get => texts.AsReadOnly(); }
    }
}