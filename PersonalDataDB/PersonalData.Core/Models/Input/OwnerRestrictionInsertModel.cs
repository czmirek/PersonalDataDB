namespace PersonalData.Core
{
    using System;
    public class OwnerRestrictionInsertModel : IOwnerRestriction
    {
        public OwnerRestrictionInsertModel(object ownerID, Scope scope, object? ownerRestrictionExplanationID, string? customExplanation)
        {
            this.OwnerID = ownerID ?? throw new System.ArgumentNullException(nameof(ownerID));
            this.Scope = scope ?? throw new System.ArgumentNullException(nameof(scope));
            this.OwnerRestrictionExplanationID = ownerRestrictionExplanationID;
            this.CustomExplanation = customExplanation;
        }

        public object ID => new object();

        public object OwnerID { get; private set; }

        public Scope Scope { get; private set; }

        public object? OwnerRestrictionExplanationID { get; private set; }

        public string? CustomExplanation { get; private set; }
    }
}