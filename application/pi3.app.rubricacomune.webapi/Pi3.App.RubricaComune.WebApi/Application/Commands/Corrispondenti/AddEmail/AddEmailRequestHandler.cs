// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.Core.Services.Principal;

namespace Pi3.App.RubricaComune.WebApi.Application.Commands.Corrispondenti.AddEmail
{

    // Richiede libreria MediatR
    public class AddEmailRequestHandler : IRequestHandler<AddEmailRequest>
    {
        #region Public Members

        public AddEmailRequestHandler(ILogger<AddEmailRequestHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, ICorrispondenteRepository corrispondenteRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._corrispondenteRepository = corrispondenteRepository;
        }

        public async Task Handle(AddEmailRequest request, CancellationToken cancellationToken)
        {
            if (!await this._corrispondenteRepository.Exists(request.Id))
                throw new CorrispondenteNotFoundPi3Exception(request.Id);

            var aggregate = await this._corrispondenteRepository.Get(request.Id);

            aggregate.AddEmail(request.DatiEmail.Email, request.DatiEmail.Preferita, request.DatiEmail.Note);

            await this._corrispondenteRepository.Update(aggregate);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddEmailRequestHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ICorrispondenteRepository _corrispondenteRepository;

        #endregion
    }

}
