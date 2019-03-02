namespace PersonalDataDB
{
    using AutoMapper;
    using PersonalDataDB.Data.Models;
    using System;

    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ConfigurationRepositoryModel, Configuration>();
            CreateMap<Configuration, ConfigurationRepositoryModel>();

            CreateMap<LanguageRepositoryModel, Language>();
            CreateMap<Language, LanguageRepositoryModel>();

            CreateMap<AgreementRepositoryModel, Agreement>()
                .ForMember(m => m.Scope, opts => opts.MapFrom(src => CreateScopeFromDbValues(src.OwnerID, src.RowID, src.Table, src.Column)));

            CreateMap<Agreement, AgreementRepositoryModel>()
                .ForMember(m => m.OwnerID, opts => opts.MapFrom(src => src.Scope.OwnerID))
                .ForMember(m => m.RowID, opts => opts.MapFrom(src => src.Scope.RowID))
                .ForMember(m => m.Table, opts => opts.MapFrom(src => src.Scope.Table))
                .ForMember(m => m.Column, opts => opts.MapFrom(src => src.Scope.Column));

            CreateMap<AgreementTextRepositoryModel, LocalizedText>();
            CreateMap<AgreementProofRepositoryModel, AgreementProof>();
            CreateMap<AgreementProof, AgreementProofRepositoryModel>();

            /*
             AgreementRepositoryModel dbAgr = new AgreementRepositoryModel()
            {
                ID = 0,
                Column = newAgreement.Scope.Column,
                OwnerID = newAgreement.Scope.OwnerID,
                RowID = newAgreement.Scope.RowID,
                Table = newAgreement.Scope.Table,
                Proofs = newAgreement.Proofs.Select(p => new AgreementProofRepositoryModel()
                {
                    ID = 0,
                    AgreementID = 0,
                    Created = DateTimeProvider.Now,
                    Proof = p.Proof
                }),
                Texts = newAgreement.LocalizedText.Select(t => new AgreementTextRepositoryModel()
                {
                    ID = 0,
                    AgreementID = 0,
                    LanguageID = t.LanguageID,
                    Order = t.Order,
                    Text = t.Text
                })
            };
             */
        }

        private Scope CreateScopeFromDbValues(int? ownerID, int? rowID, string table, string column)
        {
            if (ownerID == null)
                return Scope.GlobalScope;

            if (rowID == null && table == null && column == null)
                return Scope.CreateOwnerScope(ownerID.Value);

            if (rowID == null && table == null && column != null)
                return Scope.CreateColumnScope(ownerID.Value, column);

            if (rowID == null && table != null && column != null)
                return Scope.CreateColumnScope(ownerID.Value, table, column);

            if (rowID != null && table != null && column != null)
                return Scope.CreateCellScope(ownerID.Value, table, column, rowID.Value);

            throw new InvalidOperationException("Invalid scope data");
        }
    }
}