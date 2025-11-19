// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Transmissions.ExecuteTransmissionProject
{
    // Richiede libreria MediatR
    public class ExecuteTransmissionProjectCommandHandler : IRequestHandler<ExecuteTransmissionProjectCommand, ExecuteTransmissionProjectCommandResponse>
    {
        #region Public Members

        public ExecuteTransmissionProjectCommandHandler(ILogger<ExecuteTransmissionProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, ITrasmissioneRepository trasmissioneRepository, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<ExecuteTransmissionProjectCommandResponse> Handle(ExecuteTransmissionProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("ExecuteTransmissionProject - START");

            ExecuteTransmissionProjectCommandResponse response = new ExecuteTransmissionProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region Controllo parametri richiesta
                if (string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ID");
                }


                if (request.Receiver == null)
                {
                    //Destinatario non trovato
                    throw new RestException("REQUIRED_CORRESPONDENT");
                }
                if (string.IsNullOrEmpty(request.Receiver.Code) && string.IsNullOrEmpty(request.Receiver.Id))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_RECEIVER");
                }
                if (request.TransmissionReason == null || string.IsNullOrEmpty(request.TransmissionReason))
                {
                    throw new RestException("REQUIRED_TRANSMISSION_REASON");
                }
                else if ((!string.IsNullOrEmpty(infoUtente.diSistema)) && (infoUtente.diSistema == "1") && (request.TransmissionReason != "COMPETENZA_SIST_ESTERNI"))
                {
                    throw new Exception("Ragione di trasmissione non valida per un sistema esterno. Ragione disponibile: COMPETENZA_SIST_ESTERNI");
                }


                #endregion

                #region implementazione
                try
                {
                    await _pi3DbContext.AssertSecurityRights(request.IdProject, infoUtente.idPeople, infoUtente.idGruppo);
                }
                catch (Exception security) { throw new RestException("PROJECT_NOT_FOUND"); }

                var ragioneTrasm = (from a in _pi3DbContext.RagioneTrasmissioneEntities where a.VAR_DESC_RAGIONE.ToUpper() == request.TransmissionReason.ToUpper() && a.ID_AMM == infoUtente.idAmministrazione.AsLong() select a).FirstOrDefault();
                if (ragioneTrasm == null || ragioneTrasm.SYSTEM_ID == 0) { throw new RestException("TRANSMISSION_REASON_NOT_FOUND"); }

                CorrGlobaliEntity dest = null;
                if (!string.IsNullOrWhiteSpace(request.Receiver.Id))
                {
                    dest = (from a in _pi3DbContext.CorrGlobaliEntities where a.SYSTEM_ID == request.Receiver.Id.AsLong() && a.DTA_FINE == null select a).FirstOrDefault();
                }
                else
                {
                    dest = (from a in _pi3DbContext.CorrGlobaliEntities where a.VAR_COD_RUBRICA.ToUpper() == request.Receiver.Code.ToUpper() && a.ID_AMM == infoUtente.idAmministrazione.AsLong() && a.DTA_FINE == null select a).FirstOrDefault();
                }
                if (dest == null || dest.SYSTEM_ID == 0 ||
                    dest.CHA_TIPO_IE == "E" || dest.DTA_FINE != null) throw new RestException("CORRESPONDENT_NOT_FOUND");
                if (dest.CHA_TIPO_URP == "U")
                {
                    var destRuolo = (from a in _pi3DbContext.CorrGlobaliEntities where a.ID_UO == dest.SYSTEM_ID && a.CHA_RIFERIMENTO == "1" && a.DTA_FINE == null select a).FirstOrDefault();
                    if (destRuolo != null)
                        dest = destRuolo;
                    else throw new RestException("CORRESPONDENT_NOT_FOUND");
                }

                //TODO: Cessione diritti

                var aggregate = new Core.AggregateModels.TrasmissioneAggregate.Trasmissione(infoUtente.idAmministrazione, DateTime.Now, request.IdProject, TipiOggettiTrasmessiEnum.Fascicolo, new Autore() { IdUtente = infoUtente.idPeople, IdGruppo = infoUtente.idGruppo }
                );

                if (dest.CHA_TIPO_URP == "P")
                {
                    DatiTrasmissioneSingolaUtente datiU = new DatiTrasmissioneSingolaUtente()
                    {
                        Cognome = dest.VAR_COGNOME,
                        UserId = dest.VAR_COD_RUBRICA,
                        Nome = dest.VAR_NOME,
                        IdUtente = dest.ID_PEOPLE.ToString(),
                        IdRagioneTrasmissione = ragioneTrasm.SYSTEM_ID.ToString(),
                        NomeRagioneTrasmissione = ragioneTrasm.VAR_DESC_RAGIONE,
                        DataScadenza = null,
                        NascondiVersioniPrecedenti = false,
                        Note = null,
                        RagioneConWorkflow = ragioneTrasm.CHA_TIPO_RAGIONE == "W"
                    };
                    aggregate.PrepareTrasmissioneSingolaUtente(datiU);
                }
                else if (dest.CHA_TIPO_URP == "R")
                {
                    var utentiInRuolo = from p in _pi3DbContext.PeopleEntities
                                        where _pi3DbContext.PeopleGroupEntities
                                            .Where(pg => pg.GROUPS_SYSTEM_ID == dest.ID_GRUPPO && pg.DTA_FINE == null)
                                            .Select(pg => pg.PEOPLE_SYSTEM_ID)
                                            .Contains(p.SYSTEM_ID)
                                        select p;

                    var utentiNotificati =
                            (utentiInRuolo.Select(u => new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                            {
                                IdUtente = u.SYSTEM_ID.ToString(),
                                UserId = u.USER_ID,
                                Cognome = u.VAR_COGNOME,
                                Nome = u.VAR_NOME
                            }))
                                .ToList();

                    DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                    {
                        CodiceGruppoDestinatario = dest.VAR_COD_RUBRICA,
                        DescrizioneGruppoDestinatario = new TextValue(dest.VAR_DESC_CORR),
                        IdGruppoDestinatario = dest.ID_GRUPPO.ToString(),
                        IdRagioneTrasmissione = ragioneTrasm.SYSTEM_ID.ToString(),
                        NomeRagioneTrasmissione = ragioneTrasm.VAR_DESC_RAGIONE,
                        DataScadenza = null,
                        NascondiVersioniPrecedenti = false,
                        Note = null,
                        RagioneConWorkflow = ragioneTrasm.CHA_TIPO_RAGIONE == "W",
                        Tipo = request.TransmissionType == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                        UtentiNotificati = utentiNotificati
                    };

                    aggregate.PrepareTrasmissioneSingolaGruppo(datiG);
                }
                await _trasmissioneRepository.Add(aggregate);

                var createdAggregate = await _trasmissioneRepository.Get(infoUtente.idAmministrazione, aggregate.Id);

                InviaBehavior? inviaBehavior = !request.Notify ? new InviaBehavior()
                {
                    InibisciNotifiche = true
                } : null;

                createdAggregate.Invia(DateTime.Now, inviaBehavior);

                await _trasmissioneRepository.Update(createdAggregate);
                bool notifica = true;
                if (request.Notify != null && !request.Notify)
                {
                    notifica = false;
                }
                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    await _loggerService.LogOK("TRASM_FOLDER_" + ragioneTrasm.VAR_DESC_RAGIONE.ToUpper().Replace(" ", "_"), aggregate.OggettoTrasmesso.Id, $"PIS REST: Trasmesso Fascicolo {aggregate.OggettoTrasmesso.Id}", ts.Id, infoUtente.codWorkingApplication, !notifica);
                }

                #endregion
                response.TransmMessage = "Trasmissione effettuata";
                response.Code = TransmissionResponseCode.OK;

                _logger.LogInformation("end ExecuteTransmissionProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione ExecuteTransmissionProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new ExecuteTransmissionProjectCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione ExecuteTransmissionProject");
                response = new ExecuteTransmissionProjectCommandResponse();
                response.Code = TransmissionResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExecuteTransmissionProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;

        #endregion
    }

}
