// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioniDeleteTrasmissione
{

    // Richiede libreria MediatR
    public class TrasmissioniDeleteTrasmissioneHandler : IRequestHandler<Application.Requests.TrasmissioniDeleteTrasmissione, TrasmissioniDeleteTrasmissioneResult>
    {
        #region Public Members

        public TrasmissioniDeleteTrasmissioneHandler(ILogger<TrasmissioniDeleteTrasmissioneHandler> logger, IClaimsPrincipalService claimsPrincipalService,
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

        public async Task<TrasmissioniDeleteTrasmissioneResult> Handle(Application.Requests.TrasmissioniDeleteTrasmissione request, CancellationToken cancellationToken)
        {
            bool result = false;
            var idAmm = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
            var trasmissione = request.objTrasm;

            if (!string.IsNullOrWhiteSpace(trasmissione.systemId))
            {
                if (await _trasmissioneRepository.Exists(idAmm, trasmissione.systemId))
                {
                    var aggregate = await _trasmissioneRepository.Get(idAmm, trasmissione.systemId);

                    await _trasmissioneRepository.Delete(aggregate);

                }
            }

            return new TrasmissioniDeleteTrasmissioneResult(result);
        }


        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioniDeleteTrasmissioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }

}
