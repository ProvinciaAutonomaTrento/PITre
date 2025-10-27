// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.AllegatoIsIS;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllegatoIsPECRequest = Pi3.App.Legacy.WebApi.Application.Requests.AllegatoIsPEC;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AllegatoIsPEC
{
    public class AllegatoIsPECHandler : IRequestHandler<AllegatoIsPECRequest, AllegatoIsPECResult>
    {
        #region Public Members

        public AllegatoIsPECHandler(ILogger<AllegatoIsPECHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AllegatoIsPECResult> Handle(AllegatoIsPECRequest request, CancellationToken cancellationToken)
        {
            var output = string.Empty;

            try
            {
                var versionId = request.version_id.AsLong();

                var chaAlegatoEsterno = await this._dbContext.VersionEntities.Where(v => v.VERSION_ID == versionId).Select(c => c.CHA_ALLEGATI_ESTERNO).FirstOrDefaultAsync();
                output = chaAlegatoEsterno == "P" ? "1" : "0";
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new AllegatoIsPECResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AllegatoIsPECHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
