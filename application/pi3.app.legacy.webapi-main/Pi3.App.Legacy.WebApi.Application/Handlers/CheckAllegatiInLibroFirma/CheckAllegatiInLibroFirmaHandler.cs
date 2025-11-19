// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using CheckAllegatiInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.CheckAllegatiInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckAllegatiInLibroFirma
{
    public class CheckAllegatiInLibroFirmaHandler : IRequestHandler<CheckAllegatiInLibroFirmaRequest, CheckAllegatiInLibroFirmaResult>
    {
        #region Public Members

        public CheckAllegatiInLibroFirmaHandler(ILogger<CheckAllegatiInLibroFirmaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<CheckAllegatiInLibroFirmaResult> Handle(CheckAllegatiInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idDocumentoPrincipale = Convert.ToInt64(request.idDocumentoPrincipale);
                output = await this._dbContext.ProfileEntities.AnyAsync(p => p.ID_DOCUMENTO_PRINCIPALE == idDocumentoPrincipale && p.IN_LIBROFIRMA == "1");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new CheckAllegatiInLibroFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CheckAllegatiInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
