// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DelegaAggregate;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaVerificaUnicaAssegnataRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaVerificaUnicaAssegnata;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaVerificaUnicaAssegnata
{
    public class DelegaVerificaUnicaAssegnataHandler : IRequestHandler<DelegaVerificaUnicaAssegnataRequest, DelegaVerificaUnicaAssegnataResult>
    {
        #region Public Members

        public DelegaVerificaUnicaAssegnataHandler(ILogger<DelegaVerificaUnicaAssegnataHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DelegaVerificaUnicaAssegnataResult> Handle(DelegaVerificaUnicaAssegnataRequest request, CancellationToken cancellationToken)
        {
            bool output = true;

            try
            {
                var idUtenteDelegante = request.delega.id_utente_delegante.AsLong();
                var idCorrGlobaliDelegante = request.delega.id_ruolo_delegante.AsLong();

                var queryable = this._dbContext.DelegheEntities.AsNoTracking()
                    .Where(d => d.ID_PEOPLE_DELEGANTE == idUtenteDelegante && (d.DATA_SCADENZA == null || d.DATA_SCADENZA >= DateTime.Now));

                if(!string.IsNullOrEmpty(request.delega.id_delega))
                {
                    var idDelega = request.delega.id_delega.AsLong();
                    queryable = queryable.Where(d => d.SYSTEM_ID != idDelega);
                }

                var delegheEntity = await queryable.ToListAsync();

                foreach (var d in delegheEntity)
                {
                    var idGruppoDelegante = d.ID_RUOLO_DELEGANTE;
                    if (d.ID_RUOLO_DELEGANTE != 0)
                    {
                        var idGruppoEntity = await this._dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == d.ID_RUOLO_DELEGANTE).Select(c => new { c.ID_GRUPPO }).FirstAsync();
                        idGruppoDelegante = idGruppoEntity.ID_GRUPPO;
                    }

                    if (idCorrGlobaliDelegante == 0 || d.ID_RUOLO_DELEGANTE == 0 || idCorrGlobaliDelegante == d.ID_RUOLO_DELEGANTE)
                    {
                        if (request.delega.dataDecorrenza.AsDateTime() >= d.DATA_DECORRENZA && (d.DATA_SCADENZA == null || request.delega.dataDecorrenza.AsDateTime() <= d.DATA_SCADENZA))
                            output = false;

                        if (request.delega.dataDecorrenza.AsDateTime() <= d.DATA_DECORRENZA && (string.IsNullOrEmpty(request.delega.dataScadenza) || request.delega.dataScadenza.AsDateTime() >= d.DATA_DECORRENZA))
                            output = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                output = false;
            }

            return new DelegaVerificaUnicaAssegnataResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaVerificaUnicaAssegnataHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
