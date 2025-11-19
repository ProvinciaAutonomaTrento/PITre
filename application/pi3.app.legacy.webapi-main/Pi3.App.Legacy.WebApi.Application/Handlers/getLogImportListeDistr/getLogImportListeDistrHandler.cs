// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getLogImportListeDistrRequest = Pi3.App.Legacy.WebApi.Application.Requests.getLogImportListeDistr;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getLogImportListeDistr
{
    public class getLogImportListeDistrHandler : IRequestHandler<getLogImportListeDistrRequest, getLogImportListeDistrResult>
    {
        #region Public members
        public getLogImportListeDistrHandler(ILogger<getLogImportListeDistrHandler> logger, IClaimsPrincipalService claimsPrincipalService, IConfiguration configuration)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;

            this._logPath = configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
        }
        public async Task<getLogImportListeDistrResult> Handle(getLogImportListeDistrRequest request, CancellationToken cancellationToken)
        {
            var output = new List<string>();
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            try
            {
                var filePath = Path.Combine(this._logPath, $"logImportListeDistr_{idPeople}.log");

                output.AddRange(await File.ReadAllLinesAsync(filePath));

                File.Delete(filePath);
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new getLogImportListeDistrResult(output.ToArray());

        }
        #endregion

        #region Private members
        protected ILogger<getLogImportListeDistrHandler> _logger;
        protected IClaimsPrincipalService _claimsPrincipalService;

        protected readonly string _logPath;
        #endregion

    }
}
