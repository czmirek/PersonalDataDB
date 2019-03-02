namespace PersonalDataDB.Domain
{
    using System;

    public class Language
    {
        public Guid ID { get; }
        public string LanguageCode { get; }

        public Language(string languageCode)
        {
            if (String.IsNullOrWhiteSpace(languageCode))
                throw new ArgumentNullException("Language code cannot not be empty");

            ID = Guid.NewGuid();
            LanguageCode = languageCode;
        }
    }
}