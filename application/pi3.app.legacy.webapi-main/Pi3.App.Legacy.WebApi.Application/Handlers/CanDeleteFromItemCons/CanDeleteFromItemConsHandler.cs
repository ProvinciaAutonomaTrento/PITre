// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using CanDeleteFromItemConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.CanDeleteFromItemCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CanDeleteFromItemCons
{
    public class CanDeleteFromItemConsHandler : IRequestHandler<CanDeleteFromItemConsRequest, CanDeleteFromItemConsResult>
    {
        #region Public Members

        public CanDeleteFromItemConsHandler(ILogger<CanDeleteFromItemConsHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<CanDeleteFromItemConsResult> Handle(CanDeleteFromItemConsRequest request, CancellationToken cancellationToken)
        {
            var output = 0;

            try
            {
                var idPeople = request.idPeople.AsLong();
                var idGruppo = request.idGruppo.AsLong();
                var idProfile = request.idProfile.AsLong();

                var idRuoloInUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstOrDefaultAsync();

                var areaConservazioneEntity = await this._dbContext.AreaConservazioneEntities.AsNoTracking()
                    .Join(this._dbContext.ItemConservazioneEntities.AsNoTracking(), area => area.SYSTEM_ID, item => item.ID_CONSERVAZIONE, (area, item) => new { item.ID_PROFILE, area.ID_PEOPLE, area.ID_RUOLO_IN_UO })
                    .AnyAsync(a => a.ID_PEOPLE == idPeople && a.ID_RUOLO_IN_UO == idRuoloInUO && a.ID_PROFILE == idProfile);

                output = areaConservazioneEntity ? 1 : 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = -1;
            }

            return new CanDeleteFromItemConsResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CanDeleteFromItemConsHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
