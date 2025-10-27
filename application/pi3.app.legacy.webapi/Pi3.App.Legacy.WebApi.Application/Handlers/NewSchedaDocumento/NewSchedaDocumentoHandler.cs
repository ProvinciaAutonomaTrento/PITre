// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewSchedaDocumentoRequest = Pi3.App.Legacy.WebApi.Application.Requests.NewSchedaDocumento;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.NewSchedaDocumento
{

    // Richiede libreria MediatR
    public class NewSchedaDocumentoHandler : IRequestHandler<NewSchedaDocumentoRequest, NewSchedaDocumentoResult>
    {
        #region Public Members

        public NewSchedaDocumentoHandler(ILogger<NewSchedaDocumentoHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            ISessionRepositoryService sessionRepositoryService)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<NewSchedaDocumentoResult> Handle(NewSchedaDocumentoRequest request, CancellationToken cancellationToken)
        {
            SchedaDocumento schedaDocumento = new SchedaDocumento();

            try
            {
                schedaDocumento.systemId = null;
                schedaDocumento.oggetto = new Oggetto();
                schedaDocumento.idPeople = request.infoUtente.idPeople;
                schedaDocumento.userId = request.infoUtente.userId;
                schedaDocumento.typeId = "LETTERA";
                schedaDocumento.appId = "ACROBAT";
                schedaDocumento.privato = "0";
                schedaDocumento.repositoryContext = await _sessionRepositoryService.CreateRepository(request.infoUtente);
                schedaDocumento.documenti = new Documento[1];
                schedaDocumento.documenti[0] = new Documento()
                {
                    repositoryContext = schedaDocumento.repositoryContext,
                    applicazione = null,
                    autore = request.infoUtente.userId,
                    cartaceo = false,
                    daAggiornareFirmatari = false,
                    dataInserimento = DateTime.Now.AsDateFormat(),
                    descrizione = string.Empty,
                    docNumber = string.Empty,
                    docServerLoc = string.Empty,
                    fileName = string.Empty,
                    fileSize = "0",
                    fNversionId = string.Empty,
                    idPeople = request.infoUtente.idPeople,
                    msgErr = string.Empty,
                    path = string.Empty,
                    subVersion = "!",
                    version = "1",
                    versionId = string.Empty,
                    versionLabel = "1",
                    daInviare = "1",
                    dataArchiviazione = null,
                    dataArrivo = string.Empty
                };

                schedaDocumento.allegati = new Allegato[0];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new NewSchedaDocumentoResult(schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<NewSchedaDocumentoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        #endregion
    }

}
