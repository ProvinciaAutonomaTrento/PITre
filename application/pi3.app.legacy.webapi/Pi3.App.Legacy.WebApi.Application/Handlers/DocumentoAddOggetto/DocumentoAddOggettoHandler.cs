// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.FascicolazioneNewFolder;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate.Repositories;
using Pi3.Core.AggregateModels.OggettoAggregate;
using Pi3.Core.AggregateModels.OggettoAggregate.Repositories;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoAddOggettoRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoAddOggetto;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddOggetto
{
    public class DocumentoAddOggettoHandler : IRequestHandler<DocumentoAddOggettoRequest, DocumentoAddOggettoResult>
    {
        #region Public Members

        public DocumentoAddOggettoHandler(ILogger<DocumentoAddOggettoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IOggettoRepository oggettoRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._oggettoRepository = oggettoRepository;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoAddOggettoResult> Handle(DocumentoAddOggettoRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.Oggetto output = request.oggetto;
            var errMsg = string.Empty;
            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);
                var registerId = request.registro != null && !string.IsNullOrEmpty(request.registro.systemId) ? request.registro.systemId : null;

                var aggregate = new Oggetto(idTenant,
                                DateTime.Now,
                                new Core.SeedWork.TextValue(output.codOggetto), 
                                registerId, null, null, 
                                new Core.SeedWork.TextValue(output.descrizione));

                await this._oggettoRepository.Add(aggregate);

                output.systemId = aggregate.Id;

                await this._webMethodLoggerService.LogOK("DOCUMENTOINSOGGETTO", output.systemId, string.Format(Resources.LogInserimentoOggetto, output.descrizione));
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                errMsg = ex.Message;
                await this._webMethodLoggerService.LogKO("DOCUMENTOINSOGGETTO", output.systemId, string.Format(Resources.LogInserimentoOggetto, output.descrizione));
                output = null;
            }

            return new DocumentoAddOggettoResult(output, errMsg);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAddOggettoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected IOggettoRepository _oggettoRepository;

        #endregion
    }
}
