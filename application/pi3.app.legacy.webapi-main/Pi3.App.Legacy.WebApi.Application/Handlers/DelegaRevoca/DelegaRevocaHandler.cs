// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Deleghe;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.AggregateModels.KeywordAggregate;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaRevocaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaRevoca;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaRevoca
{

    public class DelegaRevocaHandler : IRequestHandler<DelegaRevocaRequest, DelegaRevocaResult>
    {
        #region Public Members

        public DelegaRevocaHandler(ILogger<DelegaRevocaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IDelegaRepository delegaRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._delegaRepository = delegaRepository;
        }

        public async Task<DelegaRevocaResult> Handle(DelegaRevocaRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var msg = string.Empty;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                foreach (InfoDelega delega in request.listaDeleghe)
                {
                    var aggregate = await _delegaRepository.Get(idTenant, delega.id_delega);
                    aggregate.Revoca();

                    await this._delegaRepository.Update(aggregate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DelegaRevocaResult(output, msg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaRevocaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IDelegaRepository _delegaRepository;

        #endregion
    }
}
