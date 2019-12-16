using System.Collections.Generic;

namespace PersonalData.Core
{
    public sealed class AgreementTemplateInsertModel : IAgreementTemplate
    {
        public AgreementTemplateInsertModel(object dataManagerID, string name, string content, IEnumerable<object> purposeIDs)
        {
            this.DataManagerID = dataManagerID ?? throw new System.ArgumentNullException(nameof(dataManagerID));
            this.Name = name ?? throw new System.ArgumentNullException(nameof(name));
            this.Content = content ?? throw new System.ArgumentNullException(nameof(content));
            this.PurposeIDs = purposeIDs ?? throw new System.ArgumentNullException(nameof(purposeIDs));
        }

        public object ID => new object();

        public object DataManagerID { get; private set; }

        public string Name { get; private set; }

        public string Content { get; private set; }

        public IEnumerable<object> PurposeIDs { get; private set; }
    }
}