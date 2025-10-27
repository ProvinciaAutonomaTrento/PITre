// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.DeleteEmail
{
    public class DeleteEmailRequestHandler : IRequestHandler<DeleteEmailRequest>
    {
        #region Public Members

        public DeleteEmailRequestHandler(ILogger<DeleteEmailRequestHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;
        }

        public async Task Handle(DeleteEmailRequest request, CancellationToken cancellationToken)
        {
            if (!await this._corrispondenteRepository.Exists(request.Id))
                throw new CorrispondenteNotFoundPi3Exception(request.Id);

            var aggregate = await this._corrispondenteRepository.Get(request.Id);

            aggregate.RemoveEmail(request.Email);

            await this._corrispondenteRepository.Update(aggregate);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DeleteEmailRequestHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        #endregion
    }

}
