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
using getStatoConservazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.getStatoConservazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getStatoConservazione
{
    public class getStatoConservazioneHandler : IRequestHandler<getStatoConservazioneRequest, getStatoConservazioneResult>
    {
        #region Public Members

        public getStatoConservazioneHandler(
            ILogger<getStatoConservazioneHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<getStatoConservazioneResult> Handle(getStatoConservazioneRequest request, CancellationToken cancellationToken)
        {
            var stato = await this._pi3DbContext.VersamentoEntities
                .AsNoTracking()
                .Where(v => v.ID_PROFILE == request.idDoc.AsLong())
                .Select(v => v.CHA_STATO)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(stato))
                stato = "N";

            return new getStatoConservazioneResult(stato);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getStatoConservazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected IPi3DbContext _pi3DbContext;

        #endregion
    }
}