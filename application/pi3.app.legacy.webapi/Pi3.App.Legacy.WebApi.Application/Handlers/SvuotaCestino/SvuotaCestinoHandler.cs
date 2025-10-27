// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Semplificata;
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.EliminaDoc;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.Interoperability;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SvuotaCestino
{

    // Richiede libreria MediatR
    public class SvuotaCestinoHandler : IRequestHandler<Application.Requests.SvuotaCestino, SvuotaCestinoResult>
    {
        #region Public Members

        public SvuotaCestinoHandler(ILogger<SvuotaCestinoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService,
            IConfigurationService configurationService,
            IDocumentoAmministrativoRepository documentRepository,
            IInteroperabilityService interopService,
            IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
            this._configurationService = configurationService;
            this._documentRepository = documentRepository;
            this._interopService = interopService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<SvuotaCestinoResult> Handle(Application.Requests.SvuotaCestino request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.InfoDocumento[] listaDoc = request.ListaDoc;

            bool result = false;
            bool docInCestino = false;

            try
            {
                var listadocInCestino = (await this._mediator.Send(new Application.Requests.DocumentoGetDocInCestino(request.infoUtente))).output;
                docInCestino = listadocInCestino.Count() != listaDoc.Length;

                foreach (var doc in listaDoc)
                {
                    result = (await this._mediator.Send(new Application.Requests.EliminaDoc(request.infoUtente, doc))).output; 
                }

                await this._webMethodLoggerService.LogOK("SVUOTACESTINO", objectDescription: Resources.LogSvuotaCestino);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                await this._webMethodLoggerService.LogKO("SVUOTACESTINO", objectDescription: Resources.LogSvuotaCestino);
            }

            return new SvuotaCestinoResult(result, docInCestino);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<SvuotaCestinoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;
        protected readonly IConfigurationService _configurationService;
        protected readonly IDocumentoAmministrativoRepository _documentRepository;
        protected readonly IInteroperabilityService _interopService;
        protected IHttpContextAccessor _httpContextAccessor;

        #endregion
    }

}
