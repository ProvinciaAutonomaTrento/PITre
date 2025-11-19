// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmministrazioneGetAmministrazioniByUser
{

    // Richiede libreria MediatR
    public class AmministrazioneGetAmministrazioniByUserHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.amministrazioneGetAmministrazioniByUser, amministrazioneGetAmministrazioniByUserResult>
    {
        #region Public Members

        public AmministrazioneGetAmministrazioniByUserHandler(ILogger<AmministrazioneGetAmministrazioniByUserHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<amministrazioneGetAmministrazioniByUserResult> Handle(amministrazioneGetAmministrazioniByUser request, CancellationToken cancellationToken)
        {

            string message = string.Empty;

            var amm = _dbContext.PeopleEntities.DefaultIfEmpty().Join(_dbContext.AmministraEntities, p => p.ID_AMM, a => a.SYSTEM_ID, (p, a) => new { p, a })
                .AsNoTracking().Where(w => w.p.USER_ID.ToUpper() == request.userId.ToUpper());

            if (!request.controllo)
            {
                amm = amm.AsNoTracking().Where(w => w.p.DISABLED == "N");
            }

            var amministrazione = await amm.Select(s => new DocsPaVO.utente.Amministrazione()
            {
                systemId = s.a.SYSTEM_ID.ToString(),
                codice = s.a.VAR_CODICE_AMM,
                descrizione = s.a.VAR_DESC_AMM,
                libreria = s.a.VAR_LIBRERIA

            }).ToArrayAsync();

            return new amministrazioneGetAmministrazioniByUserResult(amministrazione, message);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmministrazioneGetAmministrazioniByUserHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
