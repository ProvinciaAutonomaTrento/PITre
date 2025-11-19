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

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.GetTypeSignatureToBeEntered
{

    // Richiede libreria MediatR
    public class GetTypeSignatureToBeEnteredHandler : IRequestHandler<Application.Requests.GetTypeSignatureToBeEntered, GetTypeSignatureToBeEnteredResult>
    {
        #region Public Members

        public GetTypeSignatureToBeEnteredHandler(ILogger<GetTypeSignatureToBeEnteredHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

        }

        public async Task<GetTypeSignatureToBeEnteredResult> Handle(Application.Requests.GetTypeSignatureToBeEntered request, CancellationToken cancellationToken)
        {
            long docnumber = long.Parse(request.fileReq.docNumber);

            var j = _dbContext.IstanzaProcessoFirmaEntities.Join(_dbContext.IstanzaPassoFirmaEntities, a => a.ID_ISTANZA, b => b.ID_ISTANZA_PROCESSO, (a, b) => new { a, b });
            string? tipoFirma = j.Where(w => w.a.ID_DOCUMENTO == docnumber && w.a.STATO == "IN_EXEC" && w.b.STATO_PASSO == "LOOK").Select(s => s.b.TIPO_FIRMA).FirstOrDefault();

            if(!string.IsNullOrWhiteSpace(tipoFirma))
                return new GetTypeSignatureToBeEnteredResult(tipoFirma);
            else
                return new GetTypeSignatureToBeEnteredResult(null);
            

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTypeSignatureToBeEnteredHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
