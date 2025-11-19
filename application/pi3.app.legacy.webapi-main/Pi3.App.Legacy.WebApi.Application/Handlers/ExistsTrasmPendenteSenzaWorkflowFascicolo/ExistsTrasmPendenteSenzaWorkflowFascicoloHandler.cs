// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExistsTrasmPendenteSenzaWorkflowFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.ExistsTrasmPendenteSenzaWorkflowFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ExistsTrasmPendenteSenzaWorkflowFascicolo
{
    public class ExistsTrasmPendenteSenzaWorkflowFascicoloHandler : IRequestHandler<ExistsTrasmPendenteSenzaWorkflowFascicoloRequest, ExistsTrasmPendenteSenzaWorkflowFascicoloResult>
    {
        #region Public Members

        public ExistsTrasmPendenteSenzaWorkflowFascicoloHandler(ILogger<ExistsTrasmPendenteSenzaWorkflowFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<ExistsTrasmPendenteSenzaWorkflowFascicoloResult> Handle(ExistsTrasmPendenteSenzaWorkflowFascicoloRequest request, CancellationToken cancellationToken)
        {

            bool output = false;

            try
            {
                var idCorrGlobaliAsLong = request.idRuoloInUO.AsLong();
                var idProjectAsLong = request.idProject.AsLong();
                var idPeopleAsLong = request.idPeople.AsLong();

                output = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(), t => t.SYSTEM_ID, s => s.ID_TRASMISSIONE, (t, s) => new { t, s })
                    .Join(this._dbContext.TrasmUtenteEntities.AsNoTracking(), j => j.s.SYSTEM_ID, u => u.ID_TRASM_SINGOLA, (j, u) => new { j.t, j.s, u })
                    .Join(this._dbContext.RagioneTrasmissioneEntities.AsNoTracking(), j => j.s.ID_RAGIONE, r => r.SYSTEM_ID, (j, r) => new { j.t, j.s, j.u, r })
                    .AnyAsync(j => j.t.ID_PROJECT == idProjectAsLong && j.t.DTA_INVIO != null && j.t.CHA_TIPO_OGGETTO == "F"
                        && j.s.ID_CORR_GLOBALE == idCorrGlobaliAsLong && j.r.CHA_TIPO_RAGIONE != "W"
                        && j.u.ID_PEOPLE == idPeopleAsLong && j.u.CHA_IN_TODOLIST == "1" && j.u.CHA_VALIDA == "1");
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new ExistsTrasmPendenteSenzaWorkflowFascicoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ExistsTrasmPendenteSenzaWorkflowFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
