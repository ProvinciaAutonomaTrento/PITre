// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoRiproponiConCopiaDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoRiproponiConCopiaDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoRiproponiConCopiaDoc
{
    public class DocumentoRiproponiConCopiaDocHandler : IRequestHandler<DocumentoRiproponiConCopiaDocRequest, DocumentoRiproponiConCopiaDocResult>
    {
        #region Public Members

        public DocumentoRiproponiConCopiaDocHandler(ILogger<DocumentoRiproponiConCopiaDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<DocumentoRiproponiConCopiaDocResult> Handle(DocumentoRiproponiConCopiaDocRequest request, CancellationToken cancellationToken)
        {
            SchedaDocumento schedaDocumento = null;
            try
            {
                schedaDocumento = (await this._mediator.Send(new Application.Requests.DocumentoGetDettaglioDocumento(request.infoUtente, request.schedaDocumento.systemId, request.schedaDocumento.docNumber))).output; ;
                schedaDocumento.repositoryContext = await _sessionRepositoryService.CreateRepository(request.infoUtente);

                schedaDocumento.systemId = null;
                schedaDocumento.docNumber = null;
                schedaDocumento.dataCreazione = null;
                schedaDocumento.oraCreazione = null;
                schedaDocumento.checkOutStatus = null;
                schedaDocumento.accessRights = null;
                schedaDocumento.noteDocumento = new List<DocsPaVO.Note.InfoNota>();
                schedaDocumento.rispostaDocumento = null;
                schedaDocumento.riferimentoMittente = null;
                schedaDocumento.checkOutStatus = null;
                schedaDocumento.autore = null;
                schedaDocumento.creatoreDocumento = null;
                schedaDocumento.protocollatore = null;
                schedaDocumento.protocollo = null;

                Documento documento = (Documento)schedaDocumento.documenti[0];
                schedaDocumento.documenti = new Documento[] { documento };

                FileRequest fileRequest = (FileRequest)schedaDocumento.documenti[0];
                fileRequest.version = "1";
                fileRequest.versionLabel = "1";
                if (!string.IsNullOrEmpty(fileRequest.fileSize) && fileRequest.fileSize != "0" && !string.IsNullOrEmpty(fileRequest.fileName))
                {
                    FileDocumento file = (await this._mediator.Send(new Application.Requests.GetFileDocument(fileRequest, request.infoUtente))).output;
                    await _sessionRepositoryService.SetFile(((FileRequest)schedaDocumento.documenti[0]).repositoryContext, ((FileRequest)schedaDocumento.documenti[0]), file);
                }
                ((FileRequest)schedaDocumento.documenti[0]).dataInserimento = string.Empty;
                ((FileRequest)schedaDocumento.documenti[0]).repositoryContext = schedaDocumento.repositoryContext;

                foreach (Allegato allegato in schedaDocumento.allegati)
                {
                    if (!string.IsNullOrEmpty(allegato.fileSize) && allegato.fileSize != "0" && !string.IsNullOrEmpty(allegato.fileName))
                    {
                        FileDocumento file = (await this._mediator.Send(new Application.Requests.GetFileDocument(allegato, request.infoUtente))).output;
                        await _sessionRepositoryService.SetFile(schedaDocumento.repositoryContext, allegato, file);
                    }
                    allegato.dataInserimento = string.Empty;
                    allegato.repositoryContext = schedaDocumento.repositoryContext;
                }

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                schedaDocumento = null;
            }

            return new DocumentoRiproponiConCopiaDocResult(schedaDocumento);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoRiproponiConCopiaDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        #endregion
    }
}
