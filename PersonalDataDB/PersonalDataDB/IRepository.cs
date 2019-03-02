namespace PersonalDataDB
{
    using System.Collections.Generic;
    using PersonalDataDB.Data.Models;


    public interface IRepository
    {
        IEnumerable<AgreementRepositoryModel> GetAgreements();
        void AddAgreement(AgreementRepositoryModel agreementRepositoryModel);
        void RemoveAgreement(int agreementID);

        IEnumerable<LanguageRepositoryModel> GetLanguages();
        bool IsRepositoryInitialized();

        void AddLanguage(LanguageRepositoryModel languageRepositoryModel);

        T GetConfiguration<T>(string key);
        ConfigurationRepositoryModel GetConfiguration();
        void SaveConfiguration(ConfigurationRepositoryModel model);
        void SaveConfiguration<T>(string key, T value);
        void AddSystemLog(SystemLogRepositoryModel logModel);
        void AddAgreementProof(AgreementProofRepositoryModel dbModel);
        IEnumerable<AgreementProofRepositoryModel> GetAgreementProofs();
        void RemoveAgreementProof(int agreementProofID);
    }
}
