// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getModelloSystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getModelloSystemId;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getModelloSystemId
{
    public class getModelloSystemIdHandler : IRequestHandler<getModelloSystemIdRequest, getModelloSystemIdResult>
    {
        #region Public Members

        public getModelloSystemIdHandler(ILogger<getModelloSystemIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getModelloSystemIdResult> Handle(getModelloSystemIdRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var modelliTrasmEntity = new ModelloTrasmEntity();
                await this._dbContext.ModelloTrasmEntities.AddAsync(modelliTrasmEntity);

                output = modelliTrasmEntity.SYSTEM_ID.ToString();
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new getModelloSystemIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getModelloSystemIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
