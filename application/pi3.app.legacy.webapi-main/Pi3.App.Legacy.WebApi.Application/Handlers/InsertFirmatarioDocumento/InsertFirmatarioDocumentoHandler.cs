// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsertFirmatarioDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertFirmatarioDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertFirmatarioDocumento
{
    public class InsertFirmatarioDocumentoHandler : IRequestHandler<InsertFirmatarioDocumentoRequest, InsertFirmatarioDocumentoResult>
    {
        #region Public Members

        public InsertFirmatarioDocumentoHandler(ILogger<InsertFirmatarioDocumentoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InsertFirmatarioDocumentoResult> Handle(InsertFirmatarioDocumentoRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var firmatarioEntity = new FirmatarioDocEntity()
                {
                    ID_PROFILE = request.firmatarioDoc.IdProfile.AsLong(),
                    ID_VERSION = request.firmatarioDoc.IdVersion.AsLong(),
                    DESCRIZIONE_FIRMATARIO = request.firmatarioDoc.DescrizioneFirmatario.Replace("\"", ""),
                    DTA_FIRMA = await _dbContext.GetSystemDateTime()
                };

                await _dbContext.FirmatarioDocEntities.AddAsync(firmatarioEntity);

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new InsertFirmatarioDocumentoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertFirmatarioDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}