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
using IsAssociatoRuoloDiagrammaRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsAssociatoRuoloDiagramma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsAssociatoRuoloDiagramma
{
    public class IsAssociatoRuoloDiagrammaHandler : IRequestHandler<IsAssociatoRuoloDiagrammaRequest, IsAssociatoRuoloDiagrammaResult>
    {
        #region Public Members

        public IsAssociatoRuoloDiagrammaHandler(ILogger<IsAssociatoRuoloDiagrammaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
        }

        public async Task<IsAssociatoRuoloDiagrammaResult> Handle(IsAssociatoRuoloDiagrammaRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idDiagramma = Convert.ToInt64(request.idDiagramma);
                long idRuolo = Convert.ToInt64(request.idRuolo);

                output = await _dbContext.AssRuoloStatiDiagrammaEntities.Where(a => a.ID_DIAGRAMMA == idDiagramma && a.ID_GRUPPO == idRuolo && a.ID_STATO == 0).AnyAsync() == false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new IsAssociatoRuoloDiagrammaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsAssociatoRuoloDiagrammaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
