// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getDimensioneMassimaIstanze_ByteRequest = Pi3.App.Legacy.WebApi.Application.Requests.getDimensioneMassimaIstanze_Byte;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getDimensioneMassimaIstanze_Byte
{
    public class getDimensioneMassimaIstanze_ByteHandler : IRequestHandler<getDimensioneMassimaIstanze_ByteRequest, getDimensioneMassimaIstanze_ByteResult>
    {
        #region Public Members

        public getDimensioneMassimaIstanze_ByteHandler(ILogger<getDimensioneMassimaIstanze_ByteHandler> logger,
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

        public async Task<getDimensioneMassimaIstanze_ByteResult> Handle(getDimensioneMassimaIstanze_ByteRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var value = await this._configurationService.GetValue<string>(idTenant, "BE_CONSERVAZIONE_MAX_DIM_ISTANZA");

                if (!string.IsNullOrEmpty(value))
                {
                    string dimMaxIst = value.Split('§')[1];
                    Int32.TryParse(dimMaxIst, out output);
                }
                else
                {
                    output = 650;
                }

                double dimMax_Byte = output * 1024 * 1024;
                output = Convert.ToInt32(dimMax_Byte);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = 0;
            }

            return new getDimensioneMassimaIstanze_ByteResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getDimensioneMassimaIstanze_ByteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
