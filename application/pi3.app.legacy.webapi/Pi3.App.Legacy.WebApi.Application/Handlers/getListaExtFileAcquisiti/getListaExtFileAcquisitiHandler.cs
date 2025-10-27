// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
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
using getListaExtFileAcquisitiRequest = Pi3.App.Legacy.WebApi.Application.Requests.getListaExtFileAcquisiti;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getListaExtFileAcquisiti
{
    public class getListaExtFileAcquisitiHandler : IRequestHandler<getListaExtFileAcquisitiRequest, getListaExtFileAcquisitiResult>
    {
        #region Public Members

        public getListaExtFileAcquisitiHandler(ILogger<getListaExtFileAcquisitiHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getListaExtFileAcquisitiResult> Handle(getListaExtFileAcquisitiRequest request, CancellationToken cancellationToken)
        {
            string[] output = null;

            try
            {
                var idAmm = request.idamm.AsLong();
                var formatiEntity = await this._dbContext.FormatoDocumentoEntities
                    .Where(f => f.ID_AMMINISTRAZIONE == idAmm && !f.FILE_EXTENSION.Contains("."))
                    .OrderBy(f => f.FILE_EXTENSION)
                    .Select(f => f.FILE_EXTENSION.ToUpper())
                    .ToListAsync();

                if(formatiEntity != null && formatiEntity.Count() > 0)
                    output = formatiEntity.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = null;
            }

            return new getListaExtFileAcquisitiResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getListaExtFileAcquisitiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
