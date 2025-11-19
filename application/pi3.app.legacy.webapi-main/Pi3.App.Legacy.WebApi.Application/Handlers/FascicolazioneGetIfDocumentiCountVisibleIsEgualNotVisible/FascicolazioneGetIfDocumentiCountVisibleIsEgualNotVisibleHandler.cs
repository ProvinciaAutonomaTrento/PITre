// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible
{

    // Richiede libreria MediatR
    public class FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleHandler : IRequestHandler<Application.Requests.FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible, FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleResult>
    {
        #region Public Members

        public FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleHandler(ILogger<FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;

        }

        public async Task<FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleResult> Handle(Application.Requests.FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisible request, CancellationToken cancellationToken)
        {
            bool result = false;

            long?[] secCheck = { _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser),
                                 _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup)};

            var q1 = _dbContext.ProfileEntities.Join(_dbContext.ProjectComponentEntities, p => p.SYSTEM_ID, pe => pe.LINK, (p, pe) => new { p, pe }).
                Join(_dbContext.DocumentTypesEntities, j => j.p.DOCUMENTTYPE, d => d.SYSTEM_ID, (j, d) => new { j.p, j.pe, d }).
                Where(w => w.pe.PROJECT_ID == request.folder.systemID.AsLong() && w.pe.TYPE == "D" && w.p.DTA_ANNULLA == null && (w.p.CHA_IN_CESTINO ?? "0") == "1");

            var visibile = await q1.Join(_dbContext.SecurityEntities, q => q.p.SYSTEM_ID, s => s.THING, (q, s) => new { q.d, q.p, q.pe, s })
                .Where(a => secCheck.Contains(a.s.PERSONORGROUP) && a.s.ACCESSRIGHTS > 0).Select(s=>s.p.SYSTEM_ID).ToListAsync();


            var notvisible = await _dbContext.ProfileEntities.Join(_dbContext.ProjectComponentEntities, p => p.SYSTEM_ID, pe => pe.LINK, (p, pe) => new { p, pe }).
                Join(_dbContext.DocumentTypesEntities, j => j.p.DOCUMENTTYPE, d => d.SYSTEM_ID, (j, d) => new { j.p, j.pe, d }).
                Where(w => w.pe.PROJECT_ID == request.folder.systemID.AsLong() && w.pe.TYPE == "D" && w.p.DTA_ANNULLA == null && (w.p.CHA_IN_CESTINO ?? "0") == "1")
                .Select(s => s.p.SYSTEM_ID).ToListAsync();

            if(visibile.Count == notvisible.Count)
            {
                result = true;
            }

            return new FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleResult(result);


        }



        #endregion

        #region Private Members

        protected readonly ILogger<FascicolazioneGetIfDocumentiCountVisibleIsEgualNotVisibleHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;


        #endregion
    }

}
