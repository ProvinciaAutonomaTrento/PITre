// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetConvertiblePdfFileTypes
{

    // Richiede libreria MediatR
    public class GetConvertiblePdfFileTypesHandler : IRequestHandler<Application.Requests.GetConvertiblePdfFileTypes, GetConvertiblePdfFileTypesResult>
    {
        #region Public Members

        public GetConvertiblePdfFileTypesHandler(ILogger<GetConvertiblePdfFileTypesHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IConfigurationService configurationService,
            IPi3DbContext dbContext
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
        }

        public async Task<GetConvertiblePdfFileTypesResult> Handle(Application.Requests.GetConvertiblePdfFileTypes request, CancellationToken cancellationToken)
        {
            var configValue = await this._configurationService.GetValue<string>("PDF_CONVERTIBLE_FILE_TYPES");
            
            string[] convertibleTypes = null!;

            if (!string.IsNullOrWhiteSpace(configValue))
                convertibleTypes = configValue.Split("|");

            if (convertibleTypes == null)
                convertibleTypes = new string[1] { "*" };

            return new GetConvertiblePdfFileTypesResult(convertibleTypes);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetConvertiblePdfFileTypesHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
