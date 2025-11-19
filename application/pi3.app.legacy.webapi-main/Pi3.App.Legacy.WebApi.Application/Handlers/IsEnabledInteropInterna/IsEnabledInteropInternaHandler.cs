// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsEnabledInteropInternaRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEnabledInteropInterna;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEnabledInteropInterna
{
    public class IsEnabledInteropInternaHandler : IRequestHandler<IsEnabledInteropInternaRequest, IsEnabledInteropInternaResult>
    {
        protected readonly IConfigurationService _configurationService;
        protected readonly ILogger<IsEnabledInteropInternaHandler> _logger;

        public IsEnabledInteropInternaHandler(
            IConfigurationService configurationService,
            ILogger<IsEnabledInteropInternaHandler> logger
            )
        {
            this._logger = logger;
            this._configurationService = configurationService;
        }


        public async Task<IsEnabledInteropInternaResult> Handle(IsEnabledInteropInternaRequest request, CancellationToken cancellationToken)
        {
            //Nel BE OLD di PiTre la chiave INTEROP_INT_NO_MAIL non è presente
            return new IsEnabledInteropInternaResult(false);
        }

    }
}
