// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AddressbookGetRuoloRespUoFromUo
{

    // Richiede libreria MediatR
    public class AddressbookGetRuoloRespUoFromUoResultHandler : IRequestHandler<Application.Requests.AddressbookGetRuoloRespUoFromUo, AddressbookGetRuoloRespUoFromUoResult>
    {
        #region Public Members

        public AddressbookGetRuoloRespUoFromUoResultHandler(ILogger<AddressbookGetRuoloRespUoFromUoResultHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AddressbookGetRuoloRespUoFromUoResult> Handle(Application.Requests.AddressbookGetRuoloRespUoFromUo request, CancellationToken cancellationToken)
        {
            string result = string.Empty;

            string idUo = request.idCorrGlobaliUo;
            string tipoRuolo = request.tipoRuolo;
            string idCorr = request.idCorr;

            try
            {
                long idUoAsLong = idUo.AsLong();
                bool noRuoloResp = false;
                bool isIdParentNull = false;
                long? idParent = 0;
                long? idUoAppo = 0;
                while (!isIdParentNull && !noRuoloResp)
                {
                    var queryable = _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.ID_UO == idUoAsLong && c.CHA_TIPO_URP == "R" &&
                                    !c.DTA_FINE.HasValue);

                    queryable = tipoRuolo == "R" ? queryable.Where(c => c.CHA_RESPONSABILE == "1") : queryable.Where(c => c.CHA_SEGRETARIO == "1");

                    idUoAppo = await queryable
                        .Select(c => c.SYSTEM_ID)
                        .FirstOrDefaultAsync();

                    if(idUoAppo > 0)
                    {
                        if(idUoAppo != idCorr.AsLong())
                        {
                            noRuoloResp = true;
                            result = idUoAppo.ToString();
                        }
                    }

                    if (!noRuoloResp)
                    {
                        idParent = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == idUoAsLong)
                            .Select(c => c.ID_PARENT)
                            .FirstOrDefaultAsync();

                        if (idParent > 0)
                            idUoAsLong = idParent.Value;
                        else
                            isIdParentNull = true;
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new AddressbookGetRuoloRespUoFromUoResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AddressbookGetRuoloRespUoFromUoResultHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;

        protected readonly IPi3DbContext _dbContext;
        #endregion
    }

}
