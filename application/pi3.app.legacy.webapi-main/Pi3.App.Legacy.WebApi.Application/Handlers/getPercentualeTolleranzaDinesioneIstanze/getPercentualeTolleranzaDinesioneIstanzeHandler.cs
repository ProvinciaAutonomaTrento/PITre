// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using getPercentualeTolleranzaDinesioneIstanzeRequest = Pi3.App.Legacy.WebApi.Application.Requests.getPercentualeTolleranzaDinesioneIstanze;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getPercentualeTolleranzaDinesioneIstanze
{

    public class getPercentualeTolleranzaDinesioneIstanzeHandler : IRequestHandler<getPercentualeTolleranzaDinesioneIstanzeRequest, getPercentualeTolleranzaDinesioneIstanzeResult>
    {
        #region Public Members

        public getPercentualeTolleranzaDinesioneIstanzeHandler(ILogger<getPercentualeTolleranzaDinesioneIstanzeHandler> logger,
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

        public async Task<getPercentualeTolleranzaDinesioneIstanzeResult> Handle(getPercentualeTolleranzaDinesioneIstanzeRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var value = await this._configurationService.GetValue<string>(idTenant, "BE_CONS_PERC_TOLL_MAX_DIM_IST");

                output = !string.IsNullOrEmpty(value) && Int32.TryParse(value, out output) ? output : 10;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = 0;
            }

            return new getPercentualeTolleranzaDinesioneIstanzeResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getPercentualeTolleranzaDinesioneIstanzeHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
