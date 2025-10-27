// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Spedizione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Org.BouncyCastle.Ocsp;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedisciDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocument
{
    // Richiede libreria MediatR
    public class SendDocumentCommandHandler : IRequestHandler<SendDocumentCommand, SendDocumentCommandResponse>
    {
        #region Public Members

        public SendDocumentCommandHandler(ILogger<SendDocumentCommandHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor, 
            IPi3DbContext pi3DbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._webMethodLoggerService = webMethodLoggerService;

            this.InitializeMapper();
        }

        public async Task<SendDocumentCommandResponse> Handle(SendDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SendDocument - START");

            SendDocumentCommandResponse response = new SendDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (string.IsNullOrEmpty(request.IdDocument) && string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.IdDocument) && !string.IsNullOrEmpty(request.Signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }
                #endregion

                #region implementazione

                DocsPaVO.documento.SchedaDocumento documento = new DocsPaVO.documento.SchedaDocumento();

                // Controllo visibilit� documento
                try
                {
                    long? idProfile = !string.IsNullOrEmpty(request.IdDocument) ? request.IdDocument.AsLong() : null;
                    if (idProfile == null && !string.IsNullOrEmpty(request.Signature))
                    {
                        idProfile = await _pi3DbContext.ProfileEntities.AsNoTracking()
                                .Where(p => p.VAR_SEGNATURA != null && p.VAR_SEGNATURA.ToUpper() == request.Signature.ToUpper())
                                .Select(p => p.SYSTEM_ID)
                                .FirstOrDefaultAsync();
                    }

                    await _pi3DbContext.AssertSecurityRights(idProfile.ToString(), infoUtente.idPeople, infoUtente.idGruppo);

                    documento = (await this._mediator.Send(new DocumentoGetDettaglioDocumentoNoSecurityCommand()
                    {
                        Infoutente = infoUtente,
                        DocNumber = idProfile.ToString(),
                        IdProfile = idProfile.ToString()
                    })).Output;
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                if(documento == null)
                    throw new RestException("DOCUMENT_NOT_FOUND");

                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = (await _mediator.Send(new GetSpedizioneDocumentoCommand()
                {
                       Documento = documento,
                       InfoUtente = infoUtente
                })).SpedizioneDocumento;

                List<DocsPaVO.utente.Registro> listRegistriRF = new List<DocsPaVO.utente.Registro>();
                string mailspedizione = "";

                if (!string.IsNullOrEmpty(documento.registro?.systemId))
                {
                    Registro registro = DBUtils.getRegistro(documento.registro.systemId, _pi3DbContext);
                    Registro[] listaRegistriRF = await UtenteGetRegistriWithRf(infoUtente.idCorrGlobali.AsLong(), "1", documento.registro.systemId.AsLong());
                    foreach(Registro reg in listaRegistriRF)
                    {
                        var rightRuoloRfEntity = await _pi3DbContext.RuoloRegistroEntities
                            .Join(_pi3DbContext.VisMailRegistriEntities,
                                r => r.ID_REGISTRO,
                                v => v.ID_REGISTRO,
                                (r, v) => new { r, v })
                            .Join(_pi3DbContext.MailRegistriEntities,
                                j => j.v.ID_REGISTRO,
                                m => m.ID_REGISTRO,
                                (j, m) => new { j.r, j.v, m })
                            .Where(j => j.r.ID_REGISTRO == reg.systemId.AsLong()
                                && j.r.ID_RUOLO_IN_UO == ruolo.systemId.AsLong()
                                && j.v.VAR_EMAIL_REGISTRO == j.m.VAR_EMAIL_REGISTRO
                                && j.r.ID_RUOLO_IN_UO == j.v.ID_RUOLO_IN_UO
                                && j.v.CHA_SPEDISCI == "1")
                            .Select(j => new
                            {
                                EMAIL_REGISTRO = j.v.VAR_EMAIL_REGISTRO,
                                CONSULTA = j.v.CHA_CONSULTA,
                                NOTIFICA = j.v.CHA_NOTIFICA,
                                SPEDISCI = j.v.CHA_SPEDISCI,
                                j.m.VAR_NOTE,
                                ID_MAIL_REGISTRI = j.m.SYSTEM_ID
                            })
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

                        if(rightRuoloRfEntity != null)
                        {
                            listRegistriRF.Add(reg);
                            mailspedizione = rightRuoloRfEntity.EMAIL_REGISTRO;
                            break;
                        }
                    }

                    var rightRuoloRegistroEntity = await _pi3DbContext.RuoloRegistroEntities
                           .Join(_pi3DbContext.VisMailRegistriEntities,
                               r => r.ID_REGISTRO,
                               v => v.ID_REGISTRO,
                               (r, v) => new { r, v })
                           .Join(_pi3DbContext.MailRegistriEntities,
                               j => j.v.ID_REGISTRO,
                               m => m.ID_REGISTRO,
                               (j, m) => new { j.r, j.v, m })
                           .Where(j => j.r.ID_REGISTRO == registro.systemId.AsLong()
                               && j.r.ID_RUOLO_IN_UO == ruolo.systemId.AsLong()
                               && j.v.VAR_EMAIL_REGISTRO == j.m.VAR_EMAIL_REGISTRO
                               && j.r.ID_RUOLO_IN_UO == j.v.ID_RUOLO_IN_UO
                               && j.v.CHA_SPEDISCI == "1")
                           .Select(j => new
                           {
                               EMAIL_REGISTRO = j.v.VAR_EMAIL_REGISTRO,
                               CONSULTA = j.v.CHA_CONSULTA,
                               NOTIFICA = j.v.CHA_NOTIFICA,
                               SPEDISCI = j.v.CHA_SPEDISCI,
                               j.m.VAR_NOTE,
                               ID_MAIL_REGISTRI = j.m.SYSTEM_ID
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync();

                    if (rightRuoloRegistroEntity != null)
                    {
                        listRegistriRF.Add(registro);
                        mailspedizione = rightRuoloRegistroEntity.EMAIL_REGISTRO ?? string.Empty;
                    }

                    if (listaRegistriRF == null || listaRegistriRF.Length == 0)
                    {
                        response.Code = MessageResponseCode.SYSTEM_ERROR;
                    }

                    infoSpedizione.IdRegistroRfMittente = listRegistriRF[0].systemId;
                    infoSpedizione.mailAddress = !string.IsNullOrEmpty(mailspedizione) ? mailspedizione : listRegistriRF[0].email;
                }

                _logger.LogInformation($"infoSpedizione.IdRegistroRfMittente = {infoSpedizione.IdRegistroRfMittente}");
                _logger.LogInformation($"infoSpedizione.mailAddress = {infoSpedizione.mailAddress}");

                //Spedizione del documento
                infoSpedizione = (await _mediator.Send(new SpedisciDocumentoCommand()
                {
                    Documento = documento,
                    InfoSpedizione = infoSpedizione,
                    InfoUtente = infoUtente
                })).SpedizioneDocumento;

                #endregion

                response.Code = infoSpedizione.Spedito ? MessageResponseCode.OK : MessageResponseCode.SYSTEM_ERROR;
                if(!infoSpedizione.Spedito)
                    throw new RestException("SEND_DOCUMENT_FAILED");

                response.ResultMessage = Resources.DocumentoSpedito;

                await this._webMethodLoggerService.LogOK("DOCUMENTOSPEDISCI", documento.docNumber,
                          string.Format(Resources.LogSpedizioneDocumento, documento.docNumber), null, "PITRE");

                _logger.LogInformation("end SendDocument");

            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SendDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SendDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SendDocument");
                response = new SendDocumentCommandResponse();
                response.Code = MessageResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SendDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP ?? string.Empty))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) ? opt.CHA_DISABILITATO : "0"))
                    .ForMember(dest => dest.Sospeso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) && opt.CHA_DISABILITATO.Equals("1") ? true : false))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.VAR_PREG) && opt.VAR_PREG.Equals("1") ? true : false));
            });
            _mapper = configuration.CreateMapper();
        }


        public async Task<Registro[]> UtenteGetRegistriWithRf(long idCorrGlobali, string all, long? id_aoo_collegata = null)
        {
            Registro[] output = null;

            try
            {
                var registro_entity = _pi3DbContext.RegistroEntities
                    .Join(
                        _pi3DbContext.RuoloRegistroEntities,
                        registro => registro.SYSTEM_ID,
                        ruolo => ruolo.ID_REGISTRO,
                        (registro, ruolo) => new
                        {
                            registro,
                            ruolo.CHA_PREFERITO,
                            ruolo.ID_RUOLO_IN_UO
                        }
                    )
                    .Where(x => x.ID_RUOLO_IN_UO == idCorrGlobali)
                    .OrderByDescending(x => x.CHA_PREFERITO ?? "0")
                    .OrderBy(x => x.registro.VAR_CODICE)
                    .Select(x => x.registro);

                if (!string.IsNullOrEmpty(all))
                {
                    registro_entity = registro_entity.Where(x => x.CHA_RF.Equals(all));
                    if (id_aoo_collegata != null && all.Equals("1"))
                    {
                        registro_entity = registro_entity.Where(x => x.ID_AOO_COLLEGATA == id_aoo_collegata);
                    }
                }

                output = _mapper.Map<DocsPaVO.utente.Registro[]>(registro_entity);

                if (output != null && output.Any())
                {
                    long id_amministrazione = output[0].idAmministrazione.AsLong();
                    var codiceAmministrazione = await this._pi3DbContext.AmministraEntities.AsNoTracking()
                        .Where(a => a.SYSTEM_ID == id_amministrazione)
                        .Select(a => a.VAR_CODICE_AMM)
                        .FirstAsync();

                    foreach (DocsPaVO.utente.Registro registro in output)
                        registro.codAmministrazione = codiceAmministrazione;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return output;
        }

        #endregion
    }

}