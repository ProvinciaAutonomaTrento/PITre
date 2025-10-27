// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsDocumentoClassificatoInFolderRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsDocumentoClassificatoInFolder;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsDocumentoClassificatoInFolder
{
    public class IsDocumentoClassificatoInFolderHandler : IRequestHandler<IsDocumentoClassificatoInFolderRequest, IsDocumentoClassificatoInFolderResult>
    {
        #region Public Members

        public IsDocumentoClassificatoInFolderHandler(ILogger<IsDocumentoClassificatoInFolderHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<IsDocumentoClassificatoInFolderResult> Handle(IsDocumentoClassificatoInFolderRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                var idProfileAsLong = request.idProfile.AsLong();
                var idProjectAsLong = request.idFolder.AsLong();

                output = await this._dbContext.ProjectComponentEntities.AsNoTracking().AnyAsync(p => p.PROJECT_ID == idProjectAsLong && p.LINK == idProfileAsLong);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new IsDocumentoClassificatoInFolderResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsDocumentoClassificatoInFolderHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
