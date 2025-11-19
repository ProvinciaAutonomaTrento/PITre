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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsDocOrAllInLibroFirma
{
    public class IsDocOrAllInLibroFirmaHandler : IRequestHandler<Application.Requests.IsDocOrAllInLibroFirma, IsDocOrAllInLibroFirmaResult>
    {
        #region Public Members

        public IsDocOrAllInLibroFirmaHandler(ILogger<IsDocOrAllInLibroFirmaHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<IsDocOrAllInLibroFirmaResult> Handle(Application.Requests.IsDocOrAllInLibroFirma request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long docnumber = Convert.ToInt64(request.docNumber);
                output = await this._dbContext.ProfileEntities.AnyAsync(p => (p.DOCNUMBER == docnumber || p.ID_DOCUMENTO_PRINCIPALE == docnumber) && p.IN_LIBROFIRMA == "1");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new IsDocOrAllInLibroFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsDocOrAllInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
