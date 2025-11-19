// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.DocumentAlreadyTransmitted_Opt
{


    public class DocumentAlreadyTransmittedOptHandler : IRequestHandler<Application.Requests.DocumentAlreadyTransmitted_Opt, DocumentAlreadyTransmitted_OptResult>
    {
        #region Public Members

        public DocumentAlreadyTransmittedOptHandler(ILogger<DocumentAlreadyTransmittedOptHandler> logger, IPi3DbContext dbContext, IMediator mediator, 
            IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<DocumentAlreadyTransmitted_OptResult> Handle(Application.Requests.DocumentAlreadyTransmitted_Opt request, CancellationToken cancellationToken)
        {
            bool result = false;
            long idProfile = long.Parse(request.idDocument);

            if(_dbContext.TrasmissioneEntities.Where(a => a.ID_PROFILE == idProfile && a.DTA_INVIO != null).Select(a => a.SYSTEM_ID).Count() > 0)
            {
                result = true;
            }

            return new DocumentAlreadyTransmitted_OptResult(result);
        }


        #endregion

        #region Private Members
        protected ILogger<DocumentAlreadyTransmittedOptHandler> _logger;
        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IClaimsPrincipalService _claimsPrincipalService;

       

        #endregion
    }

}

