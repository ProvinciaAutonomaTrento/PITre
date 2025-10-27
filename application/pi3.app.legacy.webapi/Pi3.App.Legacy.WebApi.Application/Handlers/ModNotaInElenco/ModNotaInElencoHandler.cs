// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.NotaRFAggregate;
using Pi3.Core.AggregateModels.NotaRFAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaRFAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModNotaInElencoRequest = Pi3.App.Legacy.WebApi.Application.Requests.ModNotaInElenco;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ModNotaInElenco
{
    public class ModNotaInElencoHandler : IRequestHandler<ModNotaInElencoRequest, ModNotaInElencoResult>
    {
        #region Public Members

        public ModNotaInElencoHandler(ILogger<ModNotaInElencoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            INotaRFRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<ModNotaInElencoResult> Handle(ModNotaInElencoRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var message = string.Empty;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = await this._repository.Get(idTenant, request.nota.idNota);
                aggregate.ChangeName(new TextValue(request.nota.descNota));
                aggregate.ChangeRFNota(request.nota.idRegRf, request.nota.codRegRf, null);

                await this._repository.Update(aggregate);
                output = !aggregate.GetUncommittedChanges().Any();

            }
            catch (NotaRFAlreadyExistsPi3Exception exPi3)
            {
                this._logger.LogError(exPi3, null, null);
                output = false;
                message = exPi3.Message;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new ModNotaInElencoResult(output, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ModNotaInElencoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly INotaRFRepository _repository;

        #endregion
    }
}
