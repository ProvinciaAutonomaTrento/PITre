// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.areaConservazione;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetParoleChiave
{

    // Richiede libreria MediatR
    public class DocumentoGetParoleChiaveHandler : IRequestHandler<Application.Requests.DocumentoGetParoleChiave, DocumentoGetParoleChiaveResult>
    {
        #region Public Members

        public DocumentoGetParoleChiaveHandler(ILogger<DocumentoGetParoleChiaveHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;
        }

        public async Task<DocumentoGetParoleChiaveResult> Handle(Application.Requests.DocumentoGetParoleChiave request, CancellationToken cancellationToken)
        {
            long idAmm = request.idAmministrazione.AsLong();

            return new DocumentoGetParoleChiaveResult
                (_dbContext.ParolaEntities.Where(w => w.ID_AMM == idAmm).Select(s => new DocsPaVO.documento.ParolaChiave() {
                    systemId = s.SYSTEM_ID.ToString(),
                    idAmministrazione = s.ID_AMM.ToString(),
                    descrizione = s.VAR_DESC_PAROLA
                }).OrderBy(s => s.descrizione).ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetParoleChiaveHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IWebMethodLoggerService _webMethodLoggerService;


        #endregion
    }

}
