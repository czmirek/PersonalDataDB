namespace PersonalDataDB.Domain
{
    using System;

    public class LocalizedText
    {
        public LocalizedText(Guid languageID, string text, int order)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException("Localized text cannot be empty");

            ID = Guid.NewGuid();
            LanguageID = languageID;
            Text = text;
            Order = order;
        }

        public Guid ID { get; }
        public Guid LanguageID { get; }
        public int Order { get; }
        public string Text { get; }
    }
}