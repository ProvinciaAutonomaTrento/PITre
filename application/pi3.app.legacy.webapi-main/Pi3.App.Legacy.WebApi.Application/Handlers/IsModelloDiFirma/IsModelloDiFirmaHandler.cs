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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.IsModelloDiFirma
{

    // Richiede libreria MediatR
    public class IsModelloDiFirmaHandler : IRequestHandler<Application.Requests.IsModelloDiFirma, IsModelloDiFirmaResult>
    {
        #region Public Members

        public IsModelloDiFirmaHandler(ILogger<IsModelloDiFirmaHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<IsModelloDiFirmaResult> Handle(Application.Requests.IsModelloDiFirma request, CancellationToken cancellationToken)
        {
            bool result = false;
            long idProcesso = Convert.ToInt64(request.idProcesso);

            try
            {
                var entities = _dbContext.SchemaProcessoFirmaEntities
                    .Where(x => x.ID_PROCESSO == idProcesso && x.CHA_MODELLO.Equals("1"))
                    .Select(p => p.ID_PROCESSO)
                    .ToList();
                result = entities.Count() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }


            return new IsModelloDiFirmaResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsModelloDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
