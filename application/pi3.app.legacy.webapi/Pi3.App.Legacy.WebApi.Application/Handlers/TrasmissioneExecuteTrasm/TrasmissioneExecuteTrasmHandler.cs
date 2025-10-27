// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneExecuteTrasm
{

    // Richiede libreria MediatR
    public class TrasmissioneExecuteTrasmHandler : IRequestHandler<Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneExecuteTrasm, TrasmissioneExecuteTrasmResult>
    {
        #region Public Members

        public TrasmissioneExecuteTrasmHandler(ILogger<TrasmissioneExecuteTrasmHandler> logger, IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext,
            ITrasmissioneRepository trasmissioneRepository,
            IWebMethodLoggerService webMethodLoggerService
            )
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;
            this._trasmissioneRepository = trasmissioneRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }



        public async Task<TrasmissioneExecuteTrasmResult> Handle(Application.Requests.TrasmissioneExecuteTrasm request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.Trasmissione output = request.trasmissione;
            Trasmissione aggregate = null;

            try
            {
                var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

                aggregate = _trasmissioneRepository.Get(idTenant, request.trasmissione.systemId).Result;
                aggregate.Invia();
                await _trasmissioneRepository.Update(aggregate);
                request.trasmissione.dataInvio = aggregate.DataInvio.AsDateFormat() ?? string.Empty;

                foreach (var ts in aggregate.TrasmissioniSingole)
                {
                    var method = aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "TRASM_DOC_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_")
                        : "TRASM_FOLDER_" + ts.RagioneTrasmissione.Nome.ToUpper().Replace(" ", "_");

                    var objectDescription = string.Empty;
                    if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
                    {
                        objectDescription = output.infoDocumento.segnatura != null ? output.infoDocumento.segnatura : output.infoDocumento.docNumber;
                    }
                    else
                    {
                        objectDescription = output.infoFascicolo.idFascicolo;
                    }

                    await this._webMethodLoggerService.LogOK(method, aggregate.OggettoTrasmesso.Id, string.Format(Resources.DocTrasm, objectDescription), ts.Id);
                }
            }
            catch (Exception ex)
            {
                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
                {
                    if (output.infoDocumento.segnatura != null)
                        await this._webMethodLoggerService.LogKO("DOCUMENTOTRASMESSO", aggregate.OggettoTrasmesso.Id, string.Format(Resources.DocTrasm, output.infoDocumento.segnatura));
                    else
                        await this._webMethodLoggerService.LogKO("DOCUMENTOTRASMESSO", aggregate.OggettoTrasmesso.Id, string.Format(Resources.DocTrasm, output.infoDocumento.docNumber));
                }

                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new TrasmissioneExecuteTrasmResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneExecuteTrasmHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected readonly ITrasmissioneRepository _trasmissioneRepository;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;

        #endregion
    }

}
