// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneSaveTrasm
{

    // Richiede libreria MediatR
    public class TrasmissioneSaveTrasmHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneSaveTrasm, TrasmissioneSaveTrasmResult>
    {
        #region Public Members

        public TrasmissioneSaveTrasmHandler(ILogger<TrasmissioneSaveTrasmHandler> logger, IClaimsPrincipalService claimsPrincipalService,
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



        public async Task<TrasmissioneSaveTrasmResult> Handle(Application.Requests.TrasmissioneSaveTrasm request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.Trasmissione trasmissione = request.trasmissione;

            try
            {
                var idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                Trasmissione aggregate = null;

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

                        aggregate.PrepareTrasmissioneSingolaUtente(datiU);

                        #endregion

                    }
                    else
                    {
                        #region prepara trasmissione singola per i non Persona
                        var utentiNotificati =
                            (ts.trasmissioneUtente.Where(s => s.daNotificare)?
                                .OfType<DocsPaVO.trasmissione.TrasmissioneUtente>()
                                .ToArray()
                                .Select(u => new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                                {
                                    IdUtente = u.utente.idPeople,
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

                        aggregate.PrepareTrasmissioneSingolaGruppo(datiG);

                        #endregion
                    }
                }

                if (trasmissione.trasmissioniSingole.Count() > 0)
                {
                    await _trasmissioneRepository.Add(aggregate);
                    trasmissione.systemId = string.IsNullOrWhiteSpace(trasmissione.systemId) ? aggregate.Id : trasmissione.systemId;

                }

                //var x = _trasmissioneRepository.Get(idAmm, aggregate.Id);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                trasmissione = null;
            }



            return new TrasmissioneSaveTrasmResult(trasmissione);




        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneSaveTrasmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;



        #endregion
    }

}
