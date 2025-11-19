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
using DocumentoExecAddConservazioneAMRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoExecAddConservazioneAM;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoExecAddConservazioneAM
{
    public class DocumentoExecAddConservazioneAMHandler : IRequestHandler<DocumentoExecAddConservazioneAMRequest, DocumentoExecAddConservazioneAMResult>
    {
        #region Public Members

        public DocumentoExecAddConservazioneAMHandler(ILogger<DocumentoExecAddConservazioneAMHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<DocumentoExecAddConservazioneAMResult> Handle(DocumentoExecAddConservazioneAMRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("DocumentoExecAddConservazioneAM");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoExecAddConservazioneAMHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        #endregion
    }
}