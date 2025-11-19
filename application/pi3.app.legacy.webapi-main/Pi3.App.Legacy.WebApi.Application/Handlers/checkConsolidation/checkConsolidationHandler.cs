// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using checkConsolidationRequest = Pi3.App.Legacy.WebApi.Application.Requests.checkConsolidation;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.checkConsolidation
{
    public class checkConsolidationHandler : IRequestHandler<checkConsolidationRequest, checkConsolidationResult>
    {
        #region Public Members

        public checkConsolidationHandler(ILogger<checkConsolidationHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDocumentoAmministrativoRepository repository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._repository = repository;
        }

        public async Task<checkConsolidationResult> Handle(checkConsolidationRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                foreach(string id in request.idList)
                {
                    var idProfile = id.AsLong();
                    var consolidatioState = await this._dbContext.ProfileEntities.AsNoTracking().Where(p => p.SYSTEM_ID == idProfile).Select(p => p.CONSOLIDATION_STATE).FirstOrDefaultAsync();
                    if (consolidatioState != "2")
                    {
                        output = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new checkConsolidationResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<checkConsolidationHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IDocumentoAmministrativoRepository _repository;

        #endregion
    }
}
