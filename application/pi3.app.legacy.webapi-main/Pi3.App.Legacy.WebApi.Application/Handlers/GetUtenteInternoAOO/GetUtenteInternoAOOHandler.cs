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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetUtenteInternoAOO
{

    // Richiede libreria MediatR
    public class GetUtenteInternoAOOHandler : IRequestHandler<getUtenteInternoAOO, getUtenteInternoAOOResult>
    {
        #region Public Members

        public GetUtenteInternoAOOHandler(ILogger<GetUtenteInternoAOOHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<getUtenteInternoAOOResult> Handle(getUtenteInternoAOO request, CancellationToken cancellationToken)
        {
            long idPeople = long.Parse(request.idPeople);
            long idRegistro = long.Parse(request.systemIdRegistro);

            var j1 = _dbContext.CorrGlobaliEntities.Join(_dbContext.PeopleGroupEntities, c => c.ID_GRUPPO, pg => pg.GROUPS_SYSTEM_ID, (c,pg)  => new { c,pg});
            var j2 = j1.Join(_dbContext.RuoloRegistroEntities, j => j.c.SYSTEM_ID, r => r.ID_RUOLO_IN_UO, (j, r) => new { j.c, j.pg, r });
            return new getUtenteInternoAOOResult(j2.Where(j => j.pg.PEOPLE_SYSTEM_ID == idPeople && j.r.ID_REGISTRO == idRegistro).Select(s => s.c.ID_GRUPPO.ToString()).ToArray());
               
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUtenteInternoAOOHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
