// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.Mobile;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.RejectElementsSignatureProcess;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterruptionSignatureProcessByProponentRequest = Pi3.App.Legacy.WebApi.Application.Requests.InterruptionSignatureProcessByProponent;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InterruptionSignatureProcessByProponent
{
    public class InterruptionSignatureProcessByProponentHandler : IRequestHandler<InterruptionSignatureProcessByProponentRequest, InterruptionSignatureProcessByProponentResult>
    {
        #region Public Members

        public InterruptionSignatureProcessByProponentHandler(ILogger<InterruptionSignatureProcessByProponentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InterruptionSignatureProcessByProponentResult> Handle(InterruptionSignatureProcessByProponentRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            try
            {
                await this._mediator.Send(new Application.Requests.InterruzioneProcessoFirma(request.istanza.docNumber, request.noteInterruzione, "P", request.infoUtente));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new InterruptionSignatureProcessByProponentResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InterruptionSignatureProcessByProponentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
