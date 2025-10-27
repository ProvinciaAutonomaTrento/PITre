// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsDocumentReceivedWithIS
{
    // Richiede libreria MediatR
    public class IsDocumentReceivedWithISHandler : IRequestHandler<Application.Requests.IsDocumentReceivedWithIS, IsDocumentReceivedWithISResult>
    {
        #region Public Members

        public IsDocumentReceivedWithISHandler(ILogger<IsDocumentReceivedWithISHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
           IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsDocumentReceivedWithISResult> Handle(Application.Requests.IsDocumentReceivedWithIS request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idProfile = Convert.ToInt64(request.documentId);
                output = await this._dbContext.SimpInteropReceivedMessageEntities.Where(x => x.PROFILEID == idProfile).AnyAsync();
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new IsDocumentReceivedWithISResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsDocumentReceivedWithISHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
