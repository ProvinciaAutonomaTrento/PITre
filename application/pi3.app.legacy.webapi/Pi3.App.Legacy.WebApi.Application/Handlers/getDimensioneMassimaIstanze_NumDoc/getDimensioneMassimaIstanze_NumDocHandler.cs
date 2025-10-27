// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getDimensioneMassimaIstanze_NumDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDimensioneMassimaIstanze_NumDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDimensioneMassimaIstanze_NumDoc
{
        public class getDimensioneMassimaIstanze_NumDocHandler : IRequestHandler<getDimensioneMassimaIstanze_NumDocRequest, getDimensioneMassimaIstanze_NumDocResult>
    {
        #region Public Members

        public getDimensioneMassimaIstanze_NumDocHandler(ILogger<getDimensioneMassimaIstanze_NumDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;

        }


        public async Task<getDimensioneMassimaIstanze_NumDocResult> Handle(getDimensioneMassimaIstanze_NumDocRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var value = await this._configurationService.GetValue<string>(idTenant, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");

                if (!string.IsNullOrEmpty(value))
                {
                    string dimMaxIst = value.Split('§')[0];
                    Int32.TryParse(dimMaxIst, out output);
                }
                else
                {
                    output = 250;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = 0;
            }

            return new getDimensioneMassimaIstanze_NumDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDimensioneMassimaIstanze_NumDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
