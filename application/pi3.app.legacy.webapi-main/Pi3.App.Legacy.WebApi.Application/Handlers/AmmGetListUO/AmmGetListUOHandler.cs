// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
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
using AmmGetListUORequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListUO;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListUO
{
    public class AmmGetListUOHandler : IRequestHandler<AmmGetListUORequest, AmmGetListUOResult>
    {
        #region Public Members

        public AmmGetListUOHandler(ILogger<AmmGetListUOHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<AmmGetListUOResult> Handle(AmmGetListUORequest request, CancellationToken cancellationToken)
        {
            List<OrgUO> output = new List<OrgUO>();

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);

            var UOQueryable = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(c => c.ID_AMM == idTenant && c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && c.DTA_FINE == null && c.CHA_SYSTEM_ROLE != "1");

            if(!string.IsNullOrEmpty(request.idParent))
            {
                var idParent = request.idParent.AsLong();
                UOQueryable = UOQueryable.Where(c => c.ID_PARENT == idParent);
            }

            if (!string.IsNullOrEmpty(request.livello))
            {
                var livello = request.livello.AsLong();
                UOQueryable = UOQueryable.Where(c => c.NUM_LIVELLO == livello);
            }

            var uoEntity = await UOQueryable.OrderBy(c => c.ID_PESO_ORG).ThenBy(c => c.VAR_DESC_CORR)
                .Select(c => new
                {
                    c.SYSTEM_ID,
                    c.VAR_CODICE,
                    c.VAR_COD_RUBRICA,
                    c.VAR_DESC_CORR,
                    c.ID_AMM,
                    c.NUM_LIVELLO,
                    c.VAR_CODICE_AOO,
                    c.ID_PESO_ORG,
                    c.INTEROPREGISTRYID,
                    c.INTEROPRFID
                })
                .ToListAsync();

            foreach (var c in uoEntity)
            {
                output.Add(new OrgUO
                {
                    IDCorrGlobale = c.SYSTEM_ID.ToString(),
                    Codice = c.VAR_CODICE,
                    CodiceRubrica = c.VAR_COD_RUBRICA,
                    Descrizione = c.VAR_DESC_CORR,
                    IDAmministrazione = c.ID_AMM.ToString(),
                    Livello = c.NUM_LIVELLO.ToString(),
                    CodiceRegistroInterop = c.VAR_CODICE_AOO,
                    Ruoli = (await this._dbContext.CorrGlobaliEntities.AsNoTracking().CountAsync(r => r.CHA_TIPO_URP == "R" && r.CHA_TIPO_IE == "I" && r.DTA_FINE == null && r.ID_UO == c.SYSTEM_ID)).ToString(),
                    SottoUo = (await this._dbContext.CorrGlobaliEntities.AsNoTracking().CountAsync(u => u.ID_AMM == idTenant && u.CHA_TIPO_URP == "U" && u.CHA_TIPO_IE == "I" && u.DTA_FINE == null && u.ID_PARENT == c.SYSTEM_ID)).ToString(),
                    IDPeso = c.ID_PESO_ORG.ToString(),
                    IdRegistroInteroperabilitaSemplificata = c.INTEROPREGISTRYID.ToString(),
                    IdRfInteroperabilitaSemplificata = c.INTEROPRFID.ToString()
                });
            }

            return new AmmGetListUOResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListUOHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
