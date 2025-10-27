// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetCorrispondentiLista
{

    // Richiede libreria MediatR
    public class GetCorrispondentiListaHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.getCorrispondentiLista, getCorrispondentiListaResult>
    {
        #region Public Members

        public GetCorrispondentiListaHandler(ILogger<GetCorrispondentiListaHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator, IPi3DbContext dbContext, IListaDistribuzioneRepository listaDistribuzioneRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._listaDistribuzioneRepository = listaDistribuzioneRepository;
        }

        public async Task<getCorrispondentiListaResult> Handle(getCorrispondentiLista request, CancellationToken cancellationToken)
        {
            DataSet corrInLista = null;

            try
            {
                corrInLista = this._dbContext.ListeDistrEntities
                    .Join(this._dbContext.CorrGlobaliEntities, l => l.ID_DPA_CORR, c => c.SYSTEM_ID, (l, c) => new { l, c })
                    .Where(x => x.l.ID_LISTA_DPA_CORR == request.codiceLista.AsLong() && x.c.DTA_FINE == null)
                    .Select(x => new
                    {
                        ID_DPA_CORR = x.l.ID_DPA_CORR,
                        VAR_DESC_CORR = x.c.VAR_DESC_CORR,
                        VAR_COD_RUBRICA = x.c.VAR_COD_RUBRICA,
                        CHA_TIPO_IE = x.c.CHA_TIPO_IE,
                        CHA_DISABLED_TRASM = x.c.CHA_DISABLED_TRASM
                    })
                    .OrderBy(x => x.VAR_DESC_CORR.TrimStart()).AsDataSet();

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }


            return new getCorrispondentiListaResult(corrInLista);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetCorrispondentiListaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IListaDistribuzioneRepository _listaDistribuzioneRepository;


        #endregion
    }

}
