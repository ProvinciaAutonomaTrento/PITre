// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.TotalFileSizeDocument;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsElectronicallySignedRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsElectronicallySigned;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsElectronicallySigned
{
    public class IsElectronicallySignedHandler : IRequestHandler<IsElectronicallySignedRequest, IsElectronicallySignedResult>
    {
        #region Public Members

        public IsElectronicallySignedHandler(ILogger<IsElectronicallySignedHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsElectronicallySignedResult> Handle(IsElectronicallySignedRequest request, CancellationToken cancellationToken)
        {
            var output = false;

            try
            {
                var idProfileAsLong = request.docnumber.AsLong();
                var versionIdAsLong = request.versionId.AsLong();

                output = await this._dbContext.FirmaElettronicaEntities.AnyAsync(f => f.ID_DOCUMENTO == idProfileAsLong && f.VERSION_ID == versionIdAsLong);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new IsElectronicallySignedResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsElectronicallySignedHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
