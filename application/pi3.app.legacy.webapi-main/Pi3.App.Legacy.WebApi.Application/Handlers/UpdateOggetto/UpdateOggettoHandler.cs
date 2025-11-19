// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.OggettoAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateOggettoRequest = Pi3.App.Legacy.WebApi.Application.Requests.UpdateOggetto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UpdateOggetto
{
    public class UpdateOggettoHandler : IRequestHandler<UpdateOggettoRequest, UpdateOggettoResult>
    {
        #region Public Members

        public UpdateOggettoHandler(ILogger<UpdateOggettoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IOggettoRepository oggettoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._oggettoRepository = oggettoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<UpdateOggettoResult> Handle(UpdateOggettoRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

                var aggregate = await this._oggettoRepository.Get(idTenant, request.oggetto.systemId);

                await this._oggettoRepository.Delete(aggregate);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new UpdateOggettoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UpdateOggettoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IOggettoRepository _oggettoRepository;

        #endregion
    }
}
