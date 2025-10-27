// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
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
using DO_GetCountReportDocClassCompactRequest = Pi3.App.Legacy.WebApi.Application.Requests.DO_GetCountReportDocClassCompact;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DO_GetCountReportDocClassCompact
{
    public class DO_GetCountReportDocClassCompactHandler : IRequestHandler<DO_GetCountReportDocClassCompactRequest, DO_GetCountReportDocClassCompactResult>
    {
        #region Public Members

        public DO_GetCountReportDocClassCompactHandler(ILogger<DO_GetCountReportDocClassCompactHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DO_GetCountReportDocClassCompactResult> Handle(DO_GetCountReportDocClassCompactRequest request, CancellationToken cancellationToken)
        {
            var idAmm = Convert.ToInt64(request.idamm);
            var idRegistro = Convert.ToInt64(request.idReg);
            var anno = Convert.ToInt64(request.anno);

            var prospettiQueryable = this._dbContext.MvProspettiDocClassCompAllEntities.AsNoTracking()
                .Where(m => m.ID_AMM == idAmm && m.ID_REGISTR == idRegistro && m.ID_ANNO == anno);

            if (!string.IsNullOrEmpty(request.sede))
                prospettiQueryable = prospettiQueryable.Where(m => (m.SEDE ?? "").Trim() == request.sede);

            if(request.titolario != 0)
            {
                var titolario = Convert.ToInt64(request.titolario);
                prospettiQueryable = prospettiQueryable.Where(m => m.TITOLARIO == titolario);
            }

            var output = await prospettiQueryable.Select(m => m.TOT_DOC_CLASS).Distinct().SumAsync();

            return new DO_GetCountReportDocClassCompactResult(output == null ? string.Empty : output.ToString());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DO_GetCountReportDocClassCompactHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
