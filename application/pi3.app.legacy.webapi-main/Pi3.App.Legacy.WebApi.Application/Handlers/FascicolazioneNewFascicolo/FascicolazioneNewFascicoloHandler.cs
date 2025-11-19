// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FascicolazioneNewFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.FascicolazioneNewFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneNewFascicolo
{
    public class FascicolazioneNewFascicoloHandler : IRequestHandler<FascicolazioneNewFascicoloRequest, FascicolazioneNewFascicoloResult>
    {
        #region Public Members

        public FascicolazioneNewFascicoloHandler(ILogger<FascicolazioneNewFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IAggregazioneDocumentaleRepository repository,
            INotaRepository repositoryNota)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
            this._repositoryNota = repositoryNota;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<FascicolazioneNewFascicoloResult> Handle(FascicolazioneNewFascicoloRequest request, CancellationToken cancellationToken)
        {
            Fascicolo output = request.fascicolo;
            ResultCreazioneFascicolo resultCreazione = ResultCreazioneFascicolo.OK;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                TipologieVisibilitaEnum visibilitaEnum = output.privato == "1" ? TipologieVisibilitaEnum.Privata : TipologieVisibilitaEnum.Gerarchica;
                var idPianoConservazione = output.pianoConservazione != null && !string.IsNullOrEmpty(output.pianoConservazione.SystemId) ? output.pianoConservazione.SystemId : null;
                var nomeSerieDocumentale = output.pianoConservazione != null && !string.IsNullOrEmpty(output.pianoConservazione.TipologiaFascicolo) ? output.pianoConservazione.TipologiaFascicolo : string.Empty;
                TipiAggregazioneEnum tipiAggregazione = !string.IsNullOrEmpty(idPianoConservazione) ? TipiAggregazioneEnum.SerieDocumentale : TipiAggregazioneEnum.Fascicolo;
                TipologieFascicoloEnum? tipologiaFascicolo = tipiAggregazione == TipiAggregazioneEnum.Fascicolo ? TipologieFascicoloEnum.ProcedimentoAmministrativo : null;

                var aggregate = new AggregazioneDocumentale(idTenant, DateTime.Now, new TextValue(output.descrizione), tipiAggregazione, tipologiaFascicolo, visibilitaEnum);
                aggregate.AddClassification(request.classificazione.systemID, new TextValue(request.classificazione.descrizione), idPianoConservazione, new TextValue(nomeSerieDocumentale));

                //Nell import fascicoli questi campi non sono valorizzati, la collocazione fisica non compare
                aggregate.AssignCollocazioneFisica(new CollocazioneFisica()
                {
                    Id = output.idUoLF,
                    DataCollocazione = !string.IsNullOrEmpty(output.dtaLF) ? output.dtaLF.AsDateTime() : null,
                    Descrizione = new TextValue(output.descrizioneUOLF),
                    Cartaceo = output.cartaceo
                });

                if (output.template != null && output.template.SYSTEM_ID != 0)
                {
                    aggregate.AddProfile(output.template.SYSTEM_ID.ToString(), new TextValue(output.template.DESCRIZIONE));

                    foreach (var oggettoCustom in output.template.ELENCO_OGGETTI)
                    {
                        switch (oggettoCustom.TIPO.DESCRIZIONE_TIPO)
                        {
                            case "Contatore":
                            case "ContatoreSottocontatore":
                                aggregate.AddProfileField(
                                output.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ContatoreRepertorioFieldValue(oggettoCustom.ID_AOO_RF, oggettoCustom.CONTATORE_DA_FAR_SCATTARE, oggettoCustom.RESETTA_CONTATORE_INIZIO_ANNO == "SI"));
                                break;
                            case "CasellaDiSelezione":
                                aggregate.AddProfileField(
                                output.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldMultiValue(oggettoCustom.VALORI_SELEZIONATI.Select(s => new TextValue(s)).ToArray()));
                                break;
                            default:
                                aggregate.AddProfileField(
                                output.template.SYSTEM_ID.ToString(),
                                oggettoCustom.SYSTEM_ID.ToString(),
                                new TextValue(oggettoCustom.DESCRIZIONE),
                                oggettoCustom.TIPO.DESCRIZIONE_TIPO,
                                new ElementFieldSingleValue(new TextValue(oggettoCustom.VALORE_DATABASE)));
                                break;
                        }
                    }
                }

                await this._repository.Add(aggregate);
                if(!string.IsNullOrEmpty(aggregate.Id))
                {
                    var projectEntity = await this._dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == aggregate.Id.AsLong())
                        .Select(p =>  new 
                        {
                            p.ID_FASCICOLO, 
                            p.DESCRIPTION 
                        }).FirstAsync();

                    output.systemID = projectEntity.ID_FASCICOLO.ToString();
                    output.codice = projectEntity.DESCRIPTION;
                }
                
                foreach (var note in output.noteFascicolo)
                {
                    TipoAccessoNotaEnum accesso = TipoAccessoNotaEnum.Personale;
                    switch(note.TipoVisibilita)
                    {
                        case DocsPaVO.Note.TipiVisibilitaNotaEnum.Tutti:
                            accesso = TipoAccessoNotaEnum.Pubblica;
                            break;
                        case DocsPaVO.Note.TipiVisibilitaNotaEnum.Ruolo:
                            accesso = TipoAccessoNotaEnum.Ruolo;
                            break;
                        case DocsPaVO.Note.TipiVisibilitaNotaEnum.RF:
                            accesso = TipoAccessoNotaEnum.RF;
                            break;
                    }
                    var aggregateNota = new Nota(idTenant, DateTime.Now, new TextValue(note.Testo), null,
                        new AutoreNota()
                        {
                            IdUtente = note.UtenteCreatore.IdUtente,
                            IdRuolo = note.UtenteCreatore.IdRuolo,
                            IdUtenteDelegato = !string.IsNullOrWhiteSpace(note.IdPeopleDelegato) ? note.IdPeopleDelegato : string.Empty
                        },
                        output.systemID,
                        TipiOggettoEnum.Fascicolo,
                        accesso,
                        note.IdRfAssociato
                        );

                    await this._repositoryNota.Add(aggregateNota);
                    note.Id = aggregateNota.Id;
                }

                await this._webMethodLoggerService.LogOK("FASCICOLAZIONENEWFASCICOLO", output.systemID, string.Format(Resources.LogFascicolazioneNewFascicolo, output.codice));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                resultCreazione = ResultCreazioneFascicolo.GENERIC_ERROR;
                await this._webMethodLoggerService.LogKO("FASCICOLAZIONENEWFASCICOLO", output.systemID, string.Format(Resources.LogFascicolazioneNewFascicolo, output.codice));
                output = null;
            }

            return new FascicolazioneNewFascicoloResult(output, resultCreazione);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneNewFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IAggregazioneDocumentaleRepository _repository;
        protected INotaRepository _repositoryNota;

        #endregion
    }
}
