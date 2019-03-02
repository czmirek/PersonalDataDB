namespace PersonalDataDB.Domain
{
    using Newtonsoft.Json;
    using System.Linq;
    using System;
    using System.Collections.Generic;

    public class PersonalDataDatabaseDomain
    {
        private readonly IPersonalDataRepository personalDataRepository;

        public string PersonalDataResponsibleContact { get; set; }
        
        public PersonalDataDatabaseDomain(string personalDataResponsibleContact, string defaultLanguageCode, IPersonalDataRepository personalDataRepository)
        {
            if (String.IsNullOrEmpty(personalDataResponsibleContact))
                throw new InvalidOperationException("Personal data responsible contact must not be empty.");

            this.personalDataRepository = personalDataRepository ?? throw new ArgumentNullException(nameof(personalDataRepository));
            PersonalDataResponsibleContact = personalDataResponsibleContact;
            AddLanguage(defaultLanguageCode);
        }

        public void AddLanguage(string languageCode)
        {
            if (languages.Any(l => l.LanguageCode.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase)))
                throw new InvalidOperationException($"Language with code {languageCode} already exists");

            languages.Add(new Language(languageCode));
        }

        /// <summary>
        /// Adds a new agreement from the perspective of the database administrator.
        /// </summary>
        /// <param name="newAgreement">New agreement.</param>
        public void AddAgreement(Agreement newAgreement)
        {
            if (!languages.Any())
                throw new InvalidOperationException("An agreement must contain at least one text for all languages in the database but there are no languages specified.");

            foreach (Language lang in languages)
            {
                if (newAgreement.Texts.Any(l => l.LanguageID == lang.ID))
                    throw new InvalidOperationException($"Please specify at least a single text for all languages in the new agreement. Text is missing for language \"{lang.LanguageCode}\".");
            }

            if (NumberOfRequiredAgreementProofs > 0 && newAgreement.Proofs.Count() < NumberOfRequiredAgreementProofs)
                throw new InvalidOperationException($"At least {NumberOfRequiredAgreementProofs} proofs are required in the agreement.");

            if (RequireOwnerInAgreementProofs && newAgreement.Scope.OwnerID == null)
                throw new InvalidOperationException($"Global agreement is forbidden, please specify OwnerID in new agreement.");

            agreements.Add(newAgreement);
        }

        /// <summary>
        /// Removes an existing agreement from the perspective of the database administrator. 
        /// Throws an exception if agreement does not exist or if there is personal data covered
        /// by this agreement only.
        /// </summary>
        /// <param name="agreementID">ID of the agreement</param>
        /// <param name="force">If true, removes all corresponding personal data which are covered only by this agreement.</param>
        public void RemoveAgreement(Guid agreementID, bool force = false)
        {
            if (!agreements.Any(a => a.ID == agreementID))
                throw new InvalidOperationException("Unable to remove, agreement does not exist.");



            //zakázat, pokud tady už existují nějaké osobní údaje
            //nebo zohlednit force
            throw new NotImplementedException();
        }

        public void AddPersonalDataTable(PersonalDataTable newTable)
        {
            if (tables.Any(t => t.Name.Equals(newTable.Name, StringComparison.Ordinal)))
                throw new InvalidOperationException($"Table {newTable.Name} is already specified.");

            tables.Add(newTable);
        }

        public void AddPersonalDataColumn(string tableName, string newColumn)
        {
            PersonalDataTable table = tables.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.Ordinal));
            if (table == null)
                throw new InvalidOperationException($"Table {tableName} is already specified.");

            var newTable = new PersonalDataTable(table.Name, table.Columns.Union(new string[] { newColumn }));
            tables.Remove(table);
            tables.Add(newTable);
        }

        [JsonProperty("Tables")]
        private List<PersonalDataTable> tables = new List<PersonalDataTable>();

        [JsonProperty("Languages")]
        private List<Language> languages = new List<Language>();

        [JsonProperty("Agreements")]
        private List<Agreement> agreements = new List<Agreement>();

        [JsonIgnore]
        public IEnumerable<PersonalDataTable> Tables { get => tables.AsReadOnly(); }

        [JsonIgnore]
        public IEnumerable<Language> Languages { get => languages.AsReadOnly();  }

        [JsonIgnore]
        public IEnumerable<Agreement> Agreements { get => agreements.AsReadOnly(); }

        private int numberOfRequiredAgreementProofs = 0;
        public int NumberOfRequiredAgreementProofs
        {
            get => numberOfRequiredAgreementProofs;
            set
            {
                throw new NotImplementedException();
            }
        }

        private bool requireOwnerInAgreementProofs = false;
        public bool RequireOwnerInAgreementProofs
        {
            get => requireOwnerInAgreementProofs;
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
