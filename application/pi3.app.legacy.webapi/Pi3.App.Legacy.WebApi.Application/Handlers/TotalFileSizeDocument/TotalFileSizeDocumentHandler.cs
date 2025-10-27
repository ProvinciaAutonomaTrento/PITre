// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.getCanaleBySystemId;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalFileSizeDocumentRequest = Pi3.App.Legacy.WebApi.Application.Requests.TotalFileSizeDocument;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TotalFileSizeDocument
{
    public class TotalFileSizeDocumentHandler : IRequestHandler<TotalFileSizeDocumentRequest, TotalFileSizeDocumentResult>
    {
        #region Public Members

        public TotalFileSizeDocumentHandler(ILogger<TotalFileSizeDocumentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<TotalFileSizeDocumentResult> Handle(TotalFileSizeDocumentRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idProfileAsLong = request.idDocumento.AsLong();

                var sum = await this._dbContext.ProfileEntities
                    .Join(this._dbContext.ComponentEntities, profile => profile.SYSTEM_ID, components => components.DOCNUMBER, (profile, components) => new { profile, components })
                    .Where(j => j.profile.SYSTEM_ID == idProfileAsLong || j.profile.ID_DOCUMENTO_PRINCIPALE == idProfileAsLong
                    && j.components.VERSION_ID == (this._dbContext.VersionEntities.Where(v => v.DOCNUMBER == j.profile.SYSTEM_ID).Max(j => j.VERSION_ID)))
                    .AsNoTracking()
                    .SumAsync(j => j.components.FILE_SIZE);

                if(sum != null)
                    output = Convert.ToInt32(sum);
            }
            catch (Exception ex)
            {
                output = 0;
                this._logger.LogError(ex, null, null);
            }

            return new TotalFileSizeDocumentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TotalFileSizeDocumentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
