// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmistamentoExistUOInfRequest = Pi3.App.Legacy.WebApi.Application.Requests.SmistamentoExistUOInf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SmistamentoExistUOInf
{

    public class SmistamentoExistUOInfHandler : IRequestHandler<SmistamentoExistUOInfRequest, SmistamentoExistUOInfResult>
    {
        #region Public Members

        public SmistamentoExistUOInfHandler(ILogger<SmistamentoExistUOInfHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<SmistamentoExistUOInfResult> Handle(SmistamentoExistUOInfRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var idUOAsLong = request.idUO.AsLong();
            var registriAppartenenza = request.mittente.RegistriAppartenenza.Select(i => i.AsLong()).ToArray();

            var count = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Join(
                    this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                    cg => cg.SYSTEM_ID,
                    cgg => cgg.ID_UO,
                    (cg, cgg) => new { cg, cgg }
                )
                .Join(
                    this._dbContext.UoRegEnties.AsNoTracking(),
                    j => j.cg.SYSTEM_ID,
                    uor => uor.ID_UO,
                    (j, uor) => new { j.cg, j.cgg, uor }
                )
                .Where(
                    j =>
                        j.cgg.ID_UO > 0
                        && j.cg.SYSTEM_ID > 0
                        && j.cg.DTA_FINE == null
                        && j.cgg.DTA_FINE == null
                        && j.cgg.CHA_RIFERIMENTO == "1"
                        && j.cg.ID_PARENT == idUOAsLong
                        && registriAppartenenza.Contains((long)j.uor.ID_REGISTRO)
                )
                .Select( j => j.cg.SYSTEM_ID)
                .Distinct()
                .CountAsync();

            output = count > 0;

            return new SmistamentoExistUOInfResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SmistamentoExistUOInfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
