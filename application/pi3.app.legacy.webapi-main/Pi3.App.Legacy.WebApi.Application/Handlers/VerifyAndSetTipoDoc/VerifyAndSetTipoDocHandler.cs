// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.VerifyAndSetTipoDoc
{

    public class VerifyAndSetTipoDocHandler : IRequestHandler<Application.Requests.VerifyAndSetTipoDoc, VerifyAndSetTipoDocResult>
    {
        #region Public Members

        public VerifyAndSetTipoDocHandler(ILogger<VerifyAndSetTipoDocHandler> logger, IMediator mediator, IClaimsPrincipalService claimsPrincipalService,IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

       
        public async Task<VerifyAndSetTipoDocResult> Handle(Application.Requests.VerifyAndSetTipoDoc request, CancellationToken cancellationToken)
        {
            return new VerifyAndSetTipoDocResult(string.Empty, request.schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<VerifyAndSetTipoDocHandler> _logger;
        protected readonly IMediator _mediator;
        protected readonly IClaimsPrincipalService _claimsPrincipalService; 
        protected readonly IPi3DbContext _dbContext;


        #endregion
    }

}
