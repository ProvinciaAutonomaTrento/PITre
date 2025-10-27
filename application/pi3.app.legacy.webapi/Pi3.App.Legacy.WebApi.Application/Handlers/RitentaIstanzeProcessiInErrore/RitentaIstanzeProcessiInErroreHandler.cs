// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.LibroFirma;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RitentaIstanzeProcessiInErroreRequest = Pi3.App.Legacy.WebApi.Application.Requests.RitentaIstanzeProcessiInErrore;
using EseguiPassoAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.EseguiPassoAutomatico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.RitentaIstanzeProcessiInErrore
{
    public class RitentaIstanzeProcessiInErroreHandler : IRequestHandler<RitentaIstanzeProcessiInErroreRequest, RitentaIstanzeProcessiInErroreResult>
    {
        #region Public Members

        public RitentaIstanzeProcessiInErroreHandler(ILogger<RitentaIstanzeProcessiInErroreHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<RitentaIstanzeProcessiInErroreResult> Handle(RitentaIstanzeProcessiInErroreRequest request, CancellationToken cancellationToken)
        {
            var output = true;

            try
            {
                if(request.istanzeProcessi != null && request.istanzeProcessi.Any())
                {
                    foreach(var istanza in request.istanzeProcessi.Where(i => i.statoProcesso == TipoStatoProcesso.IN_ERROR))
                    {
                        var idIstanzaProcesso = istanza.idIstanzaProcesso.AsLong();

                        var istanzaProcessoEntity = await this._dbContext.IstanzaProcessoFirmaEntities.FirstAsync(i => i.ID_ISTANZA == idIstanzaProcesso);
                        istanzaProcessoEntity.STATO = TipoStatoProcesso.REPLAY.ToString();

                        await ((DbContext)_dbContext).SaveChangesAsync();

                        await this._mediator.Send(new EseguiPassoAutomaticoRequest(istanza.idIstanzaProcesso));
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = false;
            }

            return new RitentaIstanzeProcessiInErroreResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<RitentaIstanzeProcessiInErroreHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}