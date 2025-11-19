// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.fascicolazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AggiornaDescrizioneFascicolo
{

    // Richiede libreria MediatR
    public class AggiornaDescrizioneFascicoloHandler : IRequestHandler<Application.Requests.AggiornaDescrizioneFascicolo, AggiornaDescrizioneFascicoloResult>
    {
        #region Public Members

        public AggiornaDescrizioneFascicoloHandler(ILogger<AggiornaDescrizioneFascicoloHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IAggregazioneDocumentaleRepository aggregazioneDocumentaleRepository)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _aggregazioneDocumentaleRepository = aggregazioneDocumentaleRepository;

        }

        public async Task<AggiornaDescrizioneFascicoloResult> Handle(Application.Requests.AggiornaDescrizioneFascicolo request, CancellationToken cancellationToken)
        {
            var descFasc = request.descFasc;

            bool AlreadyExist = await DescAlreadyExist(descFasc);
            if (AlreadyExist)
            {
                return new AggiornaDescrizioneFascicoloResult(false, ResultDescrizioniFascicolo.DESCRIZIONE_PRESENTE);
            }

            try
            {
                var entity = await _dbContext.DescrizioneFascEntities.Where(w => w.SYSTEM_ID == descFasc.SystemId.AsLong()).FirstOrDefaultAsync();

                entity.VAR_DESCRIZIONE = descFasc.Descrizione.Replace("'", "''");
                entity.VAR_CODICE = descFasc.Codice.Replace("'", "''");
                ((DbContext)_dbContext).Update(entity);
                ((DbContext)_dbContext).SaveChanges();

                return new AggiornaDescrizioneFascicoloResult(true, ResultDescrizioniFascicolo.OK);
            }
            catch
            {
                return new AggiornaDescrizioneFascicoloResult(false, ResultDescrizioniFascicolo.KO);
            }
        }

        #endregion

        #region Private Members 

        protected virtual async Task<bool> DescAlreadyExist(DescrizioneFascicolo descFasc)
        {
            var result = _dbContext.DescrizioneFascEntities.Where(w => string.IsNullOrWhiteSpace(descFasc.Codice) ?
            (w.VAR_DESCRIZIONE.ToUpper() == descFasc.Descrizione.Replace("'", "''").ToUpper() || w.VAR_CODICE.ToUpper() == descFasc.Codice.Replace("'", "''").ToUpper())
            : w.VAR_DESCRIZIONE.ToUpper() == descFasc.Descrizione.Replace("'", "''").ToUpper() && w.ID_REGISTRO == descFasc.IdRegistro.AsLong() &&
            w.ID_AMM == descFasc.IdAmm.AsLong() && w.SYSTEM_ID != descFasc.SystemId.AsLong()).Any();

            return result;
        }

        protected readonly ILogger<AggiornaDescrizioneFascicoloHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IAggregazioneDocumentaleRepository _aggregazioneDocumentaleRepository;

        #endregion
    }

}
