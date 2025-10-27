// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getPrimaIstanzaAreaConsRequest = Pi3.App.Legacy.WebApi.Application.Requests.getPrimaIstanzaAreaCons;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getPrimaIstanzaAreaCons
{
    public class getPrimaIstanzaAreaConsHandler : IRequestHandler<getPrimaIstanzaAreaConsRequest, getPrimaIstanzaAreaConsResult>
    {
        protected readonly ILogger<getPrimaIstanzaAreaConsHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public getPrimaIstanzaAreaConsHandler(
            ILogger<getPrimaIstanzaAreaConsHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }


        public async Task<getPrimaIstanzaAreaConsResult> Handle(getPrimaIstanzaAreaConsRequest request, CancellationToken cancellationToken)
        {
            string output = string.Empty;
            try
            {
                var subQ = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == request.idGruppo.AsLong()).Select(c => c.SYSTEM_ID).ToListAsync();


                var res = await (from a in this._dbContext.AreaConservazioneEntities.AsNoTracking()
                                 where a.ID_PEOPLE == request.idPeople.AsLong() && 
                                 a.ID_RUOLO_IN_UO != null && 
                                 subQ.Contains((long)a.ID_RUOLO_IN_UO) &&
                                 a.CHA_STATO != null && a.CHA_STATO.Equals("N")
                                 select a.SYSTEM_ID).ToListAsync();

                if(res != null && res.Count > 0)
                {
                    output = res[0].ToString();
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new(output);
        }
    }
}
