// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocumentFormat.OpenXml.Bibliography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.HSM_RequestOTP;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Principal;
using Pi3.Core.Services.File.FirmaRemota2;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VerifyCertificateExpiredRequest = Pi3.App.Legacy.WebApi.Application.Requests.VerifyCertificateExpired;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.VerifyCertificateExpired
{
    public class VerifyCertificateExpiredHandler : IRequestHandler<VerifyCertificateExpiredRequest, VerifyCertificateExpiredResult>
    {
        #region Public Members

        public VerifyCertificateExpiredHandler(ILogger<VerifyCertificateExpiredHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IFirmaRemota2Service firmaRemotaService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._firmaRemotaService = firmaRemotaService;
        }

        public async Task<VerifyCertificateExpiredResult> Handle(VerifyCertificateExpiredRequest request, CancellationToken cancellationToken)
        {
            CertificateInfo output = new CertificateInfo();
            output.RevocationStatus = 0; //Sempre valido

            return new VerifyCertificateExpiredResult(output); 
        }

        #endregion

        #region Private Members

        protected readonly ILogger<VerifyCertificateExpiredHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IFirmaRemota2Service _firmaRemotaService;

        #endregion
    }
}
