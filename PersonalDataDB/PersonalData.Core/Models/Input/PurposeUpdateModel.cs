namespace PersonalData.Core
{
    public class PurposeUpdateModel : IPurpose
    {
        public PurposeUpdateModel(object purposeId, string name, string description, PurposeType type, Scope purposeScope)
        {
            this.ID = purposeId;
            this.Name = name;
            this.Description = description;
            this.Type = type;
            this.PurposeScope = purposeScope;
        }

        public object ID { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public PurposeType Type { get; private set; }
        public Scope PurposeScope { get; private set; }
    }
}