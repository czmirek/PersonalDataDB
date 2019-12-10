using PersonalData.Core;

namespace PersonalData.Test
{
    public class ConfigurationMock : IConfiguration
    {
        public bool AllowPurposeChoiceOnAgreementCreation { get; set; }
    }
}