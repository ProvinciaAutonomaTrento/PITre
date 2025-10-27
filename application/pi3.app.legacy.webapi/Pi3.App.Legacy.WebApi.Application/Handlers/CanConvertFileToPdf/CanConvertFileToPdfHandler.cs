// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CanConvertFileToPdf
{
    // Richiede libreria MediatR
    public class CanConvertFileToPdfHandler : IRequestHandler<Application.Requests.CanConvertFileToPdf, CanConvertFileToPdfResult>
    {
        #region Public Members

        public CanConvertFileToPdfHandler(
            ILogger<CanConvertFileToPdfHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IConfigurationService configurationService,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
        }

        public async Task<CanConvertFileToPdfResult> Handle(Application.Requests.CanConvertFileToPdf request, CancellationToken cancellationToken)
        {
            bool result = false;
            string fileName = request.fileName;

            try
            {
                if (await this._configurationService.GetValue<bool>("PDF_CONVERT_INLINE_ACTIVE"))
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    
                    string fileExt = fileInfo.Extension.ToLower().Replace(".", "");

                    if (fileExt != null && fileExt.Equals("pdf"))
                        return new CanConvertFileToPdfResult(false);

                    var convertibleTypesRequest = await this._mediator.Send(new GetConvertiblePdfFileTypes());
                    
                    string[] convertibleTypes = convertibleTypesRequest.output;
                    
                    result = convertibleTypes.Length > 0 && convertibleTypes.Any(x => x.Equals("*") || x.ToLower().Equals(fileExt));
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, null);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, null);
            }

            return new CanConvertFileToPdfResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CanConvertFileToPdfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
