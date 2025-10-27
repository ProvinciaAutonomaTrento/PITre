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
using SmistaGetRegistriRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.SmistaGetRegistriRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SmistaGetRegistriRuolo
{
    public class SmistaGetRegistriRuoloHandler : IRequestHandler<SmistaGetRegistriRuoloRequest, SmistaGetRegistriRuoloResult>
    {
        #region Public Members

        public SmistaGetRegistriRuoloHandler(ILogger<SmistaGetRegistriRuoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<SmistaGetRegistriRuoloResult> Handle(SmistaGetRegistriRuoloRequest request, CancellationToken cancellationToken)
        {
            var idRuoloAsLong = request.idRuolo.AsLong();

            var idRegistro = await this._dbContext.RuoloRegistroEntities.AsNoTracking()
                .Where(r => r.ID_RUOLO_IN_UO == idRuoloAsLong && r.DTA_FINE == null)
                .Select(r => r.ID_REGISTRO)
                .ToListAsync();

            string[] output = idRegistro.Select(i => i.ToString()).ToArray();

            return new SmistaGetRegistriRuoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SmistaGetRegistriRuoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
