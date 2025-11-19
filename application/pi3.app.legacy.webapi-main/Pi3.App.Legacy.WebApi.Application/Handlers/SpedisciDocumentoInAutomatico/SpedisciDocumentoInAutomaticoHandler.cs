// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Spedizione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpedisciDocumentoInAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.SpedisciDocumentoInAutomatico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SpedisciDocumentoInAutomatico
{
    public class SpedisciDocumentoInAutomaticoHandler : IRequestHandler<SpedisciDocumentoInAutomaticoRequest, SpedisciDocumentoInAutomaticoResult>
    {
        #region Public Members

        public SpedisciDocumentoInAutomaticoHandler(ILogger<SpedisciDocumentoInAutomaticoHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<SpedisciDocumentoInAutomaticoResult> Handle(SpedisciDocumentoInAutomaticoRequest request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var amministraEntity = await _dbContext.AmministraEntities
                .Where(a => a.SYSTEM_ID == idTenant)
                .Select(a => new
                {
                    a.SPEDIZIONE_AUTO_DOC,
                    a.TRASMISSIONE_AUTO_DOC
                })
                .FirstAsync();

            var infoSpedizione = (await _mediator.Send(new Requests.GetSpedizioneDocumento(request.infoUtente, request.documento))).output;

            foreach(DestinatarioEsterno dest in infoSpedizione.DestinatariEsterni)
                dest.IncludiInSpedizione = dest.Interoperante && amministraEntity.SPEDIZIONE_AUTO_DOC == "1";

            foreach (Destinatario dest in infoSpedizione.DestinatariInterni)
                dest.IncludiInSpedizione = amministraEntity.TRASMISSIONE_AUTO_DOC == "1";

            var spedisciDocumentoResult = await _mediator.Send(new Requests.SpedisciDocumento(request.infoUtente, request.documento, infoSpedizione));

            return new SpedisciDocumentoInAutomaticoResult(spedisciDocumentoResult.output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SpedisciDocumentoInAutomaticoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}