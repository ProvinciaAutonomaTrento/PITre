// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsEnabledMultiMailRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEnabledMultiMail;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEnabledMultiMail
{

    public class IsEnabledMultiMailHandler : IRequestHandler<IsEnabledMultiMailRequest, IsEnabledMultiMailResult>
    {
        #region Public Members

        public IsEnabledMultiMailHandler(ILogger<IsEnabledMultiMailHandler> logger,
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

        public async Task<IsEnabledMultiMailResult> Handle(IsEnabledMultiMailRequest request, CancellationToken cancellationToken)
        {
            var value = await this._configurationService.GetValue<string>(request.idAmm, "FE_ATTIVA_GESTIONE_MULTIMAIL");
            bool output = value == "1";

            return new IsEnabledMultiMailResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsEnabledMultiMailHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}
