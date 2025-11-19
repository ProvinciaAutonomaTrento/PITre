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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentoVersioneConSegnaturaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoVersioneConSegnatura;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoVersioneConSegnatura
{
    public class DocumentoVersioneConSegnaturaHandler : IRequestHandler<DocumentoVersioneConSegnaturaRequest, DocumentoVersioneConSegnaturaResult>
    {
        #region Public Members

        public DocumentoVersioneConSegnaturaHandler(ILogger<DocumentoVersioneConSegnaturaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoVersioneConSegnaturaResult> Handle(DocumentoVersioneConSegnaturaRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var versionIdAsLong = request.versionId.AsLong();

                var versionEntity = await this._dbContext.VersionEntities.FirstOrDefaultAsync(v => v.VERSION_ID == versionIdAsLong);
                if(versionEntity != null)
                {
                    versionEntity.CHA_SEGNATURA = "1";
                    await ((DbContext)_dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new DocumentoVersioneConSegnaturaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoVersioneConSegnaturaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
