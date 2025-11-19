// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using checkTrasm_UNO_TUTTI_AccettataRifiutataRequest = Pi3.App.Legacy.WebApi.Application.Requests.checkTrasm_UNO_TUTTI_AccettataRifiutata;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.checkTrasm_UNO_TUTTI_AccettataRifiutata
{
    public class checkTrasm_UNO_TUTTI_AccettataRifiutataHandler : IRequestHandler<checkTrasm_UNO_TUTTI_AccettataRifiutataRequest, checkTrasm_UNO_TUTTI_AccettataRifiutataResult>
    {
        #region Public Members

        public checkTrasm_UNO_TUTTI_AccettataRifiutataHandler(ILogger<checkTrasm_UNO_TUTTI_AccettataRifiutataHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<checkTrasm_UNO_TUTTI_AccettataRifiutataResult> Handle(checkTrasm_UNO_TUTTI_AccettataRifiutataRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                if (request.trasmSingola.tipoDest == DocsPaVO.trasmissione.TipoDestinatario.RUOLO)
                {
                    long idTrasmSingola = request.trasmSingola.systemId.AsLong();
                    switch (request.trasmSingola.tipoTrasm)
                    {
                        case "S":
                            output = !(await this._dbContext.TrasmSingolaEntities
                                .Join(this._dbContext.TrasmUtenteEntities, trasmSingola => trasmSingola.SYSTEM_ID, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (trasmSingola, trasmUtente) => new
                                {
                                    SYSTEM_ID_TRASM_SINGOLA = trasmSingola.SYSTEM_ID,
                                    trasmUtente.DTA_ACCETTATA,
                                    trasmUtente.DTA_RIFIUTATA
                                })
                                .AnyAsync(t => t.SYSTEM_ID_TRASM_SINGOLA == idTrasmSingola && (t.DTA_ACCETTATA != null || t.DTA_RIFIUTATA != null)));
                            break;

                        case "T":
                            var countTrasmUtente = await this._dbContext.TrasmSingolaEntities
                                .Join(this._dbContext.TrasmUtenteEntities, trasmSingola => trasmSingola.SYSTEM_ID, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (trasmSingola, trasmUtente) => new
                                {
                                    SYSTEM_ID_TRASM_SINGOLA = trasmSingola.SYSTEM_ID,
                                    trasmUtente.DTA_ACCETTATA,
                                    trasmUtente.DTA_RIFIUTATA
                                })
                                .CountAsync(t => t.SYSTEM_ID_TRASM_SINGOLA == idTrasmSingola);

                            var countDaAccRif = await this._dbContext.TrasmSingolaEntities
                                .Join(this._dbContext.TrasmUtenteEntities, trasmSingola => trasmSingola.SYSTEM_ID, trasmUtente => trasmUtente.ID_TRASM_SINGOLA, (trasmSingola, trasmUtente) => new
                                {
                                    SYSTEM_ID_TRASM_SINGOLA = trasmSingola.SYSTEM_ID,
                                    trasmUtente.DTA_ACCETTATA,
                                    trasmUtente.DTA_RIFIUTATA
                                })
                                .CountAsync(t => t.SYSTEM_ID_TRASM_SINGOLA == idTrasmSingola && ((t.DTA_ACCETTATA == null && t.DTA_RIFIUTATA != null) || (t.DTA_ACCETTATA != null && t.DTA_RIFIUTATA == null)));

                            output = countTrasmUtente > countDaAccRif;

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
            }

            return new checkTrasm_UNO_TUTTI_AccettataRifiutataResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<checkTrasm_UNO_TUTTI_AccettataRifiutataHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
