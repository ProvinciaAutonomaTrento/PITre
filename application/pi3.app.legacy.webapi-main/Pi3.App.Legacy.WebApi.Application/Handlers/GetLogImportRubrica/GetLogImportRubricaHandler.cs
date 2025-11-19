// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetLogImportRubrica.Exceptions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetLogImportRubrica
{
    public class GetLogImportRubricaHandler : IRequestHandler<getLogImportRubrica, GetLogImportRubricaResult>
    {
        protected readonly ILogger<GetLogImportRubricaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;
        protected readonly string _logPath;

        public GetLogImportRubricaHandler(ILogger<GetLogImportRubricaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IConfiguration configuration
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this._logPath = configuration.GetSection("LogImportazioniOptions:LogRootPath").Value ?? string.Empty;
        }

        public async Task<GetLogImportRubricaResult> Handle(getLogImportRubrica request, CancellationToken cancellationToken)
        {
            var output = new List<string>();
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);

            try
            {
                var filePath = Path.Combine(this._logPath, $"logImportRubrica_{idPeople}.log");

                output.AddRange(await File.ReadAllLinesAsync(filePath));

                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new GetLogImportRubricaResult(output.ToArray());
        }
    }
}
