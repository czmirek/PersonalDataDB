namespace PersonalData.Core
{
    using System.Collections.Generic;
    public sealed class AgreementTemplateUpdateModel : IAgreementTemplate
    {
        public AgreementTemplateUpdateModel(object agreementTemplateId, object dataManagerID, string name, string content, IEnumerable<object> purposeIDs)
        {
            this.ID = agreementTemplateId;
            this.DataManagerID = dataManagerID;
            this.Name = name;
            this.Content = content;
            this.PurposeIDs = purposeIDs;
        }

        public object ID { get; private set; }

        public object DataManagerID { get; private set; }

        public string Name { get; private set; }

        public string Content { get; private set; }

        public IEnumerable<object> PurposeIDs { get; private set; }
    }
}