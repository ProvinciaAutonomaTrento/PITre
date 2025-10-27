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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isUltimaDaAccettareRequest = Pi3.App.Legacy.WebApi.Application.Requests.isUltimaDaAccettare;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isUltimaDaAccettare
{
    public class isUltimaDaAccettareHandler : IRequestHandler<isUltimaDaAccettareRequest, isUltimaDaAccettareResult>
    {
        #region Public Members

        public isUltimaDaAccettareHandler(ILogger<isUltimaDaAccettareHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isUltimaDaAccettareResult> Handle(isUltimaDaAccettareRequest request, CancellationToken cancellationToken)
        {
            bool output = false;

            try
            {
                long idTrasmissione = request.idTrasmissione.AsLong();

                var isValid = await this._dbContext.TrasmissioneEntities
                    .Join(this._dbContext.TrasmSingolaEntities, trasm => trasm.SYSTEM_ID, trasmSingola => trasmSingola.ID_TRASMISSIONE, (trasm, trasmSingola) => new
                    {
                        SYSTEM_ID_TRASM = trasm.SYSTEM_ID,
                        SYSTEM_ID_TRASM_SINGOLA = trasmSingola.SYSTEM_ID,
                        trasmSingola.ID_RAGIONE
                    })
                    .Join(this._dbContext.TrasmUtenteEntities, trasm => trasm.SYSTEM_ID_TRASM_SINGOLA, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (trasm, trasmUtente) => new
                    {
                        trasm.SYSTEM_ID_TRASM,
                        trasm.SYSTEM_ID_TRASM_SINGOLA,
                        trasm.ID_RAGIONE,
                        trasmUtente.CHA_ACCETTATA,
                        trasmUtente.CHA_VALIDA
                    })
                    .Join(this._dbContext.RagioneTrasmissioneEntities, trasm => trasm.ID_RAGIONE, ragione => ragione.SYSTEM_ID, (trasm, ragione) => new
                    {
                        trasm.SYSTEM_ID_TRASM,
                        trasm.SYSTEM_ID_TRASM_SINGOLA,
                        trasm.ID_RAGIONE,
                        trasm.CHA_ACCETTATA,
                        trasm.CHA_VALIDA,
                        ragione.CHA_TIPO_RAGIONE
                    })
                    .AnyAsync(t => t.SYSTEM_ID_TRASM == idTrasmissione && t.CHA_TIPO_RAGIONE == "W" && t.CHA_ACCETTATA == "0" && t.CHA_VALIDA == "1");

                var isRejected = await this._dbContext.TrasmissioneEntities
                    .Join(this._dbContext.TrasmSingolaEntities, trasm => trasm.SYSTEM_ID, trasmSingola => trasmSingola.ID_TRASMISSIONE, (trasm, trasmSingola) => new
                    {
                        SYSTEM_ID_TRASM = trasm.SYSTEM_ID,
                        SYSTEM_ID_TRASM_SINGOLA = trasmSingola.SYSTEM_ID,
                        trasmSingola.ID_RAGIONE
                    })
                    .Join(this._dbContext.TrasmUtenteEntities, trasm => trasm.SYSTEM_ID_TRASM_SINGOLA, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (trasm, trasmUtente) => new
                    {
                        trasm.SYSTEM_ID_TRASM,
                        trasm.SYSTEM_ID_TRASM_SINGOLA,
                        trasm.ID_RAGIONE,
                        trasmUtente.CHA_RIFIUTATA
                    })
                    .Join(this._dbContext.RagioneTrasmissioneEntities, trasm => trasm.ID_RAGIONE, ragione => ragione.SYSTEM_ID, (trasm, ragione) => new
                    {
                        trasm.SYSTEM_ID_TRASM,
                        trasm.SYSTEM_ID_TRASM_SINGOLA,
                        trasm.ID_RAGIONE,
                        trasm.CHA_RIFIUTATA,
                        ragione.CHA_TIPO_RAGIONE
                    })
                    .AnyAsync(t => t.SYSTEM_ID_TRASM == idTrasmissione && t.CHA_TIPO_RAGIONE == "W" && t.CHA_RIFIUTATA == "1");

                output = !(isValid || isRejected);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new isUltimaDaAccettareResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isUltimaDaAccettareHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
