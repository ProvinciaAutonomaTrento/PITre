// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.InstanceAccess.Metadata;
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using PutElectronicSignatureRequest = Pi3.App.Legacy.WebApi.Application.Requests.PutElectronicSignature;
using AggiornaEsecuzioneElementoInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AggiornaEsecuzioneElementoInLibroFirma;
using Pi3.App.Legacy.WebApi.Application.Services.RabbitMQ;
using Pi3.App.Legacy.WebApi.Application.Handlers.LibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.PutElectronicSignature
{
    public class PutElectronicSignatureHandler : IRequestHandler<PutElectronicSignatureRequest, PutElectronicSignatureResult>
    {
        #region Public Members

        public PutElectronicSignatureHandler(ILogger<PutElectronicSignatureHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;

            this.InitializeMapper();
        }

        public async Task<PutElectronicSignatureResult> Handle(PutElectronicSignatureRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            var message = string.Empty;
            FileRequest approvingFile = request.approvingFile;
            var versionIdAsLong = approvingFile.versionId.AsLong();
            var docnumberAsLong = approvingFile.docNumber.AsLong();
            var azione = request.isAdvancementProcess ? "DOC_STEP_OVER" : "DOC_VERIFIED";

            var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var userId = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId);
            var UserName = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserName);
            var UserSurname = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserSurname);

            var idGruppo = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var groupDescription = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.GroupDescription);

            var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);
            var delegatedUserName = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserName);
            var delegatedUserSurname = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedUserSurname);

            try
            {
                if (approvingFile.inLibroFirma)
                {
                    var canToSign = await this._dbContext.ElementoInLibroFirmaEntities.AnyAsync(e => e.DOC_NUMBER == docnumberAsLong && e.TIPO_FIRMA == azione
                                                && e.DTA_ESECUZIONE == null && e.ID_RUOLO_TITOLARE == idGruppo
                                                && (e.ID_UTENTE_TITOLARE == idUser || e.ID_UTENTE_TITOLARE == null)
                                                && (e.ID_UTENTE_LOCKER == idUser || e.ID_UTENTE_LOCKER == null));

                    if (!canToSign)
                        throw new OperazionePresaInCaricoDaAltroUtentePi3Exception();
                }

                var nominativo = $"{UserName} {UserSurname}";
                var delegato = delegatedIdUser != 0 ? $"{delegatedUserName} {delegatedUserSurname}" : string.Empty;
                var corrGlobaliUser = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == idUser).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();
                var corrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

                var numAll = approvingFile.versionLabel.ToUpper().Replace("A", "");
                var firmaElettronicaEntity = new FirmaElettronicaEntity()
                {
                    ID_DOCUMENTO = docnumberAsLong,
                    VERSION_ID = versionIdAsLong,
                    DOC_ALL = approvingFile.GetType().Equals(typeof(DocsPaVO.documento.Allegato)) ? "A" : "D",
                    NUM_ALL = !string.IsNullOrEmpty(numAll) ? numAll.AsLong() : null,
                    DATA_APPOSIZIONE = await _dbContext.GetSystemDateTime(),
                    NUMERO_VERSIONE = approvingFile.version.AsLong(),
                    XML = null
                };
                if (!request.isAdvancementProcess)
                {
                    FirmaElettronica firma = this._mapper.Map<FirmaElettronica>(firmaElettronicaEntity);
                    firma.Xml = Resources.FirmaElettronicaXml;
                    firma.Imponta = await this._dbContext.ComponentEntities.Where(c => c.VERSION_ID == versionIdAsLong && c.DOCNUMBER == docnumberAsLong).Select(c => c.VAR_IMPRONTA).FirstOrDefaultAsync();
                    firma.GeneraXML(nominativo, groupDescription, delegato, corrGlobaliGruppo.ToString(), corrGlobaliUser.ToString());
                    firmaElettronicaEntity.XML = firma.Xml;
                }

                await this._dbContext.FirmaElettronicaEntities.AddAsync(firmaElettronicaEntity);
                await ((DbContext)_dbContext).SaveChangesAsync();

                output = true;

                if(!request.isAdvancementProcess)
                {
                    //imposto la versione come firmata
                    var tipoFirma = approvingFile.tipoFirma;
                    switch (tipoFirma)
                    {
                        case (DocsPaVO.documento.TipoFirma.CADES):
                            tipoFirma = TipoFirma.CADES_ELETTORNICA;
                            break;
                        case (DocsPaVO.documento.TipoFirma.XADES):
                            tipoFirma =TipoFirma.XADES_ELETTORNICA;
                            break;
                        case (DocsPaVO.documento.TipoFirma.PADES):
                            tipoFirma = TipoFirma.PADES_ELETTORNICA;
                            break;
                        case (DocsPaVO.documento.TipoFirma.TSD):
                            tipoFirma = TipoFirma.TSD_ELETTORNICA;
                            break;
                        case (DocsPaVO.documento.TipoFirma.NESSUNA_FIRMA):
                            tipoFirma = TipoFirma.ELETTORNICA;
                            break;
                    }

                    var componentsEntity = await this._dbContext.ComponentEntities.FirstAsync(c => c.VERSION_ID == versionIdAsLong && c.DOCNUMBER == docnumberAsLong);
                    componentsEntity.CHA_FIRMATO = "1";
                    componentsEntity.CHA_TIPO_FIRMA = tipoFirma;

                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch(OperazionePresaInCaricoDaAltroUtentePi3Exception ex)
            {
                this._logger.LogError(ex, null, null);
                message = ex.Message;
                output = false;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                message = ErrorDescriptions.ErroreGenerico;
                output = false;
            }

            //Se il documento è in libro firma aggiorno lo stato
            if (approvingFile.inLibroFirma)
            {
                await this._mediator.Send(new AggiornaEsecuzioneElementoInLibroFirmaRequest(docnumberAsLong.ToString(),
                    request.isAdvancementProcess ? Resources.LogAvanzamentoIter : Resources.LogFirmaElettronica, output,
                    TipoStatoElemento.FIRMATO, message));
            }

            if(output)
            {
                await this._webMethodLoggerService.LogOK(azione,
                    approvingFile.docNumber,
                    request.isAdvancementProcess ? Resources.LogAvanzamentoIter : Resources.LogFirmaElettronica, null, "PITRE");

                //Aggiungo in coda al motore di Libro Firma
                if ((await this._mediator.Send(new Requests.IsDocInLibroFirma(approvingFile.docNumber))).output)
                {
                    await this._mediator.Send(
                    new MessageQueueCommandWrapper(new LibroFirmaRequest(this._claimsPrincipalService.Current)
                    {
                        IdProfile = approvingFile.docNumber,
                        Evento = azione,
                    }));
                }
            }

            return new PutElectronicSignatureResult(output, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<PutElectronicSignatureHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FirmaElettronicaEntity, FirmaElettronica>()
                    .ForMember(dest => dest.IdFirma, src => src.MapFrom(opt => opt.ID_FIRMA))
                    .ForMember(dest => dest.Docnumber, src => src.MapFrom(opt => opt.ID_DOCUMENTO))
                    .ForMember(dest => dest.Versionid, src => src.MapFrom(opt => opt.VERSION_ID))
                    .ForMember(dest => dest.DocAll, src => src.MapFrom(opt => opt.DOC_ALL))
                    .ForMember(dest => dest.NumAll, src => src.MapFrom(opt => opt.NUM_ALL))
                    .ForMember(dest => dest.NumVersione, src => src.MapFrom(opt => opt.NUMERO_VERSIONE))
                    .ForMember(dest => dest.Xml, src => src.MapFrom(opt => opt.XML))
                    .ForMember(dest => dest.DataApposizione, src => src.MapFrom(opt => opt.DATA_APPOSIZIONE.AsDateTimeFormat()));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
