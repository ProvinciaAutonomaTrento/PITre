// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
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
using isEnableProtocolloTitolarioRequest = Pi3.App.Legacy.WebApi.Application.Requests.isEnableProtocolloTitolario;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isEnableProtocolloTitolario
{
    public class isEnableProtocolloTitolarioHandler : IRequestHandler<isEnableProtocolloTitolarioRequest, isEnableProtocolloTitolarioResult>
    {

        protected readonly ILogger<isEnableProtocolloTitolarioHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        public isEnableProtocolloTitolarioHandler(
            IPi3DbContext dbContext,
            IConfigurationService configurationService,
            ILogger<isEnableProtocolloTitolarioHandler> logger
            )
        {
            this._configurationService = configurationService;
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<isEnableProtocolloTitolarioResult> Handle(isEnableProtocolloTitolarioRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                var enabledContatoreTitTuple = await this._configurationService.TryGetValue<string>("ENABLE_CONTATORE_TIT");
                var enabledProtocolloTitTuple = await this._configurationService.TryGetValue<string>("ENABLE_PROTOCOLLO_TIT");

                if(enabledContatoreTitTuple.Item2 && !string.IsNullOrEmpty(enabledContatoreTitTuple.Item1) && enabledProtocolloTitTuple.Item2 && !string.IsNullOrEmpty(enabledProtocolloTitTuple.Item1))
                {
                    output = enabledProtocolloTitTuple.Item1;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }
            return new(output);
        }
    }
}
