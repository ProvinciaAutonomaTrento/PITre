// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.StartProcessoDiFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartProcessoDiFirmaMassiveRequest = Pi3.App.Legacy.WebApi.Application.Requests.StartProcessoDiFirmaMassive;
using AvvioProcessoDiFirmaRequest = Pi3.App.Legacy.WebApi.Application.Requests.AvvioProcessoDiFirma;
using DocsPaVO.documento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.StartProcessoDiFirmaMassive
{

    public class StartProcessoDiFirmaMassiveHandler : IRequestHandler<StartProcessoDiFirmaMassiveRequest, StartProcessoDiFirmaMassiveResult>
    {
        #region Public Members

        public StartProcessoDiFirmaMassiveHandler(ILogger<StartProcessoDiFirmaMassiveHandler> logger,
           IClaimsPrincipalService claimsPrincipalService,
           IMediator mediator,
           IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<StartProcessoDiFirmaMassiveResult> Handle(StartProcessoDiFirmaMassiveRequest request, CancellationToken cancellationToken)
        {
            FirmaResult[] output = null;
            try
            {
                List<FirmaResult> listResult = new List<FirmaResult>();

                foreach (var file in request.fileRequest)
                {
                    var avvioProcesso = await this._mediator.Send(new AvvioProcessoDiFirmaRequest(request.processoDiFirma, file, request.infoUtente, request.modalita, request.note, request.opzioniNotifiche));
                    listResult.Add(new FirmaResult()
                    {
                        fileRequest = file,
                        errore = avvioProcesso.output ? string.Empty : avvioProcesso.resultAvvioProcesso.ToString()
                    });
                }

                output = listResult.ToArray();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new StartProcessoDiFirmaMassiveResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<StartProcessoDiFirmaMassiveHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
