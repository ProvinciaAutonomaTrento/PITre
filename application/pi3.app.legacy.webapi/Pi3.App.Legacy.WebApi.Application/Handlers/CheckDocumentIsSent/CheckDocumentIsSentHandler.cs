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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.CheckDocumentIsSent
{
    public class CheckDocumentIsSentHandler : IRequestHandler<Application.Requests.CheckDocumentIsSent, CheckDocumentIsSentResult>
    {
        public CheckDocumentIsSentHandler(ILogger<CheckDocumentIsSentHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<CheckDocumentIsSentResult> Handle(Application.Requests.CheckDocumentIsSent request, CancellationToken cancellationToken)
        {
            var result = false;
            if (!string.IsNullOrWhiteSpace(request.idDocument))
            {
                try
                {
                    long idOggetto = request.idDocument.AsLong();

                    if (idOggetto > 0)
                    {
                        result = await _dbContext.LogEntities.AnyAsync(p => p.ID_OGGETTO == idOggetto && p.VAR_COD_AZIONE.Equals("DOCUMENTOSPEDISCI"));
                        if (!result)
                            result = await _dbContext.LogStoricoEntities.AnyAsync(e => e.ID_OGGETTO == idOggetto && e.VAR_COD_AZIONE.Equals("DOCUMENTOSPEDISCI"));
                    }   
                }
                catch (InvalidCastException)
                {
                    result = false;
                }
                catch
                {
                    throw;
                }
            }

            return new CheckDocumentIsSentResult(result);
        }

        protected readonly ILogger<CheckDocumentIsSentHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
    }
}
