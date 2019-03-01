namespace PersonalDataDB
{
    using Dapper.Contrib.Extensions;

    public class Configuration
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public string AdministratorFullName { get; set; }
        public string AdministratorOffice { get; set; }
        public string AdministratorPhone { get; set; }
        public string AdministratorEmail { get; set; }
    }
}
