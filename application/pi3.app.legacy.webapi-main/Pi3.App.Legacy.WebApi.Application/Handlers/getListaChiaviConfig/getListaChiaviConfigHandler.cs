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
using getListaChiaviConfigRequest = Pi3.App.Legacy.WebApi.Application.Requests.getListaChiaviConfig;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getListaChiaviConfig
{
    public class getListaChiaviConfigHandler : IRequestHandler<getListaChiaviConfigRequest, getListaChiaviConfigResult>
    {
        #region Public Members

        public getListaChiaviConfigHandler(
            ILogger<getListaChiaviConfigHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<getListaChiaviConfigResult> Handle(getListaChiaviConfigRequest request, CancellationToken cancellationToken)
        {
            var chiaviConfigurazione = await this._pi3DbContext.ChiaviConfigurazioneEntities
                .AsNoTracking()
                .Where(c => c.ID_AMM == request.idAmm.AsLong())
                .OrderBy(c => c.VAR_CODICE)
                .Select(c => new DocsPaVO.amministrazione.ChiaveConfigurazione()
                {
                    IDChiave = c.SYSTEM_ID.ToString(),
                    IDAmministrazione = c.ID_AMM.ToString(),
                    Codice = c.VAR_CODICE,
                    Descrizione = c.VAR_DESCRIZIONE ?? String.Empty,
                    Valore = c.VAR_VALORE,
                    TipoChiave = c.CHA_TIPO_CHIAVE ?? String.Empty,
                    Visibile = c.CHA_VISIBILE,
                    Modificabile = c.CHA_MODIFICABILE,
                    IsGlobale = c.CHA_GLOBALE,
                    IsConservazione = c.CHA_CONSERVAZIONE ?? String.Empty
                })
                .ToArrayAsync();

            return new getListaChiaviConfigResult(chiaviConfigurazione);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getListaChiaviConfigHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }
}