// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
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
using InsertNotaInElencoRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertNotaInElenco;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertNotaInElenco
{
    public class InsertNotaInElencoHandler : IRequestHandler<InsertNotaInElencoRequest, InsertNotaInElencoResult>
    {
        #region Public Members

        public InsertNotaInElencoHandler(ILogger<InsertNotaInElencoHandler> logger,
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

        public async Task<InsertNotaInElencoResult> Handle(InsertNotaInElencoRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var message = string.Empty;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = new NotaRF(idTenant, DateTime.Now, new TextValue(request.nota.descNota), null, request.nota.idRegRf, request.nota.codRegRf, null);
                await this._repository.Add(aggregate);

                if(!string.IsNullOrEmpty(aggregate.Id))
                    output = true;
            }
            catch(NotaRFAlreadyExistsPi3Exception exPi3)
            {
                this._logger.LogError(exPi3, null, null);
                output = false;
                message = Resources.LogNotaEsistente;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new InsertNotaInElencoResult(output, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertNotaInElencoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly INotaRFRepository _repository;

        #endregion
    }
}
