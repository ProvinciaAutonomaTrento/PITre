// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetStatoConservazioneFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetStatoConservazioneFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetStatoConservazioneFascicolo
{
    public class GetStatoConservazioneFascicoloHandler : IRequestHandler<GetStatoConservazioneFascicoloRequest, GetStatoConservazioneFascicoloResult>
    {
        protected readonly ILogger<GetStatoConservazioneFascicoloHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        public GetStatoConservazioneFascicoloHandler(
            ILogger<GetStatoConservazioneFascicoloHandler> logger,
            IPi3DbContext dbContext
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<GetStatoConservazioneFascicoloResult> Handle(GetStatoConservazioneFascicoloRequest request , CancellationToken cancellationToken)
        {
            string output = string.Empty;

            try
            {
                var stato = await (from p in this._dbContext.VersamentoFascicoliEntities.AsNoTracking()
                                   where p.ID_PROJECT == request.idProject.AsLong()
                                   select p.CHA_STATO).FirstOrDefaultAsync();
                if(stato != null)
                {
                    output = stato;
                }

                if (string.IsNullOrEmpty(output))
                {
                    output = "N";
                }
            }
            catch ( Exception ex )
            {
                output = string.Empty;
            }

            return new(output);
        }
    }
}
