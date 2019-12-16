namespace PersonalData.Core
{
    public class PurposeInsertModel : IPurpose
    {
        public PurposeInsertModel(string name, string description, PurposeType type, Scope purposeScope)
        {
            this.Name = name;
            this.Description = description;
            this.Type = type;
            this.PurposeScope = purposeScope;
        }

        public object ID => new object();

        public string Name { get; set; }

        public string Description { get; set; }

        public PurposeType Type { get; set; }

        public Scope PurposeScope { get; set; }
    }
}