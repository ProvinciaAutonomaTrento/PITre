// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.AddressBook.AddCorrespondentAdvanced
{
    // Richiede libreria MediatR
    public class AddCorrespondentAdvancedCommandHandler : IRequestHandler<AddCorrespondentAdvancedCommand, AddCorrespondentAdvancedCommandResponse>
    {
        #region Public Members

        public AddCorrespondentAdvancedCommandHandler(ILogger<AddCorrespondentAdvancedCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<AddCorrespondentAdvancedCommandResponse> Handle(AddCorrespondentAdvancedCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("AddCorrespondentAdvanced - START");

            AddCorrespondentAdvancedCommandResponse response = new AddCorrespondentAdvancedCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
                if (request.Correspondent == null)
                {
                    //Corrispondente non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }

                if (string.IsNullOrEmpty(request.Correspondent.CorrespondentType) || (!request.Correspondent.CorrespondentType.Equals("U") && !request.Correspondent.CorrespondentType.Equals("P")))
                {
                    //Inserire il tipo U o P
                    throw new RestException("REQUIRED_CORRESPONDENTTYPE");
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

                if (request.Correspondent.EmailsDetailed != null && request.Correspondent.EmailsDetailed.Count > 0)
                {
                    // Controllo lista email inserite
                    var mainEmail = request.Correspondent.EmailsDetailed.Where(r => r.Main == "1").ToList();
                    if (mainEmail != null && mainEmail.Count > 0 && mainEmail.Count < 2)
                    {
                        var emailprincipale = mainEmail.FirstOrDefault();
                        if (emailprincipale != null)
                        {
                            if (request.Correspondent.Email.ToLower().Trim() != emailprincipale.Email.ToLower().Trim())
                                throw new RestException("EMAILSDETAILED_ERROR2");

                        }
                        else
                        {
                            throw new RestException("EMAILSDETAILED_ERROR1");
                        }
                    }
                    else
                    {
                        throw new RestException("EMAILSDETAILED_ERROR1");
                    }
                }

                if (string.IsNullOrEmpty(request.Correspondent.Code) && (request.Correspondent.CorrespondentType.ToUpper() != "O"))
                {
                    throw new RestException("REQUIRED_CODE_CORREPONDENT");
                }
                #endregion

                #region implementazione
                if (request.Correspondent.CorrespondentType.ToUpper() == "O")
                {
                    // gestione occasionali
                    var idCorrOcc = await DBUtils.AddCorrispondenteOccasionale(request.Correspondent, infoUtente, _pi3DbContext);
                    if (idCorrOcc != null)
                    {
                        var corrResp = DBUtils.GetCorrespondentFromDB(idCorrOcc, _pi3DbContext);
                        response.Correspondent = JsonConvert.DeserializeObject<CorrespondentAdvanced>(JsonConvert.SerializeObject(corrResp));
                        var emails = DBUtils.getEmailsCorrEsterno(corrResp.Id, _pi3DbContext);
                        if (emails != null && emails.Any())
                        {
                            List<string> retvalEmails = new List<string>();
                            response.Correspondent.EmailsDetailed = emails;
                            foreach (var email in emails)
                            {
                                if (email.Email.ToUpper() != corrResp.Email.ToUpper()) retvalEmails.Add(email.Email);

                            }
                            if (retvalEmails.Count > 0) response.Correspondent.OtherEmails = retvalEmails;
                        }
                    }
                }
                else
                {

                    DocsPaVO.utente.Registro reg = null;
                    if (!string.IsNullOrWhiteSpace(request.Correspondent.CodeRegisterOrRF))
                    {
                        reg = DBUtils.getRegistroByCodAOO(request.Correspondent.CodeRegisterOrRF, infoUtente.idAmministrazione, _pi3DbContext);
                        if (reg == null) throw new RestException("REGISTER_NOT_FOUND");
                    }

                    if (!string.IsNullOrWhiteSpace(request.Correspondent.Code))
                    {
                        var verificaCodice = _pi3DbContext.CorrGlobaliEntities.Where(
                            x => x.VAR_COD_RUBRICA.ToUpper() == request.Correspondent.Code.ToUpper() &&
                            x.ID_AMM == infoUtente.idAmministrazione.AsLong() &&
                            x.DTA_FINE == null &&
                            (reg == null || Int64.Parse(reg.systemId) == x.ID_REGISTRO)).ToList();
                        if (verificaCodice.Any()) throw new RestException("CODE_CORRESPONDENT_EXISTS");
                    }

                    var listaCanali = _pi3DbContext.DocumentTypesEntities.ToList();
                    var idCanaleLettera = listaCanali.Where(x => x.DESCRIPTION.ToUpper() == "LETTERA").Select(x => x.SYSTEM_ID).FirstOrDefault();
                    long idCanaleSelezionato = 0;
                    if (!string.IsNullOrWhiteSpace(request.Correspondent.PreferredChannel))
                    {
                        idCanaleSelezionato = listaCanali.Where(x => x.DESCRIPTION.ToUpper() == request.Correspondent.PreferredChannel.ToUpper()).Select(x => x.SYSTEM_ID).FirstOrDefault();
                    }
                    string descrizione = request.Correspondent.Description;
                    if (request.Correspondent.CorrespondentType == "P")
                    {
                        descrizione = string.Format("{0} {1}", request.Correspondent.Surname, request.Correspondent.Name);
                    }

                    // inserimento in DPA_CORR_GLOBALI

                    CorrGlobaliEntity corr = new CorrGlobaliEntity()
                    {
                        VAR_COD_RUBRICA = request.Correspondent.Code,
                        VAR_CODICE = request.Correspondent.Code,
                        CHA_TIPO_URP = request.Correspondent.CorrespondentType.ToUpper(),
                        CHA_TIPO_IE = "E",
                        VAR_DESC_CORR = descrizione,
                        VAR_DESC_CORR_OLD = descrizione,
                        VAR_EMAIL = request.Correspondent.Email,
                        VAR_NOME = request.Correspondent.CorrespondentType == "P" ? request.Correspondent.Name : null,
                        VAR_COGNOME = request.Correspondent.CorrespondentType == "P" ? request.Correspondent.Surname : null,
                        ID_REGISTRO = reg != null ? reg.systemId.AsLong() : null,
                        ID_AMM = infoUtente.idAmministrazione.AsLong(),
                        DTA_INIZIO = DateTime.Now,
                        CHA_DETTAGLI = "1",
                        CHA_TIPO_CORR = "S",
                        CHA_PA = "1",
                        ID_PARENT = 0,
                        ID_OLD = 0,
                        VAR_CHIAVE_AE = "0",
                        CHA_SYSTEM_ROLE = "0"
                    };
                    _pi3DbContext.CorrGlobaliEntities.Add(corr);
                    _logger.LogDebug("Corrispondente inserito con ID: " + corr.SYSTEM_ID);

                    // inserimento in DPA_DETT_GLOBALI
                    DettGlobaliEntity dett = new DettGlobaliEntity()
                    {
                        ID_CORR_GLOBALI = corr.SYSTEM_ID,
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
                    // vengono prese in esame le sole mail configurate nella lista EmailsDetailed
                    if (request.Correspondent.EmailsDetailed != null && request.Correspondent.EmailsDetailed.Any())
                    {
                        foreach (var email in request.Correspondent.EmailsDetailed)
                        {
                            await _pi3DbContext.MailCorrEsterniEntities.AddAsync(new MailCorrEsterniEntity()
                            {
                                ID_CORR = corr.SYSTEM_ID,
                                VAR_EMAIL = email.Email,
                                VAR_PRINCIPALE = email.Main,
                                VAR_NOTE = email.Note
                            });
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(request.Correspondent.Email))
                    {
                        MailCorrEsterniEntity mail = new MailCorrEsterniEntity()
                        {
                            ID_CORR = corr.SYSTEM_ID,
                            VAR_EMAIL = request.Correspondent.Email,
                            VAR_PRINCIPALE = "1"
                        };
                        await _pi3DbContext.MailCorrEsterniEntities.AddAsync(mail);

                    }

                    // inserimento in DPA_T_CANALE_CORR
                    CanaleCorrEntity canaleCorr = new CanaleCorrEntity()
                    {
                        ID_CORR_GLOBALE = corr.SYSTEM_ID,
                        ID_DOCUMENTTYPE = idCanaleSelezionato > 0 ? idCanaleSelezionato : idCanaleLettera,
                        CHA_PREFERITO = "1"
                    };
                    await _pi3DbContext.CanaleCorrEntities.AddAsync(canaleCorr);
                    await ((DbContext)this._pi3DbContext).SaveChangesAsync();

                    var corrResp = DBUtils.GetCorrespondentFromDB(corr.SYSTEM_ID.ToString(), _pi3DbContext);
                    if (corrResp != null && !string.IsNullOrWhiteSpace(corrResp.Id))
                    {
                        response.Correspondent = JsonConvert.DeserializeObject<CorrespondentAdvanced>(JsonConvert.SerializeObject(corrResp));
                        var emails = DBUtils.getEmailsCorrEsterno(corrResp.Id, _pi3DbContext);
                        if (emails != null && emails.Any())
                        {
                            List<string> retvalEmails = new List<string>();
                            response.Correspondent.EmailsDetailed = emails;
                            foreach (var email in emails)
                            {
                                if (email.Email.ToUpper() != corrResp.Email.ToUpper()) retvalEmails.Add(email.Email);

                            }
                            if (retvalEmails.Count > 0) response.Correspondent.OtherEmails = retvalEmails;
                        }
                    }
                }
                #endregion
                await _loggerService.LogOK("RUBRICAINSCORR",
                        response.Correspondent.Id, string.Format("PIS REST: Inserimento corrispondente {0} in rubrica", response.Correspondent.Code),
                        null, infoUtente.codWorkingApplication);
                response.Code = CorrAdvDetailsResponseCode.OK;

                _logger.LogInformation("end AddCorrespondentAdvanced");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione AddCorrespondentAdvanced: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new AddCorrespondentAdvancedCommandResponse();
                response.Code = CorrAdvDetailsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione AddCorrespondentAdvanced");
                response = new AddCorrespondentAdvancedCommandResponse();
                response.Code = CorrAdvDetailsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddCorrespondentAdvancedCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}
