// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
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
using InsertDescrizioneFascicoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.InsertDescrizioneFascicolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.InsertDescrizioneFascicolo
{
    public class InsertDescrizioneFascicoloHandler : IRequestHandler<InsertDescrizioneFascicoloRequest, InsertDescrizioneFascicoloResult>
    {
        #region Public Members

        public InsertDescrizioneFascicoloHandler(ILogger<InsertDescrizioneFascicoloHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<InsertDescrizioneFascicoloResult> Handle(InsertDescrizioneFascicoloRequest request, CancellationToken cancellationToken)
        {
            var output = true;
            var resultInsertDescrizioniFascicolo = ResultDescrizioniFascicolo.OK;

            try
            {
                var idRegistroAsLong = request.descFasc.IdRegistro.AsLong();
                var idAmmAsLong = request.descFasc.IdAmm.AsLong();
                var systemIdAsLong = request.descFasc.SystemId.AsLong();

                var codice = request.descFasc.Codice;
                var descrizione = request.descFasc.Descrizione;

                var queryable = this._dbContext.DescrizioneFascEntities.AsNoTracking()
                    .Where(d => d.SYSTEM_ID != systemIdAsLong && d.ID_AMM == idAmmAsLong && d.ID_REGISTRO == idRegistroAsLong);

                queryable = !string.IsNullOrEmpty(codice) ? queryable.Where(d => (d.VAR_DESCRIZIONE.ToUpper() == request.descFasc.Descrizione.ToUpper() || d.VAR_CODICE == codice.ToUpper())) 
                    : queryable.Where(d => d.VAR_DESCRIZIONE.ToUpper() == request.descFasc.Descrizione.ToUpper());

                if(await queryable.AnyAsync())
                {
                    resultInsertDescrizioniFascicolo = ResultDescrizioniFascicolo.DESCRIZIONE_PRESENTE;
                    output = false;
                }
                else
                {
                    var descrizioneFascEntity = new DescrizioneFascEntity()
                    {
                        VAR_CODICE = codice,
                        VAR_DESCRIZIONE = descrizione,
                        ID_REGISTRO = idRegistroAsLong,
                        ID_AMM = idAmmAsLong
                    };

                    await this._dbContext.DescrizioneFascEntities.AddAsync(descrizioneFascEntity);
                    await ((DbContext)this._dbContext).SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = false;
                resultInsertDescrizioniFascicolo = ResultDescrizioniFascicolo.KO;
            }

            return new InsertDescrizioneFascicoloResult(output, resultInsertDescrizioniFascicolo);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<InsertDescrizioneFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        #endregion
    }
}
