// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Settings;

namespace Pi3.App.Legacy.Admin.WebApi
{
    public class ReadHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ReadHeaderMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //if (context.Request.Headers.TryGetValue("x-connectionString", out var header))
            //{
            //    HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString(header);
            //}

            ////HeaderValue.Instance.ConnectionString.Value = "User id=pat_prod; Password=patprod15072013; Data Source=exa-x1t01-scan.intra.infotn.it:1521/X1T01SRV1;";
            //HeaderValue.Instance.ConnectionString.Value = _configuration.GetConnectionString("PAT_INSTANCE");
            await _next(context);
        }
    }
}
