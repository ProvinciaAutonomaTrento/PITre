// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.File.FirmaRemota2;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSM_RequestOTPRequest = Pi3.App.Legacy.WebApi.Application.Requests.HSM_RequestOTP;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.HSM_RequestOTP
{

    public class HSM_RequestOTPHandler : IRequestHandler<HSM_RequestOTPRequest, HSM_RequestOTPResult>
    {
        #region Public Members

        public HSM_RequestOTPHandler(ILogger<HSM_RequestOTPHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IFirmaRemota2Service firmaRemotaService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._firmaRemotaService = firmaRemotaService;
        }

        public async Task<HSM_RequestOTPResult> Handle(HSM_RequestOTPRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                var respose = await this._firmaRemotaService.RichiestaOtpREST(new RichiestaOtpRequest()
                {
                    AliasCertificato = request.AliasCertificato,
                    DominioCertificato = request.DominioCertificato
                });
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new HSM_RequestOTPResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<HSM_RequestOTPHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IFirmaRemota2Service _firmaRemotaService;

        #endregion
    }
}
