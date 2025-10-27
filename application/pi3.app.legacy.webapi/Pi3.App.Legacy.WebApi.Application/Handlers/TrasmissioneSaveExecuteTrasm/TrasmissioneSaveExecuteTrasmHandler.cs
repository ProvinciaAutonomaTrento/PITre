// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trasmissione = Pi3.Core.AggregateModels.TrasmissioneAggregate.Trasmissione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneSaveExecuteTrasm
{

    // Richiede libreria MediatR
    public class TrasmissioneSaveExecuteTrasmHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneSaveExecuteTrasm, TrasmissioneSaveExecuteTrasmResult>
    {
        #region Public Members

        public TrasmissioneSaveExecuteTrasmHandler(ILogger<TrasmissioneSaveExecuteTrasmHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }


        public async Task<TrasmissioneSaveExecuteTrasmResult> Handle(Application.Requests.TrasmissioneSaveExecuteTrasm request, CancellationToken cancellationToken)
        {
            var idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var idUser = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);

            Trasmissione aggregate = null;
            DocsPaVO.trasmissione.Trasmissione trasmissione = request.trasmissione;

            try
            {              
                
                if (!string.IsNullOrWhiteSpace(trasmissione.systemId))
                {
                    if (await _trasmissioneRepository.Exists(idAmm, trasmissione.systemId))
                    {
                        aggregate = await _trasmissioneRepository.Get(idAmm, trasmissione.systemId);

                        await _trasmissioneRepository.Delete(aggregate);

                    }
                }

                aggregate = new Trasmissione(
                idAmm,
                DateTime.Now,
                trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO ? trasmissione.infoDocumento.idProfile : trasmissione.infoFascicolo.idFascicolo,
                (TipiOggettiTrasmessiEnum)trasmissione.tipoOggetto,
                null,
                new Core.SeedWork.TextValue(trasmissione.noteGenerali));

                var cessioneImpostata = trasmissione.cessione != null && trasmissione.cessione.docCeduto;
                var accessRights = await _dbContext.GetSecurityRights(aggregate.OggettoTrasmesso.Id, idUser, idGroup);

                foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasmissione.trasmissioniSingole)
                {
                    var tipoCorr = _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.SYSTEM_ID == ts.corrispondenteInterno.systemId.AsLong()).Select(s => new
                    {
                        s.CHA_TIPO_URP,
                        s.ID_PEOPLE,
                        s.ID_GRUPPO,
                        s.VAR_COD_RUBRICA,
                        s.VAR_NOME,
                        s.VAR_COGNOME,
                        s.VAR_DESC_CORR
                    }).FirstOrDefault();

                    if (tipoCorr.CHA_TIPO_URP == "P")
                    {
                        #region prepara trasmissione singola per le persone
                        DatiTrasmissioneSingolaUtente datiU = new DatiTrasmissioneSingolaUtente()
                        {
                            Cognome = tipoCorr.VAR_COGNOME,
                            UserId = tipoCorr.VAR_DESC_CORR,
                            Nome = tipoCorr.VAR_NOME,
                            IdUtente = tipoCorr.ID_PEOPLE.ToString(),
                            IdRagioneTrasmissione = ts.ragione.systemId,
                            NomeRagioneTrasmissione = ts.ragione.descrizione,
                            DataScadenza = !string.IsNullOrWhiteSpace(ts.dataScadenza) ? ts.dataScadenza.AsDateTime() : null,
                            NascondiVersioniPrecedenti = ts.hideDocumentPreviousVersions,
                            Note = !string.IsNullOrWhiteSpace(ts.noteSingole) ? new TextValue(ts.noteSingole) : null,
                            RagioneConWorkflow = ts.ragione.isTipoTask,
                            CessioneDirittiRagione = !cessioneImpostata ? null : new CessioneDirittiRagione
                            {
                                MantieniLettura = ts.ragione.mantieniLettura == "1",
                                MantieniScrittura = ts.ragione.mantieniScrittura == "1"
                            }

                        };

                        aggregate.PrepareTrasmissioneSingolaUtente(datiU);

                        #endregion

                    }
                    else
                    {
                        #region prepara trasmissione singola per i non Persona

                        List<DatiUtenteNotificatoTrasmissioneSingolaGruppo> utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();

                        foreach(TrasmissioneUtente trasmUtente in ts.trasmissioneUtente.Where(u => u.daNotificare))
                        {
                            bool isUserDisabled = string.IsNullOrEmpty(trasmUtente.utente.disabilitato) 
                                ? await this._dbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == trasmUtente.utente.idPeople.AsLong() && p.ID_AMM == trasmUtente.utente.idAmministrazione.AsLong() && p.DISABLED == "Y")
                                : trasmUtente.utente.disabilitato.Equals("Y");

                            if (!isUserDisabled)
                            {
                                utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                {
                                    IdUtente = trasmUtente.utente.idPeople,
                                    UserId = trasmUtente.utente.userId,
                                    Cognome = trasmUtente.utente.cognome,
                                    Nome = trasmUtente.utente.nome
                                });
                            }
                        }

                        if(cessioneImpostata && ts.ragione.prevedeCessione != "N")
                        {
                            utentiNotificati = new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();
                            utentiNotificati.Add(new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                            {
                                IdUtente = trasmissione.cessione.idPeopleNewPropr
                            });
                        }

                        if (utentiNotificati != null && utentiNotificati.Count() > 0)
                        {
                            DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                            {
                                CodiceGruppoDestinatario = tipoCorr.VAR_COD_RUBRICA,
                                DescrizioneGruppoDestinatario = new TextValue(tipoCorr.VAR_DESC_CORR),
                                IdGruppoDestinatario = tipoCorr.ID_GRUPPO.ToString(),
                                IdRagioneTrasmissione = ts.ragione.systemId,
                                NomeRagioneTrasmissione = ts.ragione.descrizione,
                                DataScadenza = !string.IsNullOrWhiteSpace(ts.dataScadenza) ? ts.dataScadenza.AsDateTime() : null,
                                NascondiVersioniPrecedenti = ts.hideDocumentPreviousVersions,
                                Note = !string.IsNullOrWhiteSpace(ts.noteSingole) ? new TextValue(ts.noteSingole) : null,
                                RagioneConWorkflow = ts.ragione.isTipoTask,
                                Tipo = ts.tipoTrasm == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                                UtentiNotificati = utentiNotificati,
                                CessioneDirittiRagione = !cessioneImpostata ? null : new CessioneDirittiRagione
                                {
                                    MantieniLettura = ts.ragione.mantieniLettura == "1",
                                    MantieniScrittura = ts.ragione.mantieniScrittura == "1"
                                }
                            };

                            aggregate.PrepareTrasmissioneSingolaGruppo(datiG);
                        }

                        #endregion
                    }
                }

                //aggregate.Invia(DateTime.Now);
                await _trasmissioneRepository.Add(aggregate);

                InviaBehavior? inviaBehavior = request.trasmissione.NO_NOTIFY == "1" ? new InviaBehavior()
                {
                    InibisciNotifiche = true
                } : null;

                aggregate.Invia(DateTime.Now, inviaBehavior);

                await _trasmissioneRepository.Update(aggregate);


                #region  WebMethodLog esito OK

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    var method = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_")
                        : "TRASM_FOLDER_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_");

                    var objectDescription = string.Empty;
                    if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
                    {
                        objectDescription = trasmissione.infoDocumento.segnatura != null ? trasmissione.infoDocumento.segnatura : trasmissione.infoDocumento.docNumber;
                    }
                    else
                    {
                        objectDescription = trasmissione.infoFascicolo.idFascicolo;
                    }

                    await this._webMethodLoggerService.LogOK(method, aggregate.OggettoTrasmesso.Id, 
                        string.Format(LogEvent.DocTrasm, objectDescription), 
                        ts.Id, null,
                        request.trasmissione.NO_NOTIFY == "1");
                }

                if(cessioneImpostata)
                {
                    if(accessRights == SecurityRightTypesEnum.FullControl)
                        await this._webMethodLoggerService.LogOK("EXECSENDERRIGTHS_D", aggregate.OggettoTrasmesso.Id, string.Format(LogEvent.LogCessioneDirittiProprieta), aggregate.OggettoTrasmesso.Id);
                    else
                        await this._webMethodLoggerService.LogOK("EXECSENDERRIGTHS_D", aggregate.OggettoTrasmesso.Id, string.Format(LogEvent.LogCessioneDirittiAcquisiti), aggregate.OggettoTrasmesso.Id);
                }
                #endregion


            }
            catch (Exception ex)
            {
                #region WebMethodLog esito KO
                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
                {
                    if (trasmissione.infoDocumento.segnatura != null)
                        await this._webMethodLoggerService.LogKO("DOCUMENTOTRASMESSO", aggregate.OggettoTrasmesso.Id, string.Format(LogEvent.DocTrasm, trasmissione.infoDocumento.segnatura));
                    else
                        await this._webMethodLoggerService.LogKO("DOCUMENTOTRASMESSO", aggregate.OggettoTrasmesso.Id, string.Format(LogEvent.DocTrasm, trasmissione.infoDocumento.docNumber));
                }
                #endregion

                this._logger.LogError(exception: ex, message: ex.Message);

                trasmissione = null;
            }

            return new TrasmissioneSaveExecuteTrasmResult(trasmissione);

        }

        #endregion

        #region Private Members

        private void PrepareTrasmSing(ref Trasmissione trasm, DocsPaVO.trasmissione.Trasmissione trasmissione, string idAmm)
        {
            Autore autore = new Autore()
            {
                IdGruppo = trasmissione.ruolo.systemId,
                IdUtente = trasmissione.utente.systemId,
            };

            if (!string.IsNullOrEmpty(trasmissione.systemId))
            {
                trasm = new Trasmissione(
                trasmissione.systemId,
                idAmm,
                DateTime.Now,
                trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO ? trasmissione.infoDocumento.idProfile : trasmissione.infoFascicolo.idFascicolo,
                (TipiOggettiTrasmessiEnum)trasmissione.tipoOggetto,
                autore,
                new Core.SeedWork.TextValue(trasmissione.noteGenerali));
            }
            else
            {
                trasm = new Trasmissione(
                idAmm,
                DateTime.Now,
                trasmissione.tipoOggetto == DocsPaVO.trasmissione.TipoOggetto.DOCUMENTO ? trasmissione.infoDocumento.idProfile : trasmissione.infoFascicolo.idFascicolo,
                (TipiOggettiTrasmessiEnum)trasmissione.tipoOggetto,
                autore,
                new Core.SeedWork.TextValue(trasmissione.noteGenerali));
            }

            //PrepareTrasmSing(ref trasm, trasmissione);

            foreach (DocsPaVO.trasmissione.TrasmissioneSingola ts in trasmissione.trasmissioniSingole)
            {
                var tipoCorr = _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.SYSTEM_ID == ts.corrispondenteInterno.systemId.AsLong()).Select(s => new
                {
                    s.CHA_TIPO_URP,
                    s.ID_PEOPLE,
                    s.ID_GRUPPO,
                    s.VAR_COD_RUBRICA,
                    s.VAR_NOME,
                    s.VAR_COGNOME,
                    s.VAR_DESC_CORR
                }).FirstOrDefault();

                if (tipoCorr.CHA_TIPO_URP == "P")
                {
                    #region prepara trasmissione singola per le persone

                    DatiTrasmissioneSingolaUtente datiU = new DatiTrasmissioneSingolaUtente()
                    {
                        Cognome = tipoCorr.VAR_COGNOME,
                        UserId = tipoCorr.VAR_DESC_CORR,
                        Nome = tipoCorr.VAR_NOME,
                        IdUtente = tipoCorr.ID_PEOPLE.ToString(),
                        IdRagioneTrasmissione = ts.ragione.systemId,
                        NomeRagioneTrasmissione = ts.ragione.descrizione,
                        DataScadenza = !string.IsNullOrWhiteSpace(ts.dataScadenza) ? ts.dataScadenza.AsDateTime() : null,
                        NascondiVersioniPrecedenti = ts.hideDocumentPreviousVersions,
                        Note = !string.IsNullOrWhiteSpace(ts.noteSingole) ? new TextValue(ts.noteSingole) : null,
                        RagioneConWorkflow = ts.ragione.isTipoTask

                    };

                    trasm.PrepareTrasmissioneSingolaUtente(datiU);

                    #endregion

                }
                else
                {
                    #region prepara trasmissione singola per i non Persona
                    var utentiNotificati =
                        (ts.trasmissioneUtente?
                            .OfType<DocsPaVO.trasmissione.TrasmissioneUtente>()
                            .ToArray()
                            .Select(u => new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                            {
                                IdUtente = u.utente.systemId,
                                UserId = u.utente.userId,
                                Cognome = u.utente.cognome,
                                Nome = u.utente.nome
                            }))
                            .ToList();

                    DatiTrasmissioneSingolaGruppo datiG = new DatiTrasmissioneSingolaGruppo()
                    {
                        CodiceGruppoDestinatario = tipoCorr.VAR_COD_RUBRICA,
                        DescrizioneGruppoDestinatario = new TextValue(tipoCorr.VAR_DESC_CORR),
                        IdGruppoDestinatario = tipoCorr.ID_GRUPPO.ToString(),
                        IdRagioneTrasmissione = ts.ragione.systemId,
                        NomeRagioneTrasmissione = ts.ragione.descrizione,
                        DataScadenza = !string.IsNullOrWhiteSpace(ts.dataScadenza) ? ts.dataScadenza.AsDateTime() : null,
                        NascondiVersioniPrecedenti = ts.hideDocumentPreviousVersions,
                        Note = !string.IsNullOrWhiteSpace(ts.noteSingole) ? new TextValue(ts.noteSingole) : null,
                        RagioneConWorkflow = ts.ragione.isTipoTask,
                        Tipo = ts.tipoTrasm == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                        UtentiNotificati = utentiNotificati
                    };

                    trasm.PrepareTrasmissioneSingolaGruppo(datiG);

                    #endregion
                }
            }

            _trasmissioneRepository.Add(trasm);
        }

        protected readonly ILogger<TrasmissioneSaveExecuteTrasmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }

}
