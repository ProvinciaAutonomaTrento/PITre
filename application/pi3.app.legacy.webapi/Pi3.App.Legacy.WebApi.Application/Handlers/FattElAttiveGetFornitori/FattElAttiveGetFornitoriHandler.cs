// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FattElAttiveGetFornitoriRequest = Pi3.App.Legacy.WebApi.Application.Requests.FattElAttiveGetFornitori;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FattElAttiveGetFornitori
{
    public class FattElAttiveGetFornitoriHandler : IRequestHandler<FattElAttiveGetFornitoriRequest, FattElAttiveGetFornitoriResult>
    {
        protected readonly ILogger<FattElAttiveGetFornitoriHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IPi3DbContext _dbContext;
        public FattElAttiveGetFornitoriHandler(
            ILogger<FattElAttiveGetFornitoriHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IPi3DbContext dbContext
            )
        {

            this._logger = logger;
            this._dbContext = dbContext;
            this._claimsPrincipalService = claimsPrincipalService;
        }


        public async Task<FattElAttiveGetFornitoriResult> Handle(FattElAttiveGetFornitoriRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.ExternalServices.FornitoreFattAttiva> output = null;
            try
            {
                var idAmm = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant,true);
                var fornitori = await this._dbContext.FattAttivaCodFornitoreEntities.AsNoTracking().Where(fatt => fatt.ID_AMM == idAmm.AsLong()).ToListAsync();

                if(fornitori != null)
                {
                    output = new();
                    var fornitore = new DocsPaVO.ExternalServices.FornitoreFattAttiva();
                    fornitori.ForEach((r) =>
                    {
                        fornitore = new DocsPaVO.ExternalServices.FornitoreFattAttiva();
                        fornitore.IdAmm = r.ID_AMM.ToString();
                        fornitore.IdRegistro = r.ID_REGISTRO.ToString();
                        fornitore.CodFornitore = r.COD_FORNITORE;
                        fornitore.CodFascicolo = r.COD_FASCICOLO;
                        fornitore.CodAmmIPA = r.CODICE_AMM_IPA;

                        output.Add(fornitore);
                    });
                }
            }
            catch(Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new(output != null ? output.ToArray() : null);
        }
    }
}
