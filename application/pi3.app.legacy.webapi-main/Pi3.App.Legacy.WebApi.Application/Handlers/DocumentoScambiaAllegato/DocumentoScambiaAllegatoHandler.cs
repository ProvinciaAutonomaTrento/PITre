// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoScambiaAllegatoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoScambiaAllegato;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoScambiaAllegato
{
    public class DocumentoScambiaAllegatoHandler : IRequestHandler<DocumentoScambiaAllegatoRequest, DocumentoScambiaAllegatoResult>
    {
        #region Public Members

        public DocumentoScambiaAllegatoHandler(ILogger<DocumentoScambiaAllegatoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._sessionRepositoryService = sessionRepositoryService;

            this.InitializeMapper();
        }

        public async Task<DocumentoScambiaAllegatoResult> Handle(DocumentoScambiaAllegatoRequest request, CancellationToken cancellationToken)
        {
            var result = true;

            try
            {
                if(request.documento != null && !string.IsNullOrEmpty(request.documento.docNumber))
                {
                    var docnumberDocumentoAsLong = request.documento.docNumber.AsLong();
                    var versionIdDocumentoAsLong = request.documento.versionId.AsLong();
                    var docnumberAllegatoAsLong = request.allegato.docNumber.AsLong();
                    var versionIdAllegatoAsLong = request.allegato.versionId.AsLong();

                    var profileDocumentoEntity = await _dbContext.ProfileEntities
                        .Where(p => p.DOCNUMBER == docnumberDocumentoAsLong)
                        .FirstAsync();

                    var profileAllegatoEntity = await _dbContext.ProfileEntities
                        .Where(p => p.DOCNUMBER == docnumberAllegatoAsLong)
                        .FirstAsync();

                    //Controllo se consolidato
                    if (profileDocumentoEntity.CONSOLIDATION_STATE == DocumentConsolidationStateEnum.Step1.ToString())
                        throw new DocumentoConsolidatoPi3Exception(docnumberDocumentoAsLong.ToString());

                    var versionDocumentoEntity = await _dbContext.VersionEntities.Where(v => v.VERSION_ID == versionIdDocumentoAsLong).FirstAsync();
                    var componentDocumentoEntity = await _dbContext.ComponentEntities.Where(c => c.VERSION_ID == versionIdDocumentoAsLong).FirstAsync();
                    var versionAllegatoEntity = await _dbContext.VersionEntities.Where(v => v.VERSION_ID == versionIdAllegatoAsLong).FirstAsync();
                    var componentAllegatoEntity = await _dbContext.ComponentEntities.Where(c => c.VERSION_ID == versionIdAllegatoAsLong).FirstAsync();
                   
                    var versionsScambiaDocumento = _mapper.Map<VersionsScambiaEntity>(versionDocumentoEntity);
                    var componentsScambiaDocumento = _mapper.Map<ComponentsScambiaEntity>(componentDocumentoEntity);
                    var versionsScambiaAllegato= _mapper.Map<VersionsScambiaEntity>(versionAllegatoEntity);
                    var componentsScambiaAllegato = _mapper.Map<ComponentsScambiaEntity>(componentAllegatoEntity);

                    //Aggiornamento record VERSIONS per l'allegato con i dati del documento
                    versionAllegatoEntity.SUBVERSION = versionsScambiaDocumento.SUBVERSION;
                    versionAllegatoEntity.CARTACEO = versionsScambiaDocumento.CARTACEO;
                    versionAllegatoEntity.SCARTA_FASC_CARTACEA = versionsScambiaDocumento.SCARTA_FASC_CARTACEA;

                    //Aggiornamento record VERSIONS per il documento con i dati dell'allegato
                    versionDocumentoEntity.SUBVERSION = versionsScambiaAllegato.SUBVERSION;
                    versionDocumentoEntity.CARTACEO = versionsScambiaAllegato.CARTACEO;
                    versionDocumentoEntity.SCARTA_FASC_CARTACEA = versionsScambiaAllegato.SCARTA_FASC_CARTACEA;

                    //Aggiornamento record COMPONENTS per l'allegato con i dati del documento
                    componentAllegatoEntity.PATH = componentsScambiaDocumento.PATH;
                    componentAllegatoEntity.EXT = componentDocumentoEntity.EXT;
                    componentAllegatoEntity.FILE_SIZE = componentsScambiaDocumento.FILE_SIZE;
                    componentAllegatoEntity.VAR_IMPRONTA = componentsScambiaDocumento.VAR_IMPRONTA;
                    componentAllegatoEntity.CHA_FIRMATO = componentsScambiaDocumento.CHA_FIRMATO;
                    componentAllegatoEntity.VAR_NOMEORIGINALE = componentsScambiaDocumento.VAR_NOMEORIGINALE;
                    componentAllegatoEntity.FILE_INFO = componentsScambiaDocumento.FILE_INFO;
                    componentAllegatoEntity.ID_PEOPLE_PUTFILE = componentsScambiaDocumento.ID_PEOPLE_PUTFILE;
                    componentAllegatoEntity.ID_PEOPLE_DELEGATO_PUTFILE = componentsScambiaDocumento.ID_PEOPLE_DELEGATO_PUTFILE;
                    componentAllegatoEntity.DTA_FILE_ACQUIRED = componentsScambiaDocumento.DTA_FILE_ACQUIRED;
                    componentAllegatoEntity.CHA_TIPO_FIRMA = componentsScambiaDocumento.CHA_TIPO_FIRMA;

                    //Aggiornamento record COMPONENTS per il documento con i dati dell'allegato
                    componentDocumentoEntity.PATH = componentsScambiaAllegato.PATH;
                    componentDocumentoEntity.EXT = componentsScambiaAllegato.EXT;
                    componentDocumentoEntity.FILE_SIZE = componentsScambiaAllegato.FILE_SIZE;
                    componentDocumentoEntity.VAR_IMPRONTA = componentsScambiaAllegato.VAR_IMPRONTA;
                    componentDocumentoEntity.CHA_FIRMATO = componentsScambiaAllegato.CHA_FIRMATO;
                    componentDocumentoEntity.VAR_NOMEORIGINALE = componentsScambiaAllegato.VAR_NOMEORIGINALE;
                    componentDocumentoEntity.FILE_INFO = componentsScambiaAllegato.FILE_INFO;
                    componentDocumentoEntity.ID_PEOPLE_PUTFILE = componentsScambiaAllegato.ID_PEOPLE_PUTFILE;
                    componentDocumentoEntity.ID_PEOPLE_DELEGATO_PUTFILE = componentsScambiaAllegato.ID_PEOPLE_DELEGATO_PUTFILE;
                    componentDocumentoEntity.DTA_FILE_ACQUIRED = componentsScambiaAllegato.DTA_FILE_ACQUIRED;
                    componentDocumentoEntity.CHA_TIPO_FIRMA = componentsScambiaAllegato.CHA_TIPO_FIRMA;

                    //Scambia timestamp
                    var timestampDocumentoEntity = await _dbContext.TimestampDocEntities
                        .Where(t => t.VERSION_ID == versionIdDocumentoAsLong && t.DOC_NUMBER == docnumberDocumentoAsLong)
                        .ToListAsync();

                    var timestampAllegatoEntity = await _dbContext.TimestampDocEntities
                        .Where(t => t.VERSION_ID == versionIdAllegatoAsLong && t.DOC_NUMBER == docnumberAllegatoAsLong)
                        .ToListAsync();

                    if(timestampDocumentoEntity != null && timestampDocumentoEntity.Any())
                    {
                        timestampDocumentoEntity.ForEach(t =>
                        {
                            t.DOC_NUMBER = docnumberAllegatoAsLong;
                            t.VERSION_ID = versionIdAllegatoAsLong;
                        });
                    }

                    if (timestampAllegatoEntity != null && timestampAllegatoEntity.Any())
                    {
                        timestampAllegatoEntity.ForEach(t =>
                        {
                            t.DOC_NUMBER = docnumberDocumentoAsLong;
                            t.VERSION_ID = versionIdDocumentoAsLong;
                        });
                    }

                    //Scambia chaImg
                    var chaImgDocumento = profileDocumentoEntity.CHA_IMG;
                    var chaExtDocumento = profileDocumentoEntity.EXT;
                    var chaImgAllegato = profileAllegatoEntity.CHA_IMG;
                    var chaExtAllegato = profileAllegatoEntity.EXT;

                    profileDocumentoEntity.CHA_IMG = chaImgAllegato;
                    profileDocumentoEntity.EXT = chaExtAllegato;
                    profileAllegatoEntity.CHA_IMG = chaImgDocumento;
                    profileAllegatoEntity.EXT = chaExtDocumento;

                    //Scambio info file
                    var infoFileDocumentoEntity = await _dbContext.InfoFileEntities
                        .Where(i => i.VERSION_ID == versionIdDocumentoAsLong && i.ID_PROFILE == docnumberDocumentoAsLong)
                        .FirstOrDefaultAsync();
                    var infoFileAllegatoEntity = await _dbContext.InfoFileEntities
                        .Where(i => i.VERSION_ID == versionIdAllegatoAsLong && i.ID_PROFILE == docnumberAllegatoAsLong)
                        .FirstOrDefaultAsync();

                    if(infoFileDocumentoEntity != null && infoFileAllegatoEntity != null)
                    {
                        var infoFileScambiaDocumento = _mapper.Map<InfoFileScambiaEntity>(infoFileDocumentoEntity);
                        var infoFileScambiaAllegato= _mapper.Map<InfoFileScambiaEntity>(infoFileAllegatoEntity);

                        infoFileAllegatoEntity.DTA_ACQUISIZIONE = infoFileDocumentoEntity.DTA_ACQUISIZIONE;
                        infoFileAllegatoEntity.VAR_ESTENSIONE = infoFileDocumentoEntity.VAR_ESTENSIONE;
                        infoFileAllegatoEntity.VAR_NOME_FILE = infoFileDocumentoEntity.VAR_NOME_FILE;
                        infoFileAllegatoEntity.VAR_DESC_INFO_FILE = infoFileDocumentoEntity.VAR_DESC_INFO_FILE;
                        infoFileAllegatoEntity.CHA_CONFORME = infoFileDocumentoEntity.CHA_CONFORME;
                        infoFileAllegatoEntity.CHA_ESTENSIONE_CONFORME = infoFileDocumentoEntity.CHA_ESTENSIONE_CONFORME;
                        infoFileAllegatoEntity.CHA_PRESENZA_MACRO = infoFileDocumentoEntity.CHA_PRESENZA_MACRO;
                        infoFileAllegatoEntity.CHA_PRESENZA_FORMS = infoFileDocumentoEntity.CHA_PRESENZA_FORMS;
                        infoFileAllegatoEntity.CHA_PRESENZA_JAVASCRIPT = infoFileDocumentoEntity.CHA_PRESENZA_JAVASCRIPT;

                        infoFileDocumentoEntity.DTA_ACQUISIZIONE = infoFileScambiaAllegato.DTA_ACQUISIZIONE;
                        infoFileDocumentoEntity.VAR_ESTENSIONE = infoFileScambiaAllegato.VAR_ESTENSIONE;
                        infoFileDocumentoEntity.VAR_NOME_FILE = infoFileScambiaAllegato.VAR_NOME_FILE;
                        infoFileDocumentoEntity.VAR_DESC_INFO_FILE = infoFileScambiaAllegato.VAR_DESC_INFO_FILE;
                        infoFileDocumentoEntity.CHA_CONFORME = infoFileScambiaAllegato.CHA_CONFORME;
                        infoFileDocumentoEntity.CHA_ESTENSIONE_CONFORME = infoFileScambiaAllegato.CHA_ESTENSIONE_CONFORME;
                        infoFileDocumentoEntity.CHA_PRESENZA_MACRO = infoFileScambiaAllegato.CHA_PRESENZA_MACRO;
                        infoFileDocumentoEntity.CHA_PRESENZA_FORMS = infoFileScambiaAllegato.CHA_PRESENZA_FORMS;
                        infoFileDocumentoEntity.CHA_PRESENZA_JAVASCRIPT = infoFileScambiaAllegato.CHA_PRESENZA_JAVASCRIPT;
                    }

                    //Scambia firma elettronica
                    var firmaElettronicaDocumentoEntity = await _dbContext.FirmaElettronicaEntities
                        .Where(t => t.VERSION_ID == versionIdDocumentoAsLong && t.ID_DOCUMENTO == docnumberDocumentoAsLong)
                        .ToListAsync();

                    var firmaElettronicaAllegatoEntity = await _dbContext.FirmaElettronicaEntities
                        .Where(t => t.VERSION_ID == versionIdAllegatoAsLong && t.ID_DOCUMENTO == docnumberAllegatoAsLong)
                        .ToListAsync();

                    if (firmaElettronicaDocumentoEntity != null && firmaElettronicaDocumentoEntity.Any())
                    {
                        firmaElettronicaDocumentoEntity.ForEach(t =>
                        {
                            t.ID_DOCUMENTO = docnumberAllegatoAsLong;
                            t.VERSION_ID = versionIdAllegatoAsLong;
                        });
                    }

                    if (firmaElettronicaAllegatoEntity != null && firmaElettronicaAllegatoEntity.Any())
                    {
                        firmaElettronicaAllegatoEntity.ForEach(t =>
                        {
                            t.ID_DOCUMENTO = docnumberDocumentoAsLong;
                            t.VERSION_ID = versionIdDocumentoAsLong;
                        });
                    }

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    await this._webMethodLoggerService.LogOK("SCAMBIA_DOC",
                        request.documento.docNumber,
                        string.Format(Resources.LogScambiaAllegato, request.documento.docNumber, request.allegato.versionLabel, request.allegato.descrizione));
                }
                else
                {
                    //Il documento non � stato ancora salvato quindi modifico il session repository
                    FileDocumento? fileDocumentDocumento = null;
                    FileDocumento? fileDocumentAllegato = null;

                    if (await _sessionRepositoryService.FileExists(request.documento.repositoryContext, request.documento))
                    {
                        fileDocumentDocumento = await _sessionRepositoryService.GetFile(request.documento.repositoryContext, request.documento);
                        await _sessionRepositoryService.RemoveFile(request.documento.repositoryContext, request.documento);
                    }

                    if (await _sessionRepositoryService.FileExists(request.allegato.repositoryContext, request.allegato))
                    {
                        fileDocumentAllegato = await _sessionRepositoryService.GetFile(request.allegato.repositoryContext, request.allegato);
                        await _sessionRepositoryService.RemoveFile(request.allegato.repositoryContext, request.allegato);
                    }

                    if (fileDocumentAllegato != null)
                        await _sessionRepositoryService.SetFile(request.documento.repositoryContext, request.documento, fileDocumentAllegato);

                    if (fileDocumentDocumento != null)
                        await _sessionRepositoryService.SetFile(request.allegato.repositoryContext, request.allegato, fileDocumentDocumento);
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
                await this._webMethodLoggerService.LogKO("SCAMBIA_DOC",
                    request.documento.docNumber,
                    string.Format(Resources.LogScambiaAllegato, request.documento.docNumber, request.allegato.versionLabel, request.allegato.descrizione));
                result = false;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("SCAMBIA_DOC", 
                    request.documento.docNumber, 
                    string.Format(Resources.LogScambiaAllegato, request.documento.docNumber, request.allegato.versionLabel, request.allegato.descrizione));
                result = false;
            }

            return new DocumentoScambiaAllegatoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoScambiaAllegatoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<VersionEntity, VersionsScambiaEntity>()
                    .ForMember(dest => dest.SUBVERSION, src => src.MapFrom(opt => opt.SUBVERSION))
                    .ForMember(dest => dest.CARTACEO, src => src.MapFrom(opt => opt.CARTACEO))
                    .ForMember(dest => dest.SCARTA_FASC_CARTACEA, src => src.MapFrom(opt => opt.SCARTA_FASC_CARTACEA));

                cfg.CreateMap<ComponentEntity, ComponentsScambiaEntity>()
                        .ForMember(dest => dest.PATH, src => src.MapFrom(opt => opt.PATH))
                        .ForMember(dest => dest.FILE_SIZE, src => src.MapFrom(opt => opt.FILE_SIZE))
                        .ForMember(dest => dest.VAR_IMPRONTA, src => src.MapFrom(opt => opt.VAR_IMPRONTA))
                        .ForMember(dest => dest.CHA_FIRMATO, src => src.MapFrom(opt => opt.CHA_FIRMATO))
                        .ForMember(dest => dest.EXT, src => src.MapFrom(opt => opt.EXT))
                        .ForMember(dest => dest.VAR_NOMEORIGINALE, src => src.MapFrom(opt => opt.VAR_NOMEORIGINALE))
                        .ForMember(dest => dest.FILE_INFO, src => src.MapFrom(opt => opt.FILE_INFO))
                        .ForMember(dest => dest.ID_PEOPLE_PUTFILE, src => src.MapFrom(opt => opt.ID_PEOPLE_PUTFILE))
                        .ForMember(dest => dest.ID_PEOPLE_DELEGATO_PUTFILE, src => src.MapFrom(opt => opt.ID_PEOPLE_DELEGATO_PUTFILE))
                        .ForMember(dest => dest.DTA_FILE_ACQUIRED, src => src.MapFrom(opt => opt.DTA_FILE_ACQUIRED))
                        .ForMember(dest => dest.CHA_TIPO_FIRMA, src => src.MapFrom(opt => opt.CHA_TIPO_FIRMA));

                cfg.CreateMap<InfoFileEntity, InfoFileScambiaEntity>()
                        .ForMember(dest => dest.DTA_ACQUISIZIONE, src => src.MapFrom(opt => opt.DTA_ACQUISIZIONE))
                        .ForMember(dest => dest.VAR_ESTENSIONE, src => src.MapFrom(opt => opt.VAR_ESTENSIONE))
                        .ForMember(dest => dest.VAR_NOME_FILE, src => src.MapFrom(opt => opt.VAR_NOME_FILE))
                        .ForMember(dest => dest.VAR_DESC_INFO_FILE, src => src.MapFrom(opt => opt.VAR_DESC_INFO_FILE))
                        .ForMember(dest => dest.CHA_CONFORME, src => src.MapFrom(opt => opt.CHA_CONFORME))
                        .ForMember(dest => dest.CHA_ESTENSIONE_CONFORME, src => src.MapFrom(opt => opt.CHA_ESTENSIONE_CONFORME))
                        .ForMember(dest => dest.CHA_PRESENZA_MACRO, src => src.MapFrom(opt => opt.CHA_PRESENZA_MACRO))
                        .ForMember(dest => dest.CHA_PRESENZA_FORMS, src => src.MapFrom(opt => opt.CHA_PRESENZA_FORMS))
                        .ForMember(dest => dest.CHA_PRESENZA_JAVASCRIPT, src => src.MapFrom(opt => opt.CHA_PRESENZA_JAVASCRIPT));
            });

            _mapper = configuration.CreateMapper();
        }
        protected class VersionsScambiaEntity
        {
            public string? SUBVERSION { get; set; }
            public long? CARTACEO { get; set; }
            public long? SCARTA_FASC_CARTACEA { get; set; }
        }

        protected class ComponentsScambiaEntity
        {
            public string? PATH { get; set; }
            public long? FILE_SIZE { get; set; }
            public string? VAR_IMPRONTA { get; set; }
            public string? CHA_FIRMATO { get; set; }
            public string? EXT { get; set; }
            public string? VAR_NOMEORIGINALE { get; set; }
            public string? FILE_INFO { get; set; }
            public long? ID_PEOPLE_PUTFILE { get; set; }
            public long? ID_PEOPLE_DELEGATO_PUTFILE { get; set; }
            public DateTime? DTA_FILE_ACQUIRED { get; set; }
            public string? CHA_TIPO_FIRMA { get; set; }
        }

        protected class InfoFileScambiaEntity
        {
            public DateTime? DTA_ACQUISIZIONE { get; set; }
            public string? VAR_ESTENSIONE { get; set; }
            public string? VAR_NOME_FILE { get; set; }
            public string? VAR_DESC_INFO_FILE { get; set; }
            public string? CHA_CONFORME { get; set; }
            public string? CHA_ESTENSIONE_CONFORME { get; set; }
            public string? CHA_PRESENZA_MACRO { get; set; }
            public string? CHA_PRESENZA_FORMS { get; set; }
            public string? CHA_PRESENZA_JAVASCRIPT { get; set; }
        }
        #endregion
    }
}