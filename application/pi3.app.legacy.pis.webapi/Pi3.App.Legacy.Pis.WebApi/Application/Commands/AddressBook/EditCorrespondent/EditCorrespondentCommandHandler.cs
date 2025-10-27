// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using System.Reflection.PortableExecutable;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.EditCorrespondent
{
    // Richiede libreria MediatR
    public class EditCorrespondentCommandHandler : IRequestHandler<EditCorrespondentCommand, EditCorrespondentCommandResponse>
    {
        #region Public Members

        public EditCorrespondentCommandHandler(ILogger<EditCorrespondentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<EditCorrespondentCommandResponse> Handle(EditCorrespondentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("EditCorrespondent - START");

            EditCorrespondentCommandResponse response = new EditCorrespondentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta

                if (request == null)
                {
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.Id))
                {
                    //Corrispondente non trovato
                    throw new RestException("REQUIRED_IDCORRESPONDENT");
                }

                if (request.Correspondent.CorrespondentType.Equals("U"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Description))
                    {
                        //richiesta descrizione
                        throw new RestException("REQUIRED_DESCRIPTION_CORREPONDENT");
                    }
                }

                if (request.Correspondent.CorrespondentType.Equals("P"))
                {
                    if (string.IsNullOrEmpty(request.Correspondent.Name) || string.IsNullOrEmpty(request.Correspondent.Surname))
                    {
                        //richiesta descrizione
                        throw new RestException("REQUIRED_N_S_CORREPONDENT");
                    }
                }

                #endregion

                #region implementazione

                var oldCorr = (from a in _pi3DbContext.CorrGlobaliEntities where a.SYSTEM_ID == request.Correspondent.Id.AsLong() select a).FirstOrDefault();
                CorrGlobaliEntity newCorr = null;
                if (oldCorr != null && oldCorr.SYSTEM_ID > 0)
                {


                    // ignoro le modifiche su corrispondenti non validi
                    if (oldCorr.CHA_TIPO_CORR == "O" || oldCorr.CHA_TIPO_URP == "R" || oldCorr.CHA_TIPO_IE == "I") throw new RestException("CORRESPONDENT_NOT_FOUND");
                    var listaCanali = _pi3DbContext.DocumentTypesEntities.ToList();
                    var idCanaleLettera = listaCanali.Where(x => x.DESCRIPTION.ToUpper() == "LETTERA").Select(x => x.SYSTEM_ID).FirstOrDefault();
                    long idCanaleSelezionato = 0;
                    if (!string.IsNullOrWhiteSpace(request.Correspondent.PreferredChannel))
                    {
                        idCanaleSelezionato = listaCanali.Where(x => x.DESCRIPTION.ToUpper() == request.Correspondent.PreferredChannel.ToUpper()).Select(x => x.SYSTEM_ID).FirstOrDefault();
                    }
                    if (idCanaleSelezionato == 0) idCanaleSelezionato = idCanaleLettera;

                    // non si modifica il corrispondente attuale, va storicizzato e creato nuovo.

                    oldCorr.DTA_FINE = DateTime.Now;
                    string codiceCorrispondente = oldCorr.VAR_COD_RUBRICA;
                    oldCorr.VAR_COD_RUBRICA = codiceCorrispondente + "_" + oldCorr.SYSTEM_ID.ToString();

                    //if (corrDB.CHA_TIPO_URP == "U")
                    //{
                    //    corrDB.VAR_DESC_CORR = request.Correspondent.Description;

                    //}else if(corrDB.CHA_TIPO_URP== "P")
                    //{
                    //    corrDB.VAR_DESC_CORR = string.Format("{0} {1}", request.Correspondent.Surname, request.Correspondent.Name);
                    //    corrDB.VAR_NOME = request.Correspondent.Name;
                    //    corrDB.VAR_COGNOME = request.Correspondent.Surname;
                    //}
                    //corrDB.VAR_EMAIL = request.Correspondent.Email;
                    string descrizione = request.Correspondent.Description;
                    if (oldCorr.CHA_TIPO_URP == "P")
                    {
                        descrizione = string.Format("{0} {1}", request.Correspondent.Surname, request.Correspondent.Name);
                    }

                    // inserimento in DPA_CORR_GLOBALI

                    newCorr = new CorrGlobaliEntity()
                    {
                        VAR_COD_RUBRICA = codiceCorrispondente,
                        VAR_CODICE = codiceCorrispondente,
                        CHA_TIPO_URP = oldCorr.CHA_TIPO_URP,
                        CHA_TIPO_IE = oldCorr.CHA_TIPO_IE,
                        VAR_DESC_CORR = descrizione,
                        VAR_DESC_CORR_OLD = descrizione,
                        VAR_EMAIL = request.Correspondent.Email,
                        VAR_NOME = request.Correspondent.CorrespondentType == "P" ? request.Correspondent.Name : null,
                        VAR_COGNOME = request.Correspondent.CorrespondentType == "P" ? request.Correspondent.Surname : null,
                        ID_REGISTRO = oldCorr.ID_REGISTRO,
                        ID_AMM = infoUtente.idAmministrazione.AsLong(),
                        DTA_INIZIO = DateTime.Now,
                        CHA_DETTAGLI = "1",
                        CHA_TIPO_CORR = "S",
                        CHA_PA = "1",
                        ID_PARENT = 0,
                        ID_OLD = oldCorr.SYSTEM_ID,
                        VAR_CHIAVE_AE = "0",
                        CHA_SYSTEM_ROLE = "0"
                    };
                    _pi3DbContext.CorrGlobaliEntities.Add(newCorr);
                    _logger.LogDebug("Corrispondente inserito con ID: " + newCorr.SYSTEM_ID);

                    // inserimento in DPA_DETT_GLOBALI
                    DettGlobaliEntity dett = new DettGlobaliEntity()
                    {
                        ID_CORR_GLOBALI = newCorr.SYSTEM_ID,
                        VAR_INDIRIZZO = request.Correspondent.Address,
                        VAR_CAP = request.Correspondent.Cap,
                        VAR_PROVINCIA = request.Correspondent.Province,
                        VAR_NAZIONE = request.Correspondent.Nation,
                        VAR_COD_FISC = request.Correspondent.NationalIdentificationNumber,
                        VAR_COD_PI = request.Correspondent.VatNumber,
                        VAR_TELEFONO = request.Correspondent.PhoneNumber,
                        VAR_TELEFONO2 = request.Correspondent.PhoneNumber2,
                        VAR_FAX = request.Correspondent.Fax,
                        VAR_NOTE = request.Correspondent.Note,
                        VAR_CITTA = request.Correspondent.City,
                        VAR_LOCALITA = request.Correspondent.Location
                    };

                    await _pi3DbContext.DettGlobaliEntities.AddAsync(dett);

                    // inserimento in DPA_MAIL_CORR_ESTERNI
                    if (!string.IsNullOrWhiteSpace(request.Correspondent.Email))
                    {
                        MailCorrEsterniEntity mail = new MailCorrEsterniEntity()
                        {
                            ID_CORR = newCorr.SYSTEM_ID,
                            VAR_EMAIL = request.Correspondent.Email,
                            VAR_PRINCIPALE = "1"
                        };
                        await _pi3DbContext.MailCorrEsterniEntities.AddAsync(mail);
                    }
                    if (request.Correspondent.OtherEmails != null && request.Correspondent.OtherEmails.Any())
                    {
                        var altremail = request.Correspondent.OtherEmails.Where(x => x.ToUpper() != request.Correspondent.Email.ToUpper()).ToList();
                        if (altremail != null && altremail.Any())
                        {
                            foreach (var email in altremail)
                            {
                                await _pi3DbContext.MailCorrEsterniEntities.AddAsync(new MailCorrEsterniEntity()
                                {
                                    ID_CORR = newCorr.SYSTEM_ID,
                                    VAR_EMAIL = email,
                                    VAR_PRINCIPALE = "0"
                                });
                            }

                        }

                    }

                    // inserimento in DPA_T_CANALE_CORR
                    CanaleCorrEntity canaleCorr = new CanaleCorrEntity()
                    {
                        ID_CORR_GLOBALE = newCorr.SYSTEM_ID,
                        ID_DOCUMENTTYPE = idCanaleSelezionato > 0 ? idCanaleSelezionato : idCanaleLettera,
                        CHA_PREFERITO = "1"
                    };
                    await _pi3DbContext.CanaleCorrEntities.AddAsync(canaleCorr);
                    await ((DbContext)this._pi3DbContext).SaveChangesAsync();

                }
                else
                {
                    throw new RestException("CORRESPONDENT_NOT_FOUND");
                }
                response.Correspondent = DBUtils.GetCorrespondentFromDB(newCorr.SYSTEM_ID.ToString(), _pi3DbContext);
                // prelievo mail:
                if (response.Correspondent != null && !string.IsNullOrWhiteSpace(response.Correspondent.Id))
                {
                    var emails = DBUtils.getEmailsCorrEsterno(response.Correspondent.Id, _pi3DbContext);
                    List<string> retvalEmails = new List<string>();
                    if (emails != null && emails.Any())
                    {
                        foreach (var email in emails)
                        {
                            if (email.Email.ToUpper() != response.Correspondent.Email.ToUpper()) retvalEmails.Add(email.Email);
                        }
                        if (retvalEmails.Count > 0) response.Correspondent.OtherEmails = retvalEmails;
                    }
                }
                await _loggerService.LogOK("CORRISPONDENTIDELETECORRISPONDENTEESTERNO",
                        response.Correspondent.Id, string.Format("PIS REST: Modifica del corrispondente {0} in rubrica", response.Correspondent.Code),
                        null, infoUtente.codWorkingApplication);
                #endregion

                response.Code = GetCorrespondentResponseCode.OK;

                _logger.LogInformation("end EditCorrespondent");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione EditCorrespondent: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new EditCorrespondentCommandResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione EditCorrespondent");
                response = new EditCorrespondentCommandResponse();
                response.Code = GetCorrespondentResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<EditCorrespondentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}
