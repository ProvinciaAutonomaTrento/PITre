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
using GetIdDestinatariTrasmDocInUoRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetIdDestinatariTrasmDocInUo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetIdDestinatariTrasmDocInUo
{
    public class GetIdDestinatariTrasmDocInUoHandler : IRequestHandler<GetIdDestinatariTrasmDocInUoRequest, GetIdDestinatariTrasmDocInUoResult>
    {
        #region Public Members

        public GetIdDestinatariTrasmDocInUoHandler(ILogger<GetIdDestinatariTrasmDocInUoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<GetIdDestinatariTrasmDocInUoResult> Handle(GetIdDestinatariTrasmDocInUoRequest request, CancellationToken cancellationToken)
        {
            var output = new List<string>();
            var idUo = request.idUo.AsLong();
            var docnumber = request.docnumber.AsLong();

            var ruoliEntities = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(g => g.ID_UO == idUo && g.CHA_TIPO_URP == "R" && g.DTA_FINE == null)
                .Select(g => new
                {
                    g.SYSTEM_ID,
                    g.ID_GRUPPO
                })
                .Distinct()
                .ToListAsync();

            foreach (var g in ruoliEntities)
            {
                var trasmSingolaEntities = await this._dbContext.TrasmissioneEntities.AsNoTracking()
                    .Join(this._dbContext.TrasmSingolaEntities.AsNoTracking(),
                        t => t.SYSTEM_ID,
                        s => s.ID_TRASMISSIONE,
                        (t, s) => new { t, s })
                    .Join(this._dbContext.TrasmUtenteEntities.AsNoTracking(),
                        j => j.s.SYSTEM_ID,
                        u => u.ID_TRASM_SINGOLA,
                        (j, u) => new { j.t, j.s, u })
                    .Join(this._dbContext.CorrGlobaliEntities.AsNoTracking(),
                        j => j.s.ID_CORR_GLOBALE,
                        g => g.SYSTEM_ID,
                        (j, g) => new { j.t, j.s, j.u, g })
                    .Where(j => j.t.ID_PROFILE == docnumber
                            && (j.s.ID_CORR_GLOBALE == g.SYSTEM_ID
                                || this._dbContext.PeopleGroupEntities.AsNoTracking()
                                    .Any(pg => pg.GROUPS_SYSTEM_ID == g.ID_GRUPPO
                                            && pg.PEOPLE_SYSTEM_ID == j.g.ID_PEOPLE
                                            && pg.DTA_FINE == null)))
                    .Select(j => new { 
                        j.s.ID_CORR_GLOBALE,
                        j.u.ID_PEOPLE
                    })
                    .ToListAsync();

                if (trasmSingolaEntities != null)
                    trasmSingolaEntities.ForEach(s => { output.Add(s.ID_CORR_GLOBALE.ToString() + "_" + s.ID_PEOPLE.ToString()); });
            }

            return new GetIdDestinatariTrasmDocInUoResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetIdDestinatariTrasmDocInUoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
