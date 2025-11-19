// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isCorrInListaDistrRequest = Pi3.App.Legacy.WebApi.Application.Requests.isCorrInListaDistr;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.isCorrInListaDistr
{

    public class isCorrInListaDistrHandler : IRequestHandler<isCorrInListaDistrRequest, isCorrInListaDistrResult>
    {
        #region Public Members

        public isCorrInListaDistrHandler(ILogger<isCorrInListaDistrHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<isCorrInListaDistrResult> Handle(isCorrInListaDistrRequest request, CancellationToken cancellationToken)
        {
            DataSet output = null;

            try
            {
                var idCorrAsLong = request.idCorr.AsLong();

                var idListeCorr = await this._dbContext.ListeDistrEntities.AsNoTracking()
                    .Where(l => l.ID_DPA_CORR == idCorrAsLong)
                    .Select(l => l.ID_LISTA_DPA_CORR)
                    .ToListAsync(); 

                if(idListeCorr != null && idListeCorr.Any())
                {
                    var corrGlobaliEntities = await (from corr in this._dbContext.CorrGlobaliEntities
                                               join people in this._dbContext.PeopleEntities on corr.ID_PEOPLE_LISTE equals people.SYSTEM_ID into p
                                               from people in p.DefaultIfEmpty()
                                               join groups in this._dbContext.GroupEntities on corr.ID_GRUPPO_LISTE equals groups.SYSTEM_ID into g
                                               from groups in g.DefaultIfEmpty()
                                               where(corr.CHA_TIPO_URP == "L" && idListeCorr.Contains(corr.SYSTEM_ID))
                                               select new
                                               {
                                                   corr.VAR_CODICE,
                                                   corr.VAR_DESC_CORR,
                                                   PROP = people.FULL_NAME,
                                                   RUOLO = groups.GROUP_ID
                                               }
                                               )
                                               .AsNoTracking()
                                               .ToListAsync();

                    output = corrGlobaliEntities.AsDataSet();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new isCorrInListaDistrResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<isCorrInListaDistrHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
