// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getCorrispondentiByCodRFRequest = Pi3.App.Legacy.WebApi.Application.Requests.getCorrispondentiByCodRF;
using AddressbookGetCorrispondenteCompletoBySystemIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.AddressbookGetCorrispondenteCompletoBySystemId;
using DocsPaVO.addressbook;
using Pi3.Core.SeedWork;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getCorrispondentiByCodRF
{
    public class getCorrispondentiByCodRFHandler : IRequestHandler<getCorrispondentiByCodRFRequest, getCorrispondentiByCodRFResult>
    {
        #region Public Members

        public getCorrispondentiByCodRFHandler(ILogger<getCorrispondentiByCodRFHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<getCorrispondentiByCodRFResult> Handle(getCorrispondentiByCodRFRequest request, CancellationToken cancellationToken)
        {
            List<Corrispondente> output = new List<Corrispondente>();

            try
            {
                if (!string.IsNullOrEmpty(request.codiceRF))
                {
                    var ruoli = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Join(this._dbContext.RegistroEntities, c => c.ID_RF, r => r.SYSTEM_ID, (c, r) => new { c, r })
                        .Join(this._dbContext.RuoloRegistroEntities, j => j.r.SYSTEM_ID, l => l.ID_REGISTRO, (j, l) => new { j.c, j.r, l })
                        .Where(j => j.r.VAR_CODICE.ToUpper() == request.codiceRF.ToUpper())
                        .Select(j => new
                        {
                            j.l.ID_RUOLO_IN_UO,
                            j.r.ID_AMM,
                            j.c.SYSTEM_ID,
                            j.c.CHA_TIPO_IE
                        })
                        .ToListAsync();
                    if (ruoli != null)
                    {
                        foreach (var r in ruoli)
                        {
                            var tipoUtente = TipoUtente.GLOBALE;
                            if (r.CHA_TIPO_IE == "I")
                                tipoUtente = TipoUtente.INTERNO;
                            if (r.CHA_TIPO_IE == "E")
                                tipoUtente = TipoUtente.ESTERNO;

                            output.Add((await this._mediator.Send(new AddressbookGetCorrispondenteCompletoBySystemIdRequest(r.ID_RUOLO_IN_UO.ToString(), tipoUtente, null))).output);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new getCorrispondentiByCodRFResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getCorrispondentiByCodRFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }

}
