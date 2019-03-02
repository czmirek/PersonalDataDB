namespace PersonalDataDB.Data.Models
{
    public class ConfigurationRepositoryModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }

        public string AdministratorFullName { get; set; }
        public string AdministratorOffice { get; set; }
        public string AdministratorPhone { get; set; }
        public string AdministratorEmail { get; set; }

        public string PersonalDataResponsibleContact { get; set; }
        public bool ConfigurationIsSet { get; set; }
        public int NumberOfRequiredAgreementProofs { get; set; }
        public bool RequireOwnerInAgreementProofs { get; set; }
    }
}
