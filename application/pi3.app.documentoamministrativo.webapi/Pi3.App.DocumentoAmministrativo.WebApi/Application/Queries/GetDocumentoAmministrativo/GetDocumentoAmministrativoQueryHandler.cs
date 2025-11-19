// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using System.ComponentModel.DataAnnotations;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetDocumentoAmministrativo
{
    public class GetDocumentoAmministrativoQueryHandler : IRequestHandler<GetDocumentoAmministrativoQuery, GetDocumentoAmministrativoQueryResult>
    {
        #region Public Members

        public GetDocumentoAmministrativoQueryHandler(
            ILogger<GetDocumentoAmministrativoQueryHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository)
        {
            this._logger = logger;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._claimsPrincipalService = claimsPrincipalService;

            this.InitializeMapper();
        }

        public async virtual Task<GetDocumentoAmministrativoQueryResult> Handle(GetDocumentoAmministrativoQuery request, CancellationToken cancellationToken)
        {
            if (!request.Id.IsValidAggregateId())
                throw new InvalidIdDocumentPi3Exception(request.Id);

            Validator.ValidateObject(request, new ValidationContext(request), true);

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            if (!await _documentoAmministrativoRepository.Exists(idTenant!, request.Id))
                throw new DocumentoAmministrativoNotFoundPi3Exception(request.Id);

            var loadBehavior = new GetDocumentoAmministrativoLoadBehavior()
            {
                LoadAllegati = request.allegati,
                LoadClassifications = request.classificazioni,
                LoadAggregazioni = request.aggregazioni,
                LoadKeywords = request.keywords,
                LoadProfiles = request.profili,
                LoadPermissions = request.permessi,
                LoadMittentiDestinatari = request.soggetti,
                LoadNote = request.note,
                LoadVersions = request.versioni
            };

            var aggregate = await _documentoAmministrativoRepository.Get(idTenant!, request.Id, new []{ loadBehavior });

            return new GetDocumentoAmministrativoQueryResult()
            {
                DocumentoAmministrativo = this._mapper.Map<Documento>(aggregate)
            };
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocumentoAmministrativoQueryHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddDocumentoMapping();
                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, Documento>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento, OggettoDelDocumento>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Descrizione.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.ElementAggregate.Entities.ElementField, ProfileField>()
                //    .ForMember(dest => dest.ValueAsString, opt => opt.MapFrom(opt => opt.Value.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.ElementAggregate.Entities.ElementProfile, Profile>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.Entities.ContentElementClassification, Classification>()
                //    .ForMember(dest => dest.Name, opt => opt.MapFrom(opt => opt.Name.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.Entities.RelatedContentElement, RelatedElement>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.ContentElementAggregate.ValueObjects.ContentElementPermission, Permission>()
                //    .ForMember(dest => dest.MemberType, opt => opt.MapFrom(src => src.MemberType.Value.ToString()))
                //    .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType.ToString()))
                //    .ForMember(dest => dest.RightType, opt => opt.MapFrom(src => src.RightType.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentAggregate.Entities.DocumentVersion, DocumentVersion>()
                //    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.FileName : null)))
                //    .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.FileSize : null)))
                //    .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => (src.DocumentBlobRef != null ? src.DocumentBlobRef.ContentType : null)));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRegistrazioneProtocollo, DatiRegistrazione>()
                //    .ForMember(dest => dest.TipologiaRegistrazione, opt => opt.MapFrom(src => "Protocollo"))
                //    .ForMember(dest => dest.TipologiaFlusso, opt => opt.MapFrom(src => src.TipologiaFlusso.ToString()))
                //    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.NumeroProtocollo))
                //    .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.DataProtocollazione));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.DatiRegistrazioneRepertorio, DatiRegistrazione>()
                //    .ForMember(dest => dest.TipologiaRegistrazione, opt => opt.MapFrom(src => "Repertorio"))
                //    .ForMember(dest => dest.TipologiaFlusso, opt => opt.MapFrom(src => src.TipologiaFlusso.ToString()))
                //    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src => src.NumeroRegistrazione))
                //    .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.DataRegistrazione));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.IdDoc, IdDoc>()
                //    .ForMember(dest => dest.ImprontaCrittograficaDelDocumento, opt => opt.MapFrom(src => src.ImprontaCrittograficaDelDocumento.Impronta));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.AmministrazioneCheEffettuaLaRegistrazione, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Autore, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Mittente, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Destinatario, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Assegnatario, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Operatore, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ResponsabileGestioneDocumentale, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ResponsabileServizioProtocollo, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.RUP, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.SwProduttore, Soggetto>()
                //    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Allegato, Allegato>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.Fascicolo, Aggregazione>()
                //    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
                //    .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.SerieDocumentale, Aggregazione>()
                //    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
                //    .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Entities.SerieDiFascicoli, Aggregazione>()
                //    .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.GetType().Name))
                //    .ForMember(dest => dest.Denominazione, opt => opt.MapFrom(opt => opt.Denominazione.ToString()));

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.Annullamento, Annullamento>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente, ProtocolloMittente>();

                //cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza, ProtocolloEmergenza>();
            });


            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
