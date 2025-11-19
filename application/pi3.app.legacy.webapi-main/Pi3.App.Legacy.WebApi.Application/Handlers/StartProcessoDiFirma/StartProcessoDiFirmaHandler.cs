// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.DiagrammaStato;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.StartProcessoDiFirma;
using AvvioProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AvvioProcessoDiFirma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.StartProcessoDiFirma
{
    public class StartProcessoDiFirmaHandler : IRequestHandler<StartProcessoDiFirmaRequest, StartProcessoDiFirmaResult>
    {
        #region Public Members

        public StartProcessoDiFirmaHandler(ILogger<StartProcessoDiFirmaHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IMediator mediator,
           IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<StartProcessoDiFirmaResult> Handle(StartProcessoDiFirmaRequest request, CancellationToken cancellationToken)
        {
            var output = false;
            var resultAvvioProcesso = ResultProcessoFirma.OK;

            try
            {
                var avvioProcesso = await this._mediator.Send(new AvvioProcessoDiFirmaRequest(request.processoDiFirma, request.file, request.infoUtente, request.modalita, request.note, request.opzioniNotifiche));

                output = avvioProcesso.output;
                resultAvvioProcesso = avvioProcesso.resultAvvioProcesso;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
                resultAvvioProcesso = ResultProcessoFirma.KO;
            }

            return new StartProcessoDiFirmaResult(output, resultAvvioProcesso);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StartProcessoDiFirmaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
