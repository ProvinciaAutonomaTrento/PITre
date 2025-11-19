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
using IsDocInLibroFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsDocInLibroFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsDocInLibroFirma
{
    public class IsDocInLibroFirmaHandler : IRequestHandler<IsDocInLibroFirmaRequest, IsDocInLibroFirmaResult>
    {
        #region Public Members

        public IsDocInLibroFirmaHandler(ILogger<IsDocInLibroFirmaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<IsDocInLibroFirmaResult> Handle(IsDocInLibroFirmaRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long docnumber = Convert.ToInt64(request.docNumber);
                output = await _dbContext.ProfileEntities.Where(p => p.DOCNUMBER == docnumber && p.IN_LIBROFIRMA == "1").AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new IsDocInLibroFirmaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsDocInLibroFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
