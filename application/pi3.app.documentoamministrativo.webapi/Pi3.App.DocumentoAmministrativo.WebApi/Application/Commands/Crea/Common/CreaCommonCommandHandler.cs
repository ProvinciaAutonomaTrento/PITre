// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Entrata;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Uploader;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.UploaderFS.Services.File.Uploader;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea.Common
{
    public class CreaCommonCommandHandler : IRequestHandler<CreaCommonCommand, CreaCommonCommandResponse>
    {
        private readonly ILogger<CreaEntrataCommandHandler> _logger;
        private readonly IClaimsPrincipalService _claimsPrincipalService;
        private readonly IConfigurationService _configurationService;
        private readonly IMediator _mediator;
        private readonly IDocumentoAmministrativoRepository _documentoAmministrativoRepository;
        private readonly IDocumentBlobRepository _documentBlobRepository;
        private readonly IUploaderService _uploaderService;
        private readonly IPi3DbContext _context;

        public CreaCommonCommandHandler(
            ILogger<CreaEntrataCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IConfigurationService configurationService,
            IMediator mediator,
            IDocumentoAmministrativoRepository documentoAmministrativoRepository,
            IDocumentBlobRepository documentBlobRepository,
            IUploaderService uploaderService,
            IPi3DbContext context) 
        {

            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._configurationService = configurationService;
            this._mediator = mediator;
            this._documentoAmministrativoRepository = documentoAmministrativoRepository;
            this._documentBlobRepository = documentBlobRepository;
            this._uploaderService = uploaderService;
            this._context = context;

            this.InitializeMapper();
        }

        public async Task<CreaCommonCommandResponse> Handle(CreaCommonCommand request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var tenantCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);
            var userId = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
            var groupCode = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupCode, true);

            Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo aggregate = null!;
            var idRegistro = await DecodificaEAssegnaRegistro();
            Guid? uploadId = null;
            bool wasErrors = false;

            try
            {
                aggregate = new Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo(
                    idTenant!,
                    DateTime.Now,
                    _mapper.Map<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>(request.OggettoDelDocumento),
                    new DatiRegistro()
                    {
                        IdRegistro = idRegistro.ToString(),
                        CodiceRegistro = request.CodiceRegistro
                    },
                    request.TipoDocumento);

                foreach (var classificazione in request.Classificazioni ?? Enumerable.Empty<string>())
                    await Helpers.DecodificaEAggiungiClassificazioneNonProtocollato(classificazione, _claimsPrincipalService, _context, idTenant, aggregate);

                foreach (var a in request?.Aggregazioni?.Where(a => a.Tipo == TipiAggregazioneEnum.Fascicolo) ?? new Aggregazione[0])
                    aggregate.AddAggFascicolo(a.Id);

                foreach (var a in request?.Aggregazioni?.Where(a => a.Tipo == TipiAggregazioneEnum.SerieDocumentale) ?? new Aggregazione[0])
                    aggregate.AddAggSerieDocumentale(a.Id);

                foreach (var a in request?.Aggregazioni?.Where(a => a.Tipo == TipiAggregazioneEnum.SerieDiFascicoli) ?? new Aggregazione[0])
                    aggregate.AddAggSerieDiFascicoli(a.Id);

                if (request!.IdDoc! != null!)
                {
                    uploadId = request.IdDoc.UploadId;

                    if (!await this._uploaderService.UploadExists(uploadId.Value))
                        throw new UploadIdNotFoundPi3Exception(uploadId.Value);

                    var uploadMetadata = await this._uploaderService.GetUploadMetadata(uploadId.Value);

                    DocumentBlob documentBlobAggregate = null!;

                    using (var uploadContentStream = await this._uploaderService.GetUploadedContent(uploadId.Value))
                    {
                        var fileName = Path.GetFileName(uploadMetadata.FileName);

                        documentBlobAggregate = new DocumentBlob(idTenant!,
                            uploadMetadata.FinalizationDate, new TextValue(fileName));

                        documentBlobAggregate.UploadStream(uploadContentStream, fileName);
                        documentBlobAggregate.ComputeHash(Core.AggregateModels.DocumentBlobAggregate.ValueObjects.HashNamesEnum.SHA256);

                        await this._documentBlobRepository.Add(documentBlobAggregate);
                    }

                    aggregate.AssignDocumentBlobRef(new DocumentBlobRef()
                    {
                        FileName = documentBlobAggregate.FileName,
                        FileSize = documentBlobAggregate.FileSize,
                        CreationDate = documentBlobAggregate.CreationDate,
                        ContentType = documentBlobAggregate.ContentType,
                        Hash = documentBlobAggregate.Hash,
                        HashName = Enum.Parse<HashNamesEnum>(documentBlobAggregate.HashName!.ToString()!, true),
                        IdBlob = documentBlobAggregate.Id
                    },
                    new TargetVersionBehavior()
                    {
                        CreateNewVersion = true,
                        Name = !string.IsNullOrWhiteSpace(request.IdDoc.Descrizione) ? new TextValue(request.IdDoc.Descrizione) : null,
                    });
                }

                if (request.Riservato ?? false)
                    aggregate.ChangeTipoVisibilita(TipologieVisibilitaEnum.Privata);

                if (request.Mittente != null)
                {
                    var idMittente = (await this._context.CorrGlobaliEntities.Where(corr => corr.VAR_CODICE.ToUpper() == request.Mittente.ToUpper() &&
                        corr.DTA_FINE == null).FirstOrDefaultAsync(cancellationToken: cancellationToken))!.SYSTEM_ID;
                    var mittente = new Mittente(idMittente.ToString());

                    aggregate.AssignMittente(mittente);
                }
                if (request.TipoDocumento == TipologiaFlussoEnum.E)
                {
                    foreach (var mm in request.MittentiMultipli ?? Enumerable.Empty<string>())
                        aggregate.AddMittenteMultiplo(_mapper.Map<Mittente>(mm));

                    if (request.MezzoSpedizione != null)
                    {
                        var mezzoSpedizione = await this._context.DocumentTypesEntities.Where(c => c.DESCRIPTION.ToUpper() == request.MezzoSpedizione.ToUpper()).FirstOrDefaultAsync();
                        if (mezzoSpedizione == null)
                            throw new MezzoSpedizioneNotFoundPi3Exception(request.MezzoSpedizione);

                        aggregate.AssignMezzoSpedizione(mezzoSpedizione.SYSTEM_ID.ToString());
                    }

                    if (request.ProtocolloMittente! != null!)
                        aggregate.AssignProtocolloMittente(_mapper.Map<Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente>(request.ProtocolloMittente));

                    if (request.ProtocolloEmergenza! != null!)
                        aggregate.AssignProtocolloEmergenza(_mapper.Map<Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>(request.ProtocolloEmergenza));
                }

                if (request.TipoDocumento == TipologiaFlussoEnum.I || request.TipoDocumento == TipologiaFlussoEnum.U)
                {
                    foreach (var destinatario in request.Destinatari ?? Enumerable.Empty<string>())
                    {
                        var idDestinatario = (await this._context.CorrGlobaliEntities.Where(corr => corr!.VAR_CODICE!.ToUpper() == destinatario.ToUpper() &&
                            corr.DTA_FINE == null).FirstOrDefaultAsync(cancellationToken: cancellationToken))!.SYSTEM_ID;

                        var oggetto = new Destinatario(idDestinatario.ToString());

                        aggregate.AddDestinatario(oggetto);
                    }

                    foreach (var destinatarioCc in request.DestinatariCc ?? Enumerable.Empty<string>())
                    {
                        var idDestinatario = (await this._context.CorrGlobaliEntities.Where(corr => corr.VAR_CODICE.ToUpper() == destinatarioCc.ToUpper() &&
                            corr.DTA_FINE == null).FirstOrDefaultAsync(cancellationToken: cancellationToken))!.SYSTEM_ID;

                        var oggetto = new Destinatario(idDestinatario.ToString());
                        aggregate.AddDestinatarioCc(_mapper.Map<Destinatario>(oggetto));
                    }
                }

                if (request.Profilo! != null!)
                    await Helpers.RegistraProfilo(_context, aggregate, request.Profilo);

                foreach (var k in request.Keywords ?? Enumerable.Empty<string>())
                    aggregate.AddKeyword(new TextValue(k));

                if (request.TipoDocumento != null)
                {
                    aggregate.RichiediRegistrazione(new DatiRichiestaRegistrazione()
                    {
                        Predisponi = request.Predisponi ?? false
                    });
                }

                await _documentoAmministrativoRepository.Add(aggregate);
            }
            catch
            {
                wasErrors = true;
                throw;
            }
            finally
            {
                if (!wasErrors)
                {
                    if (uploadId.HasValue)
                    {
                        try
                        {
                            await this._uploaderService.RemoveUpload(uploadId.Value);
                        }
                        catch (Pi3Exception pi3Ex)
                        {
                            this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogCritical(exception: ex, message: ex.Message);
                        }
                    }
                }
            }

            this.InitializeMapper(request.TipoDocumento);

            return _mapper.Map<CreaCommonCommandResponse>(aggregate);

            async Task<long> DecodificaEAssegnaRegistro()
            {
                var lookupCodiceRegistro = await this._context.RegistroEntities.SystemIdDaCodiceRegistro(request.CodiceRegistro);
                if (lookupCodiceRegistro == 0)
                    throw new RegistroNotFoundPi3Exception(request.CodiceRegistro);

                return lookupCodiceRegistro;
            }
        }

        #region Private Members

        private IMapper _mapper = null!;

        private void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OggettoDelDocumento, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>();

                cfg.CreateMap<Soggetto, Mittente>()
                    .ConstructUsing(src => new Mittente(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));

                cfg.CreateMap<ProtocolloMittente, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente>();

                cfg.CreateMap<ProtocolloEmergenza, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>();

                cfg.CreateMap<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaCommonCommandResponse>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.CodiceRegistro, opt => opt.MapFrom(src => src.DatiRegistrazione.CodiceRegistro ?? null))
                    .ForMember(dest => dest.Numero, opt => opt.MapFrom(src =>
                            src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                    ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).NumeroProtocollo :
                                                    ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).NumeroRegistrazione))
                    .ForMember(dest => dest.Data, opt => opt.MapFrom(src =>
                            src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                    ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).DataProtocollazione :
                                                    ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).DataRegistrazione))
                    .ForMember(dest => dest.Segnatura, opt => opt.MapFrom(src => src.IdDoc != null ? src.IdDoc.Segnatura : null));

            });


            _mapper = configuration.CreateMapper();
        }

        private void CommonMapping(IMapperConfigurationExpression cfg) 
        {
            cfg.CreateMap<OggettoDelDocumento, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.OggettoDelDocumento>();

            cfg.CreateMap<Soggetto, Mittente>()
                .ConstructUsing(src => new Mittente(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));

            //                    cfg.CreateMap<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaInternoCommandResponse>()
            cfg.CreateMap<Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaCommonCommandResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CodiceRegistro, opt => opt.MapFrom(src => src.DatiRegistrazione.CodiceRegistro))
                .ForMember(dest => dest.Numero, opt => opt.MapFrom(src =>
                        src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).NumeroProtocollo :
                                                ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).NumeroRegistrazione))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src =>
                        src.DatiRegistrazione.GetType() == typeof(DatiRegistrazioneProtocollo) ?
                                                ((DatiRegistrazioneProtocollo)src.DatiRegistrazione).DataProtocollazione :
                                                ((DatiRegistrazioneRepertorio)src.DatiRegistrazione).DataRegistrazione))
                .ForMember(dest => dest.Segnatura, opt => opt.MapFrom(src => src.IdDoc != null ? src.IdDoc.Segnatura : null));

            cfg.CreateMap<ProtocolloMittente, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloMittente>();

            cfg.CreateMap<ProtocolloEmergenza, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>();


            cfg.CreateMap<Soggetto, Destinatario>()
                .ConstructUsing(src => new Destinatario(new PG() { DenominazioneOrganizzazione = new TextValue(src.Denominazione) }, src.Id));
        }

        private void InitializeMapper(TipologiaFlussoEnum? tipo) 
        {
            var configuration = tipo switch
            {
                null => new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.DocumentoAmministrativo, CreaCommonCommandResponse>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

                }),
                TipologiaFlussoEnum.E => new MapperConfiguration(cfg =>
                {
                    CommonMapping(cfg);
                }),
                TipologiaFlussoEnum.U => new MapperConfiguration(cfg =>
                {
                    CommonMapping(cfg);

                }),
                TipologiaFlussoEnum.I => new MapperConfiguration(cfg =>
                {
                    CommonMapping(cfg);

                })
            };
            _mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
