// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getNomeRFRequest = Pi3.App.Legacy.WebApi.Application.Requests.getNomeRF;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getNomeRF
{
    public class GetNomeRFHandler : IRequestHandler<getNomeRFRequest, getNomeRFResult>
    {
        #region Public Members

        public GetNomeRFHandler(ILogger<GetNomeRFHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<getNomeRFResult> Handle(getNomeRFRequest request, CancellationToken cancellationToken)
        {
            string output = null;
            try
            {
                output = _dbContext.CorrGlobaliEntities.Where(x => x.VAR_COD_RUBRICA.ToUpper().Equals(request.codiceRF.ToUpper()))
                    .Select(x => x.VAR_DESC_CORR).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }
            return new getNomeRFResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetNomeRFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
