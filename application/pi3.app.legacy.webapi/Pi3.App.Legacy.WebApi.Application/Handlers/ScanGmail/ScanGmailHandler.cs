// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.InstanceAccess.Metadata;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ScanGmail
{
    public class ScanGmailHandler : IRequestHandler<Requests.ScanGmail, Requests.ScanGmailResult>
    {
        #region Public Members

        public ScanGmailHandler(
            ILogger<ScanGmailHandler> logger,
            IAuthProviderFactoryService authProviderFactoryService,
            IConfiguration configuration)
        {
            this._logger = logger;
            this._authProviderFactoryService = authProviderFactoryService;
            this._configuration = configuration;
        }

        public async virtual Task<ScanGmailResult> Handle(Requests.ScanGmail request, CancellationToken cancellationToken)
        {
            var instance = request.Instance;
            var codiceAmministrazione = request.CodiceAmministrazione;
            var codiceRegistro = request.CodiceRegistro;

            var redirectUrl = this._configuration[$"GoogleOptions:{instance}-{codiceAmministrazione}-{codiceRegistro}:RedirectUrl"];

            if (string.IsNullOrWhiteSpace(redirectUrl))
                throw new ArgumentNullException(nameof(redirectUrl));

            this._logger.LogInformation($"ScanGmail - Index - redirectUrl: {redirectUrl}");

            var authProvider = _authProviderFactoryService.Create(instance, codiceAmministrazione, codiceRegistro);

            var authorizationUrl = authProvider.GetAuthorizationUrl(redirectUrl!);

            this._logger.LogInformation($"ScanGmail - Index - authorizationUrl: {authorizationUrl}");

            return new ScanGmailResult(authorizationUrl);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ScanGmailHandler> _logger;
        protected readonly IAuthProviderFactoryService _authProviderFactoryService;
        protected readonly IConfiguration _configuration;

        #endregion
    }
}
