// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.getNomeLista
{
    public class getNomeListaHandler : IRequestHandler<Application.Requests.getNomeLista, getNomeListaResult>
    {
        #region Public Members

        public getNomeListaHandler(ILogger<getNomeListaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;

            _dbContext = dbContext;
        }

        public async Task<getNomeListaResult> Handle(Application.Requests.getNomeLista request, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            try
            {
                string codiceLista = request.codiceLista.ToUpper().Replace("'", "''");
                long idAmm = Convert.ToInt64(request.idAmm);

                result = _dbContext.CorrGlobaliEntities
                        .Where(x => string.IsNullOrEmpty(request.idAmm) ? x.VAR_COD_RUBRICA.ToUpper().Equals(codiceLista) : x.VAR_COD_RUBRICA.ToUpper().Equals(codiceLista) && x.ID_AMM == idAmm)
                        .Select(x => x.VAR_DESC_CORR)
                        .ToList()
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }


            return new getNomeListaResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getNomeListaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
