// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsEnabledProfilazioneAllegatiRequest = Pi3.App.Legacy.WebApi.Application.Requests.IsEnabledProfilazioneAllegati;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.IsEnabledProfilazioneAllegati
{
    public class IsEnabledProfilazioneAllegatiHandler : IRequestHandler<IsEnabledProfilazioneAllegatiRequest, IsEnabledProfilazioneAllegatiResult>
    {
        #region Public Members

        public IsEnabledProfilazioneAllegatiHandler(ILogger<IsEnabledProfilazioneAllegatiHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<IsEnabledProfilazioneAllegatiResult> Handle(IsEnabledProfilazioneAllegatiRequest request, CancellationToken cancellationToken)
        {
            //La chiave IS_ENABLED_PROFILAZIONE_ALLEGATI è sempre attiva per tutti gli enti del PiTre
            return new IsEnabledProfilazioneAllegatiResult(true);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<IsEnabledProfilazioneAllegatiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}
