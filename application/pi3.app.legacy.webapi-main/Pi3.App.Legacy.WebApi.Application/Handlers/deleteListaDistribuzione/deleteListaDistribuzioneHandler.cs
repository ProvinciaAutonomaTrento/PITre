// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deleteListaDistribuzioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.deleteListaDistribuzione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.deleteListaDistribuzione
{
    public class deleteListaDistribuzioneHandler : IRequestHandler<deleteListaDistribuzioneRequest, deleteListaDistribuzioneResult>
    {
        #region Public Members

        public deleteListaDistribuzioneHandler(ILogger<deleteListaDistribuzioneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IListaDistribuzioneRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._repository = repository;
        }

        public async Task<deleteListaDistribuzioneResult> Handle(deleteListaDistribuzioneRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

            var aggregate = await this._repository.Get(idTenant, request.codiceLista);
            if (aggregate == null)
                throw new ListaDistribuzioneNotFoundPi3Exception(request.codiceLista);

            await this._repository.Delete(aggregate);

            return new deleteListaDistribuzioneResult();
        }

        #endregion

        #region Private Members

        protected readonly ILogger<deleteListaDistribuzioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IListaDistribuzioneRepository _repository;

        #endregion
    }
}
