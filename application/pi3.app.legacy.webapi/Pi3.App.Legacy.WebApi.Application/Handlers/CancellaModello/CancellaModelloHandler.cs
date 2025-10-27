// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CancellaModelloRequest = Pi3.App.Legacy.WebApi.Application.Requests.CancellaModello;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CancellaModello
{
    public class CancellaModelloHandler : IRequestHandler<CancellaModelloRequest, CancellaModelloResult>
    {
        #region Public Members

        public CancellaModelloHandler(ILogger<CancellaModelloHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IModelloTrasmissioneRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
        }

        public async Task<CancellaModelloResult> Handle(CancellaModelloRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var aggregate = await this._repository.Get(idTenant, request.idModello);
            if (aggregate == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(request.idModello);

            await this._repository.Delete(aggregate);

            return new CancellaModelloResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CancellaModelloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IModelloTrasmissioneRepository _repository;
        #endregion
    }
}
