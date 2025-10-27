// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Smistamento;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Events;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Events;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmistaDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.SmistaDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SmistaDocumento
{


    public class SmistaDocumentoHandler : IRequestHandler<SmistaDocumentoRequest, SmistaDocumentoResult>
    {
        #region Public Members

        public SmistaDocumentoHandler(
            ILogger<SmistaDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }


        public async Task<SmistaDocumentoResult> Handle(SmistaDocumentoRequest request, CancellationToken cancellationToken)
        {
            var output = new List<DocsPaVO.Smistamento.EsitoSmistamentoDocumento>();

            try
            {
                this.ValidateAlmostOneChecked(request);

                this.ValidateRegistriRuoli(request);

                var trasmissioneAggregate = await
                    this._trasmissioneRepository.Get(request.infoUtente.idAmministrazione, request.datiTrasmissioneDocumento.IDTrasmissione);

                if (request.datiTrasmissioneDocumento.TrasmissioneConWorkflow)
                {
                    trasmissioneAggregate.Accetta(request.infoUtente.idGruppo,
                        request.infoUtente.idPeople,
                        new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Accetta()
                        {
                            Data = DateTime.Now,
                            IdDelegato = (request.infoUtente.delegato != null ?
                                    request.infoUtente.delegato.idPeople : null),
                            Note = new TextValue(Descriptions.NoteAccettazioneAutomatica)
                        });
                }
                else
                {
                    trasmissioneAggregate.Visto(request.infoUtente.idGruppo,
                        request.infoUtente.idPeople,
                        new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Visto()
                        {
                            Data = DateTime.Now,
                            IdDelegato = (request.infoUtente.delegato != null ?
                                    request.infoUtente.delegato.idPeople : null)
                        });
                }

                await this._trasmissioneRepository.Update(trasmissioneAggregate);

                await this.TrasmettiUOSmistamento(request);

                await this.TrasmettiModelloTrasmissione(request);

                await this._webMethodLoggerService.LogOK
                    (webMethodName: Descriptions.WebMethodName,
                     idObject: request.documentoTrasmesso.IDDocumento,
                     objectDescription:
                            string.IsNullOrWhiteSpace(request.documentoTrasmesso.Segnatura) ?
                                    String.Format(Descriptions.ObjectDescription, request.documentoTrasmesso.IDDocumento) :
                                    String.Format(Descriptions.ObjectDescriptionSegnatura, request.documentoTrasmesso.Segnatura));

                output.Add(new EsitoSmistamentoDocumento()
                {
                    CodiceEsitoSmistamento = 0
                });
            }
            catch (SmistaDocumentoPi3Exception smistaDocumentoPi3Ex)
            {
                output.Add(smistaDocumentoPi3Ex.EsitoSmistamentoDocumento);

                this._logger.LogError(exception: smistaDocumentoPi3Ex, message: smistaDocumentoPi3Ex.Message);
            }
            catch (Pi3Exception pi3Ex)
            {
                output.Add(new EsitoSmistamentoDocumento()
                {
                    CodiceEsitoSmistamento = -1,
                    DescrizioneEsitoSmistamento = $"{pi3Ex.ErrorCode} - {pi3Ex.Message}"
                });

                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output.Add(new EsitoSmistamentoDocumento()
                {
                    CodiceEsitoSmistamento = -1,
                    DescrizioneEsitoSmistamento = ex.Message
                });

                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new SmistaDocumentoResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SmistaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        private void ValidateAlmostOneChecked(SmistaDocumentoRequest request)
        {
            if (!this.AlmostOneCheckedInUO(request.uoAppartenenza))
            {
                throw new SmistaDocumentoPi3Exception(new EsitoSmistamentoDocumento()
                {
                    CodiceEsitoSmistamento = -1,
                    DescrizioneEsitoSmistamento = ErrorDescriptions.NessunDestinatarioSelezionato
                });
            }
        }
        private bool AlmostOneCheckedInUO(params DocsPaVO.Smistamento.UOSmistamento[] uos)
        {
            bool almostOne = false;

            foreach (var uo in uos)
            {
                if (almostOne)
                    break;

                if (uo.FlagCompetenza || uo.FlagConoscenza)
                {
                    almostOne = true;
                    break;
                }
                else
                {
                    almostOne = this.AlmostOneCheckedInRuoli(uo.Ruoli);

                    if (!almostOne)
                    {
                        foreach (var uoInferiore in uo.UoInferiori)
                        {
                            almostOne = this.AlmostOneCheckedInUO(uoInferiore);

                            if (almostOne)
                                break;
                        }
                    }
                    else
                        break;
                }

                if (!almostOne &&
                    uo.UoSmistaTrasAutomatica != null && uo.UoSmistaTrasAutomatica.Any())
                {
                    almostOne = true;
                    break;
                }
            }

            return almostOne;
        }

        private bool AlmostOneCheckedInRuoli(params DocsPaVO.Smistamento.RuoloSmistamento[] ruoli)
        {
            bool almostOne = false;

            foreach (var ruolo in ruoli)
            {
                if (ruolo.FlagCompetenza || ruolo.FlagConoscenza)
                {
                    almostOne = true;
                    break;
                }
                else
                {
                    almostOne = AlmostOneUtenteChecked(ruolo.Utenti);

                    if (almostOne)
                        break;
                }
            }

            return almostOne;
        }

        private bool AlmostOneUtenteChecked(params DocsPaVO.Smistamento.UtenteSmistamento[] utenti)
        {
            int countComp = utenti.Count(e => e.FlagCompetenza == true);
            int countCC = utenti.Count(e => e.FlagConoscenza == true);

            return (countComp > 0 || countCC > 0);
        }

        private void ValidateRegistriRuoli(SmistaDocumentoRequest request)
        {
            this.ValidateRegistriRuoli(request.documentoTrasmesso.IDRegistro, request.uoAppartenenza);
        }

        private void ValidateRegistriRuoli(string idRegistroDocumentoTrasmesso, UOSmistamento uoSmistamento)
        {
            foreach (var ruolo in uoSmistamento.Ruoli.Where(r => r.FlagCompetenza || r.FlagConoscenza))
            {
                if (string.IsNullOrWhiteSpace(idRegistroDocumentoTrasmesso)
                        || idRegistroDocumentoTrasmesso == 0.ToString()
                        || (ruolo.Registri.Any()
                            && !ruolo.Registri.Contains(idRegistroDocumentoTrasmesso ?? String.Empty)))
                {
                    throw new SmistaDocumentoPi3Exception(new EsitoSmistamentoDocumento()
                    {
                        CodiceEsitoSmistamento = 99,
                        DescrizioneEsitoSmistamento = ErrorDescriptions.TrasmissioneSuRegistroDifferente,
                        DenominazioneDestinatario = ruolo.Descrizione
                    });
                }
            }

            foreach (var uoInferiore in (uoSmistamento.UoInferiori ?? new UOSmistamento[0]))
            {
                this.ValidateRegistriRuoli(idRegistroDocumentoTrasmesso, uoInferiore);
            }
        }

        private async Task TrasmettiModelloTrasmissione(SmistaDocumentoRequest request)
        {
            if (request.uoAppartenenza.UoSmistaTrasAutomatica != null
                && request.uoAppartenenza.UoSmistaTrasAutomatica.Any())
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

                var trasmissioneAggregate = new Trasmissione(
                    idTenant.ToString(),
                    DateTime.Now,
                    request.documentoTrasmesso.IDDocumento,
                    Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                    new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore()
                    {
                        IdUtente = request.infoUtente.idPeople,
                        IdGruppo = request.infoUtente.idGruppo,
                        IdUtenteDelegato = request.infoUtente.delegato != null ? request.infoUtente.delegato.idPeople : null
                    },
                    !string.IsNullOrWhiteSpace(request.datiTrasmissioneDocumento.NoteGenerali)
                        ? new Core.SeedWork.TextValue(request.datiTrasmissioneDocumento.NoteGenerali)
                        : null);

                foreach (var uoSmistamento in request.uoAppartenenza.UoSmistaTrasAutomatica)
                {

                    bool modelloNoNotify = request.uoAppartenenza.modelloNoNotify;
                    uoSmistamento.modelloNoNotify = modelloNoNotify;

                    foreach (var ruoloDestinatario in uoSmistamento.Ruoli)
                    {
                        //if (modelloNoNotify)
                        //    return true;

                        if (ruoloDestinatario.FlagCompetenza || ruoloDestinatario.FlagConoscenza || !string.IsNullOrWhiteSpace(ruoloDestinatario.ragioneTrasmRapida) || !string.IsNullOrEmpty(request.uoAppartenenza.ragioneTrasmRapida))
                        {
                            // Trasmissione a ruolo

                            var idRagioneTrasmissione = await this._pi3DbContext.RagioneTrasmissioneEntities
                                .AsNoTracking()
                                .Where(r => r.ID_AMM == idTenant && r.VAR_DESC_RAGIONE.ToUpper() == ruoloDestinatario.ragioneTrasmRapida.ToUpper())
                                .Select(r => r.SYSTEM_ID)
                                .FirstAsync();

                            await this.PrepareTrasmissioneRuoloSmistamentoDaModello(ruoloDestinatario, trasmissioneAggregate, idRagioneTrasmissione.ToString());
                        }
                        else
                        {
                            foreach (var utenteDestinatario in ruoloDestinatario.Utenti
                                .Where(u => !string.IsNullOrWhiteSpace(u.ragioneTrasmRapida)))
                            {
                                // Trasmissione ad utente
                                long idRagioneTrasmissione = 0; 
                                if (utenteDestinatario.FlagCompetenza || utenteDestinatario.FlagConoscenza || !string.IsNullOrEmpty(utenteDestinatario.ragioneTrasmRapida)) { 
                                    idRagioneTrasmissione = await this._pi3DbContext.RagioneTrasmissioneEntities
                                    .AsNoTracking()
                                    .Where(r => r.ID_AMM == idTenant && r.VAR_DESC_RAGIONE.ToUpper() == utenteDestinatario.ragioneTrasmRapida.ToUpper())
                                    .Select(r => r.SYSTEM_ID)
                                    .FirstAsync();

                                await this.PrepareTrasmissioneUtenteSmistamento(utenteDestinatario, trasmissioneAggregate, idRagioneTrasmissione.ToString());
                                    }
                            }
                        }
                    }
                }

                await this._trasmissioneRepository.Add(trasmissioneAggregate);

                trasmissioneAggregate.Invia();

                await _trasmissioneRepository.Update(trasmissioneAggregate);

                var docname = await _pi3DbContext.ProfileEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == trasmissioneAggregate.OggettoTrasmesso.Id.AsLong())
                    .Select(p => p.DOCNAME)
                    .FirstAsync();

                foreach (var ts in trasmissioneAggregate.TrasmissioniSingole)
                {
                    var ragione = await _pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking()
                        .Where(r => r.SYSTEM_ID == ts.RagioneTrasmissione.Id.AsLong())
                        .Select(r => r.VAR_DESC_RAGIONE)
                        .FirstAsync();

                    await this._webMethodLoggerService.LogOK("TRASM_DOC_" + ragione.ToUpper().Replace(" ", "_"),
                        trasmissioneAggregate.OggettoTrasmesso.Id,
                        string.Format(Descriptions.LogTrasmessoDocumento, docname),
                        ts.Id,
                        null,
                        request.uoAppartenenza.modelloNoNotify);
                }
            }
        }

        private async Task TrasmettiUOSmistamento(SmistaDocumentoRequest request)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            // Reperimento ragioni trasmissione di default per lo smistamento
            var amministraEntity = await this._pi3DbContext.AmministraEntities
                .AsNoTracking()
                .Where(a => a.SYSTEM_ID == idTenant)
                .Select(a => new
                {
                    a.ID_RAGIONE_COMPETENZA,
                    a.ID_RAGIONE_CONOSCENZA
                })
                .FirstAsync();

            var docname = await _pi3DbContext.ProfileEntities.AsNoTracking()
               .Where(p => p.SYSTEM_ID == request.documentoTrasmesso.IDDocumento.AsLong())
               .Select(p => p.DOCNAME)
               .FirstAsync();
            if (!amministraEntity.ID_RAGIONE_COMPETENZA.HasValue
                || !amministraEntity.ID_RAGIONE_CONOSCENZA.HasValue)
            {
                throw new SmistaDocumentoPi3Exception(new EsitoSmistamentoDocumento()
                {
                    CodiceEsitoSmistamento = 199,
                    DescrizioneEsitoSmistamento = ErrorDescriptions.RagioniTrasmissioneNonImpostate
                });
            }

            foreach (var ruoloDestinatario in request.uoAppartenenza
                    .Ruoli
                    .Select(r => r))
            {     
                var trasmissioneAggregate = new Trasmissione(
                                idTenant.ToString(),
                                DateTime.Now,
                                request.documentoTrasmesso.IDDocumento,
                                Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiOggettiTrasmessiEnum.DocumentoAmministrativo,
                                new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore()
                                {
                                    IdUtente = request.infoUtente.idPeople,
                                    IdGruppo = request.infoUtente.idGruppo,
                                    IdUtenteDelegato = request.infoUtente.delegato != null ? request.infoUtente.delegato.idPeople : null
                                },
                                !string.IsNullOrWhiteSpace(request.datiTrasmissioneDocumento.NoteGenerali)
                                    ? new Core.SeedWork.TextValue(request.datiTrasmissioneDocumento.NoteGenerali)
                                    : null);

                await this.PrepareSingleTrasmissioneUOSmistamento(
                    ruoloDestinatario,
                    trasmissioneAggregate,
                    amministraEntity.ID_RAGIONE_COMPETENZA.ToString(),
                    amministraEntity.ID_RAGIONE_CONOSCENZA.ToString());

                if (trasmissioneAggregate.GetUncommittedChanges().Any(c => c.GetType() == typeof(TrasmissioneSingolaGruppoPreparedEvent) || c.GetType() == typeof(TrasmissioneSingolaUtentePreparedEvent)))
                {

                    await this._trasmissioneRepository.Add(trasmissioneAggregate);

                    trasmissioneAggregate.Invia();

                    await _trasmissioneRepository.Update(trasmissioneAggregate);

                    foreach (var ts in trasmissioneAggregate.TrasmissioniSingole)
                    {
                        var ragione = await _pi3DbContext.RagioneTrasmissioneEntities.AsNoTracking()
                            .Where(r => r.SYSTEM_ID == ts.RagioneTrasmissione.Id.AsLong())
                            .Select(r => r.VAR_DESC_RAGIONE)
                            .FirstAsync();

                        await this._webMethodLoggerService.LogOK("TRASM_DOC_" + ragione.ToUpper().Replace(" ", "_"),
                            trasmissioneAggregate.OggettoTrasmesso.Id,
                            string.Format(Descriptions.LogTrasmessoDocumento, docname),
                            ts.Id);
                    }
                }
            }

            if (request.uoAppartenenza.UoInferiori != null && request.uoAppartenenza.UoInferiori.Length > 0)
            {
                foreach (var uoInferiore in request.uoAppartenenza.UoInferiori)
                {
                    await TrasmettiUOSmistamento(new SmistaDocumentoRequest(request.mittente, 
                        request.infoUtente, 
                        request.documentoTrasmesso,
                        request.datiTrasmissioneDocumento, 
                        uoInferiore, uoInferiore.UoInferiori, 
                        request.httpFullPath));
                }
            }
        }

        private async Task PrepareTrasmissioneRuoloSmistamento(
            RuoloSmistamento ruoloDestinatario,
            Trasmissione trasmissioneAggregate,
            string idRagioneTrasmissione)
        {
            // Trasmissione a ruolo
            var idGruppoDestinatario = await this._pi3DbContext.CorrGlobaliEntities
                            .AsNoTracking()
                            .Where(cg => cg.SYSTEM_ID == ruoloDestinatario.ID.AsLong())
                            .Select(cg => cg.ID_GRUPPO)
                            .FirstAsync();

            trasmissioneAggregate.PrepareTrasmissioneSingolaGruppo(
                new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = idGruppoDestinatario.ToString(),
                    Tipo = (string.IsNullOrEmpty(ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm) || ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm == "S" ?
                            Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum.Uno :
                            Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum.Tutti),
                    IdRagioneTrasmissione = idRagioneTrasmissione,
                    Note = (!string.IsNullOrWhiteSpace(ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) ?
                            new Core.SeedWork.TextValue(ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) : null),
                    DataScadenza = !string.IsNullOrWhiteSpace(ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza) ?
                                ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza.AsDateTime() : null,
                    UtentiNotificati =
                            ruoloDestinatario.Utenti
                                .Where(u => u.FlagCompetenza || u.FlagConoscenza)
                                .Select(u =>
                                    new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                    {
                                        IdUtente = u.ID
                                    })
                                .ToList()
                });
        }

        private async Task PrepareTrasmissioneRuoloSmistamentoDaModello(
           RuoloSmistamento ruoloDestinatario,
           Trasmissione trasmissioneAggregate,
           string idRagioneTrasmissione)
        {
            // Trasmissione a ruolo
            var idGruppoDestinatario = await this._pi3DbContext.CorrGlobaliEntities
                            .AsNoTracking()
                            .Where(cg => cg.SYSTEM_ID == ruoloDestinatario.ID.AsLong())
                            .Select(cg => cg.ID_GRUPPO)
                            .FirstAsync();

            trasmissioneAggregate.PrepareTrasmissioneSingolaGruppo(
                new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiTrasmissioneSingolaGruppo()
                {
                    IdGruppoDestinatario = idGruppoDestinatario.ToString(),
                    Tipo = (string.IsNullOrEmpty(ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm) || ruoloDestinatario.datiAggiuntiviSmistamento.tipoTrasm == "S" ?
                            Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum.Uno :
                            Core.AggregateModels.TrasmissioneAggregate.ValueObjects.TipiTrasmissioneSingolaEnum.Tutti),
                    IdRagioneTrasmissione = idRagioneTrasmissione,
                    Note = (!string.IsNullOrWhiteSpace(ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) ?
                            new Core.SeedWork.TextValue(ruoloDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) : null),
                    DataScadenza = !string.IsNullOrWhiteSpace(ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza) ?
                                ruoloDestinatario.datiAggiuntiviSmistamento.dtaScadenza.AsDateTime() : null,
                    UtentiNotificati =
                            ruoloDestinatario.Utenti
                                //.Where(u => u.FlagCompetenza || u.FlagConoscenza)
                                .Select(u =>
                                    new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                    {
                                        IdUtente = u.ID
                                    })
                                .ToList()
                });
        }

        private async Task PrepareTrasmissioneUtenteSmistamento(
            UtenteSmistamento utenteDestinatario,
            Trasmissione trasmissioneAggregate,
            string idRagioneTrasmissione)
        {
            trasmissioneAggregate.PrepareTrasmissioneSingolaUtente(
                new Core.AggregateModels.TrasmissioneAggregate.ValueObjects.DatiTrasmissioneSingolaUtente()
                {
                    IdUtente = utenteDestinatario.ID,
                    IdRagioneTrasmissione = idRagioneTrasmissione,
                    Note = (!string.IsNullOrWhiteSpace(utenteDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) ?
                            new Core.SeedWork.TextValue(utenteDestinatario.datiAggiuntiviSmistamento.NoteIndividuali) : null),
                    DataScadenza = !string.IsNullOrWhiteSpace(utenteDestinatario.datiAggiuntiviSmistamento.dtaScadenza) ?
                                utenteDestinatario.datiAggiuntiviSmistamento.dtaScadenza.AsDateTime() : null
                });
        }

        private async Task PrepareTrasmissioneUOSmistamento(
            UOSmistamento uoSmistamento,
            Trasmissione trasmissioneAggregate,
            string idRagioneTrasmissioneCompetenza,
            string idRagioneTrasmissioneConoscenza)
        {
            foreach (var ruoloDestinatario in uoSmistamento
                    .Ruoli
                    .Select(r => r))
            {
                if (ruoloDestinatario.FlagCompetenza)
                {
                    await this.PrepareTrasmissioneRuoloSmistamento(ruoloDestinatario, trasmissioneAggregate, idRagioneTrasmissioneCompetenza);
                }
                else if (ruoloDestinatario.FlagConoscenza)
                {
                    await this.PrepareTrasmissioneRuoloSmistamento(ruoloDestinatario, trasmissioneAggregate, idRagioneTrasmissioneConoscenza);
                }
                else
                {
                    foreach (var utenteDestinatario in ruoloDestinatario.Utenti)
                    {
                        if (utenteDestinatario.FlagCompetenza)
                            await this.PrepareTrasmissioneUtenteSmistamento(utenteDestinatario, trasmissioneAggregate, idRagioneTrasmissioneCompetenza);

                        else if (utenteDestinatario.FlagConoscenza)
                            await this.PrepareTrasmissioneUtenteSmistamento(utenteDestinatario, trasmissioneAggregate, idRagioneTrasmissioneConoscenza);
                    }
                }
            }

            foreach (var uoSmistamentoInferiore in (uoSmistamento.UoInferiori ?? new UOSmistamento[0]))
            {
                await this.PrepareTrasmissioneUOSmistamento(
                    uoSmistamentoInferiore,
                    trasmissioneAggregate,
                    idRagioneTrasmissioneCompetenza,
                    idRagioneTrasmissioneConoscenza);
            }
        }

        private async Task PrepareSingleTrasmissioneUOSmistamento(
           RuoloSmistamento ruoloDestinatario,
           Trasmissione trasmissioneAggregate,
           string idRagioneTrasmissioneCompetenza,
           string idRagioneTrasmissioneConoscenza)
        {
            if (ruoloDestinatario.FlagCompetenza)
            {
                await this.PrepareTrasmissioneRuoloSmistamento(ruoloDestinatario, trasmissioneAggregate, idRagioneTrasmissioneCompetenza);
            }
            else if (ruoloDestinatario.FlagConoscenza)
            {
                await this.PrepareTrasmissioneRuoloSmistamento(ruoloDestinatario, trasmissioneAggregate, idRagioneTrasmissioneConoscenza);
            }
            else
            {
                foreach (var utenteDestinatario in ruoloDestinatario.Utenti)
                {
                    if (utenteDestinatario.FlagCompetenza)
                        await this.PrepareTrasmissioneUtenteSmistamento(utenteDestinatario, trasmissioneAggregate, idRagioneTrasmissioneCompetenza);

                    else if (utenteDestinatario.FlagConoscenza)
                        await this.PrepareTrasmissioneUtenteSmistamento(utenteDestinatario, trasmissioneAggregate, idRagioneTrasmissioneConoscenza);
                }
            }
        }

    }

    #endregion
}
