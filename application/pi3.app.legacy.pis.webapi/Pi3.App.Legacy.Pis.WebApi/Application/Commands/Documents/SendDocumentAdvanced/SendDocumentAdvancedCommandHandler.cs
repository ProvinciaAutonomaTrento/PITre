// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetSpedizioneDocumento;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using DocsPaVO.utente;
using AutoMapper;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.RisolviCorrispondente;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SpedisciDocumento;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SendDocumentAdvanced
{
    // Richiede libreria MediatR
    public class SendDocumentAdvancedCommandHandler : IRequestHandler<SendDocumentAdvancedCommand, SendDocumentAdvancedCommandResponse>
    {
        #region Public Members

        public SendDocumentAdvancedCommandHandler(ILogger<SendDocumentAdvancedCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this.InitializeMapper();

        }

        public async Task<SendDocumentAdvancedCommandResponse> Handle(SendDocumentAdvancedCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("SendDocumentAdvanced - START");

            SendDocumentAdvancedCommandResponse response = new SendDocumentAdvancedCommandResponse();
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

                if (string.IsNullOrEmpty(request.CodeRegister) && string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(request.CodeRegister) && !string.IsNullOrEmpty(request.IdRegister))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
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

                if (documento == null)
                    throw new RestException("DOCUMENT_NOT_FOUND");

                DocsPaVO.Spedizione.SpedizioneDocumento infoSpedizione = (await _mediator.Send(new GetSpedizioneDocumentoCommand()
                {
                    Documento = documento,
                    InfoUtente = infoUtente
                })).SpedizioneDocumento;

                DocsPaVO.utente.Registro registro = null;
                try
                {
                    if (!string.IsNullOrEmpty(request.CodeRegister))
                    {
                        registro = DBUtils.getRegistroByCodAOO(request.CodeRegister, infoUtente.idAmministrazione,this._pi3DbContext);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(request.IdRegister))
                        {
                            try
                            {
                                registro = DBUtils.getRegistro(request.IdRegister,this._pi3DbContext);
                            }
                            catch
                            {
                                throw new RestException("REGISTER_NOT_FOUND");
                            }
                        }
                    }
                    if(registro == null)
                    {
                        throw new RestException("REGISTER_NOT_FOUND");
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogCritical("Errore ricerca registro: " + ex.Message);
                    throw new RestException("REGISTER_NOT_FOUND");
                }

                infoSpedizione.IdRegistroRfMittente = registro.systemId;
                string mailMittente = "";
                string mailspedizione = "";
                bool emailFound = false;
                bool roleEnabled = false;

                if (!string.IsNullOrEmpty(infoSpedizione.IdRegistroRfMittente))
                {
                    registro = DBUtils.getRegistro(infoSpedizione.IdRegistroRfMittente, _pi3DbContext);
                    Registro[] listaRegistriRF = await UtenteGetRegistriWithRf(infoUtente.idCorrGlobali.AsLong(), "1", infoSpedizione.IdRegistroRfMittente.AsLong());
                    foreach (Registro reg in listaRegistriRF)
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
                                //&& j.v.CHA_SPEDISCI == "1"
                                )
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

                        if (rightRuoloRfEntity != null)
                        {
                            if (!string.IsNullOrEmpty(request.SenderMail))
                            {
                                emailFound = emailFound || ((rightRuoloRfEntity.EMAIL_REGISTRO ?? string.Empty).ToUpper().Equals(request.SenderMail.ToUpper()));
                                roleEnabled = roleEnabled || "1".Equals(rightRuoloRfEntity.SPEDISCI);
                                if (roleEnabled && emailFound)
                                    break;
                            }
                            else
                            {
                                roleEnabled = roleEnabled || (registro.email.ToUpper().Equals((rightRuoloRfEntity.EMAIL_REGISTRO ?? string.Empty).ToUpper()) && "1".Equals(rightRuoloRfEntity.SPEDISCI));
                                if (roleEnabled)
                                    break;
                            }
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
                        if (!string.IsNullOrEmpty(request.SenderMail))
                        {
                            emailFound = emailFound || ((rightRuoloRegistroEntity.EMAIL_REGISTRO ?? string.Empty).ToUpper().Equals(request.SenderMail.ToUpper()));
                            roleEnabled = roleEnabled || "1".Equals(rightRuoloRegistroEntity.SPEDISCI);
                        }
                        else
                        {
                            roleEnabled = roleEnabled || (registro.email.ToUpper().Equals((rightRuoloRegistroEntity.EMAIL_REGISTRO ?? string.Empty).ToUpper()) && "1".Equals(rightRuoloRegistroEntity.SPEDISCI));
                        }
                    }

                    if (!string.IsNullOrEmpty(request.SenderMail))
                    {
                        if (!emailFound)
                        {
                            throw new Exception(string.Format(ErrorMessages.RegNotAssWithReqEmail, request.SenderMail));
                        }
                        else if (!roleEnabled)
                        {
                            throw new Exception(string.Format(ErrorMessages.RoleNotEnabled, ruolo.codice));
                        }
                        infoSpedizione.mailAddress = request.SenderMail;
                    }
                    else 
                    {
                        if (!roleEnabled)
                        {
                            throw new Exception(string.Format(ErrorMessages.RoleNotEnabled, ruolo.codice));
                        }

                        infoSpedizione.mailAddress = registro.email;
                    }
                }

                List<string> idDestinatari = new List<string>();
                if (request.Recipients == null || request.Recipients.Length < 1)
                {

                    if (infoSpedizione != null)
                    {
                        if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                        {
                            foreach (DocsPaVO.Spedizione.DestinatarioEsterno es in infoSpedizione.DestinatariEsterni)
                            {
                                es.IncludiInSpedizione = true;
                            }
                        }

                        if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                        {
                            foreach (DocsPaVO.Spedizione.DestinatarioInterno es in infoSpedizione.DestinatariInterni)
                            {
                                es.IncludiInSpedizione = true;
                            }
                        }
                    }

                }
                else
                {
                    foreach (Correspondent corrPIS in request.Recipients)
                    {
                        DocsPaVO.utente.Corrispondente corrBySys = null;

                        if (corrPIS != null && !string.IsNullOrEmpty(corrPIS.CorrespondentType) && corrPIS.CorrespondentType.Equals("O"))
                        {
                            corrBySys = RestUtils.GetCorrespondentFromPis(corrPIS, infoUtente, this._pi3DbContext);
                        }
                        else
                        {
                            corrBySys = (await this._mediator.Send(new RisolviCorrispondenteCommand()
                            {
                                InfoUtente = infoUtente,
                                SearchKey = corrPIS.Id
                            })).Corrispondente;
                            if (corrBySys == null)
                            {
                                throw new RestException("RECIPIENT_NOT_FOUND");
                            }
                            else
                            {
                                idDestinatari.Add(corrBySys.systemId);
                            }
                        }

                        if (infoSpedizione != null)
                        {
                            bool ctrlCorrDestinatario = false;
                            if (infoSpedizione.DestinatariEsterni != null && infoSpedizione.DestinatariEsterni.Count > 0)
                            {
                                foreach (DocsPaVO.Spedizione.DestinatarioEsterno es in infoSpedizione.DestinatariEsterni)
                                {
                                    if (idDestinatari.Contains(es.DatiDestinatari[0].systemId))
                                    {
                                        es.IncludiInSpedizione = true;
                                        if (es.DatiDestinatari[0].systemId == corrBySys.systemId && !string.IsNullOrWhiteSpace(corrPIS.Email))
                                        {
                                            if (corrBySys.Emails != null && corrBySys.Emails.Count > 0)
                                            {
                                                var emailfound = corrBySys.Emails.Where(r => r.Email.ToLower() == corrPIS.Email.ToLower()).ToList();
                                                if (emailfound != null && emailfound.Count > 0)
                                                    es.Email = corrPIS.Email;
                                                else
                                                {
                                                    throw new Exception(string.Format(ErrorMessages.MailNotConfigedOnCorr, corrBySys.descrizione, corrPIS.Email));
                                                }
                                            }
                                        }
                                        ctrlCorrDestinatario = true;
                                    }
                                    else
                                    {
                                        es.IncludiInSpedizione = false;
                                    }
                                }
                            }

                            if (infoSpedizione.DestinatariInterni != null && infoSpedizione.DestinatariInterni.Count > 0)
                            {
                                foreach (DocsPaVO.Spedizione.DestinatarioInterno es in infoSpedizione.DestinatariInterni)
                                {
                                    if (idDestinatari.Contains(es.DatiDestinatario.systemId))
                                    {
                                        es.IncludiInSpedizione = true;
                                        ctrlCorrDestinatario = true;
                                    }
                                    else
                                    {
                                        es.IncludiInSpedizione = false;
                                    }
                                }
                            }
                            if (!ctrlCorrDestinatario)
                                throw new Exception(string.Format(ErrorMessages.CorrNotFoundAsRec, corrBySys.descrizione));
                        }
                    }
                }

                //Spedizione del documento
                infoSpedizione = (await _mediator.Send(new SpedisciDocumentoCommand()
                {
                    Documento = documento,
                    InfoSpedizione = infoSpedizione,
                    InfoUtente = infoUtente
                })).SpedizioneDocumento;

                _logger.LogInformation($"infoSpedizione.IdRegistroRfMittente = {infoSpedizione.IdRegistroRfMittente}");
                _logger.LogInformation($"infoSpedizione.mailAddress = {infoSpedizione.mailAddress}");


                List<SendingResult> corrsSentTo = new List<SendingResult>();
                SendingResult corrRes = null;

                foreach (DocsPaVO.Spedizione.DestinatarioEsterno corr in infoSpedizione.DestinatariEsterni)
                {
                    if (request.Recipients != null && request.Recipients.Length > 0)
                    {
                        foreach (var rec in request.Recipients)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(rec.Id) && rec.Id == corr.Id)
                                {
                                    string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                                    corrRes = new SendingResult
                                    {
                                        CorrespondentId = corr.Id,
                                        CorrespondentDescription = corr.DatiDestinatari[0].descrizione,
                                        Mail = corr.Email,
                                        PrefChannel = canalePref,
                                        SendResult = corr.StatoSpedizione != null ? corr.StatoSpedizione.Descrizione : (infoSpedizione.Spedito ? "Spedito" : "Errore nella spedizione")
                                    };
                                    corrsSentTo.Add(corrRes);
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, string.Format(ErrorMessages.InsertCorrInResFailed, corr.Id, corr.DatiDestinatari[0].descrizione, corr.Email, corr.IncludiInSpedizione.ToString()));
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            string canalePref = (corr.DatiDestinatari[0].canalePref != null ? corr.DatiDestinatari[0].canalePref.descrizione : "MAIL");
                            corrRes = new SendingResult();
                            corrRes.CorrespondentId = corr.Id;
                            corrRes.CorrespondentDescription = corr.DatiDestinatari[0].descrizione;
                            corrRes.Mail = corr.Email;
                            corrRes.PrefChannel = canalePref;
                            corrRes.SendResult = corr.StatoSpedizione != null ? corr.StatoSpedizione.Descrizione : (infoSpedizione.Spedito ? "Spedito" : "Errore nella spedizione");
                            corrsSentTo.Add(corrRes);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, string.Format(ErrorMessages.InsertCorrInResFailed, corr.Id, corr.DatiDestinatari[0].descrizione, corr.Email, corr.IncludiInSpedizione.ToString()));
                        }
                    }
                }
                response.SendingResults = corrsSentTo.ToArray();
                #endregion


                response.Code = SendDocAdvResponseCode.OK;

                _logger.LogInformation("end SendDocumentAdvanced");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione SendDocumentAdvanced: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new SendDocumentAdvancedCommandResponse();
                response.Code = SendDocAdvResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione SendDocumentAdvanced");
                response = new SendDocumentAdvancedCommandResponse();
                response.Code = SendDocAdvResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<SendDocumentAdvancedCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
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