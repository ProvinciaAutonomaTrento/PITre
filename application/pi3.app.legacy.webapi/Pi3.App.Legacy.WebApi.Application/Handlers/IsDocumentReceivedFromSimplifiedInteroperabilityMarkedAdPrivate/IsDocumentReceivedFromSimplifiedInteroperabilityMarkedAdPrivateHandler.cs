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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate
{
    public class IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateHandler : IRequestHandler<Application.Requests.IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate, IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateResult>
    {
        #region Public Members

        public IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateHandler(ILogger<IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateResult> Handle(Application.Requests.IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivate request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idProfile = Convert.ToInt64(request.documentId);
                output = await this._dbContext.SimpInteropReceivedMessageEntities.AnyAsync(s => s.PROFILEID == idProfile && s.RECEIVEDPRIVATE == 1);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsDocumentReceivedFromSimplifiedInteroperabilityMarkedAdPrivateHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
