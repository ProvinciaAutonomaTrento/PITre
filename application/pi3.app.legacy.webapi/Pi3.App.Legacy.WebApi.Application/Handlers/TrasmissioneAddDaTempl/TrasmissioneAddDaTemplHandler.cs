// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;
using Trasmissione = DocsPaVO.trasmissione.Trasmissione;
using TrasmissioneSingola = DocsPaVO.trasmissione.TrasmissioneSingola;
using TrasmissioneUtente = DocsPaVO.trasmissione.TrasmissioneUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneAddDaTempl
{

    // Richiede libreria MediatR
    public class TrasmissioneAddDaTemplHandler : IRequestHandler<Application.Requests.TrasmissioneAddDaTempl, TrasmissioneAddDaTemplResult>
    {
        #region Public Members

        public TrasmissioneAddDaTemplHandler(ILogger<TrasmissioneAddDaTemplHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
        }

        public async Task<TrasmissioneAddDaTemplResult> Handle(Application.Requests.TrasmissioneAddDaTempl request, CancellationToken cancellationToken)
        {
            var trasm = _trasmissioneRepository.Get(_claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant), request.template.idTrasmissione).Result;

            List<DocsPaVO.trasmissione.TrasmissioneSingola> tsList = new List<DocsPaVO.trasmissione.TrasmissioneSingola>();
            List<DocsPaVO.trasmissione.TrasmissioneUtente> tuList = new List<DocsPaVO.trasmissione.TrasmissioneUtente>();

            Trasmissione t = new Trasmissione();
            t.systemId = trasm.Id;

            TrasmissioneSingola ts = new TrasmissioneSingola();
            ts.systemId = trasm.TrasmissioniSingole[0].Id;
            tsList.Add(ts);
           
            TrasmissioneUtente tu = new DocsPaVO.trasmissione.TrasmissioneUtente();
            tu.systemId = trasm.TrasmissioniSingole[0].TrasmissioniUtente[0].Id;
            tuList.Add(tu);
            ts.trasmissioneUtente = tuList.ToArray();
            t.trasmissioniSingole = tsList.ToArray();

            return new TrasmissioneAddDaTemplResult(t);


        }

        #endregion

        #region Private Members
      
        protected readonly ILogger<TrasmissioneAddDaTemplHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;

        #endregion
    }

}
