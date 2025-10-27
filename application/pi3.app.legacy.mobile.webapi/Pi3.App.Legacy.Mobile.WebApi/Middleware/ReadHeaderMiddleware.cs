// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Settings;
using Pi3.App.Legacy.Mobile.WebApi.Controllers;

namespace Pi3.App.Legacy.Mobile.WebApi.Middleware
{
    public class ReadHeaderMiddleware
    {
        readonly ILogger<ReadHeaderMiddleware> _logger;

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ReadHeaderMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ReadHeaderMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString("PAT_INSTANCE");
            //HeaderValue.Instance.ConnectionName.Value = "PAT_INSTANCE";

            if (context.Request.Headers.TryGetValue("Instance", out var header))
            {
                _logger.LogInformation("Instance from header: {0}", header);
                HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString(header);
                HeaderValue.Instance.ConnectionName.Value = header;

                if (string.IsNullOrWhiteSpace(HeaderValue.Instance.ConnectionString.Value)) {
                    _logger.LogInformation("Instance per header sbagliato: PAT_INSTANCE");
                    HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString("PAT_INSTANCE");
                    HeaderValue.Instance.ConnectionName.Value = "PAT_INSTANCE";
                }
            }
            else
            {
                _logger.LogInformation("Instance default: PAT_INSTANCE");
                HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString("PAT_INSTANCE");
                HeaderValue.Instance.ConnectionName.Value = "PAT_INSTANCE";
            }

            ////HeaderValue.Instance.ConnectionString.Value = "User id=pat_prod; Password=patprod15072013; Data Source=exa-x1t01-scan.intra.infotn.it:1521/X1T01SRV1;";
            await _next(context);
        }

    }
}
