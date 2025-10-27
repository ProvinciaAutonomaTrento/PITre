// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneExecuteAccRifConFascicolazione
{

    // Richiede libreria MediatR
    public class TrasmissioneExecuteAccRifConFascicolazioneHandler : IRequestHandler<Application.Requests.TrasmissioneExecuteAccRifConFascicolazione, TrasmissioneExecuteAccRifConFascicolazioneResult>
    {
        #region Public Members

        public TrasmissioneExecuteAccRifConFascicolazioneHandler(ILogger<TrasmissioneExecuteAccRifConFascicolazioneHandler> logger, IClaimsPrincipalService claimsPrincipalService,
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




        public async Task<TrasmissioneExecuteAccRifConFascicolazioneResult> Handle(Application.Requests.TrasmissioneExecuteAccRifConFascicolazione request, CancellationToken cancellationToken)
        {
            bool result = false;
            var idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var idUserDelegato = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
            var trasmUtente = request.trasmissioneUtente;
            var idGruppo = request.ruolo.idGruppo;

            if (await _trasmissioneRepository.Exists(idAmm, request.idTrasmissione))
            {
                var aggregate =await _trasmissioneRepository.Get(idAmm, request.idTrasmissione);
                if(trasmUtente.tipoRisposta==DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
                {
                    aggregate.Accetta(idGruppo, trasmUtente.utente.idPeople, new Accetta()
                    {
                        Data = DateTime.Now,
                        Note = new TextValue(trasmUtente.noteAccettazione),
                        IdDelegato = string.IsNullOrWhiteSpace(idUserDelegato) ? null : idUserDelegato
                    });

                    await _trasmissioneRepository.Update(aggregate);
                    result = true;
                }
                else
                {
                    aggregate.RifiutaTrasmissioneUtente(trasmUtente.systemId, new Rifiuta()
                    {
                        Data = DateTime.Now,
                        Note = new TextValue(trasmUtente.noteAccettazione),
                        IdDelegato = string.IsNullOrWhiteSpace(idUserDelegato) ? null : idUserDelegato
                    });

                    await _trasmissioneRepository.Update(aggregate);
                    result = true;
                }
            }

            var aggregate2 = await _trasmissioneRepository.Get(idAmm, request.idTrasmissione);

            if (trasmUtente.tipoRisposta == DocsPaVO.trasmissione.TipoRisposta.ACCETTAZIONE)
                await _webMethodLoggerService.LogOK("ACCEPTTRASMFOLDER", aggregate2.OggettoTrasmesso.Id, "Accettazione della trasmissione. Id fascicolo: " + aggregate2.OggettoTrasmesso.Id, null, null);
            else
                await _webMethodLoggerService.LogOK("REJECTTRASMFOLDER", aggregate2.OggettoTrasmesso.Id, "Rifiuto della trasmissione. Id fascicolo: " + aggregate2.OggettoTrasmesso.Id, null, null);

            return new TrasmissioneExecuteAccRifConFascicolazioneResult(result, "");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneExecuteAccRifConFascicolazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }

}
