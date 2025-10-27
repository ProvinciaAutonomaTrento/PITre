// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetStatoDocStorico
{
    public class GetStatoDocStoricoHandler : IRequestHandler<getStatoDocStorico, getStatoDocStoricoResult>
    {
        public GetStatoDocStoricoHandler(ILogger<GetStatoDocStoricoHandler> logger, IPi3DbContext dbContext, IMediator mediator, IClaimsPrincipalService claimsPrincipalService)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mediator = mediator;
            this._claimsPrincipalService = claimsPrincipalService;
        }

        public async Task<getStatoDocStoricoResult> Handle(getStatoDocStorico request, CancellationToken cancellationToken)
        {
            string output = null;

            try
            {
                if (request.docNumber != null)
                {
                    var docnumber = request.docNumber.AsLong();
                    output = _dbContext.DiagrammiStoEntities.Where(d => d.DOC_NUMBER == docnumber).Select(d => d.VAR_DESC_NEW_STATO).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }       
            return new getStatoDocStoricoResult(output);
        }

        protected IPi3DbContext _dbContext;
        protected IMediator _mediator;
        protected IClaimsPrincipalService _claimsPrincipalService;
        protected ILogger<GetStatoDocStoricoHandler> _logger;
    }
}
