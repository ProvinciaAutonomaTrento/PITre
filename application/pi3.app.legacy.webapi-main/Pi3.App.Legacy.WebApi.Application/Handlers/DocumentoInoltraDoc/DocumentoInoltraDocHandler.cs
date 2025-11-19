// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using DocumentoInoltraDocRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoInoltraDoc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoInoltraDoc
{
    public class DocumentoInoltraDocHandler : IRequestHandler<DocumentoInoltraDocRequest, DocumentoInoltraDocResult>
    {
        #region Public Members

        public DocumentoInoltraDocHandler(ILogger<DocumentoInoltraDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            ISessionRepositoryService sessionRepositoryService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            _sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<DocumentoInoltraDocResult> Handle(DocumentoInoltraDocRequest request, CancellationToken cancellationToken)
        {
            SchedaDocumento output = null;
            List<Allegato> allegati = new List<Allegato>();
            try
            {
                SchedaDocumento schedaDocumentoInoltro = (await this._mediator.Send(new Application.Requests.DocumentoGetDettaglioDocumento(request.infoUtente, request.schedaDocumento.systemId, request.schedaDocumento.docNumber))).output;

                output = (await this._mediator.Send(new Application.Requests.NewSchedaDocumento(request.infoUtente))).output;
                output.tipoProto = "P";
                output.predisponiProtocollazione = true;
                output.protocollo = new DocsPaVO.documento.ProtocolloUscita();
                output.protocollo.daProtocollare = "1";
                output.oggetto = schedaDocumentoInoltro.oggetto;
                output.oggetto.isOggettoModificato = false;
                ((ProtocolloUscita)output.protocollo).mittente = request.ruolo.uo;

                FileRequest versioneCorrente = (FileRequest)schedaDocumentoInoltro.documenti[0];
                Allegato allegatoPrincipale = new Allegato
                {
                    descrizione = Resources.DescrizioneDocumentoPrincipale,
                    fileName = versioneCorrente.fileName,
                    fileSize = versioneCorrente.fileSize,
                    firmato = versioneCorrente.firmato,
                    path = versioneCorrente.path,
                    subVersion = versioneCorrente.subVersion,
                    version = "1",
                    versionId = versioneCorrente.versionId,
                    versionLabel = string.Format("A{0:0#}", 1),
                    numeroPagine = 0,
                    dataInserimento = string.Empty
                };
                if (!string.IsNullOrEmpty(versioneCorrente.fileSize) && versioneCorrente.fileSize != "0")
                {
                    FileDocumento file = (await this._mediator.Send(new Application.Requests.GetFileDocument(versioneCorrente, request.infoUtente))).output;
                    await _sessionRepositoryService.SetFile(output.repositoryContext, allegatoPrincipale, file);
                }
                allegatoPrincipale.repositoryContext = output.repositoryContext;
                allegati.Add(allegatoPrincipale);

                foreach (Allegato allegato in schedaDocumentoInoltro.allegati.Where(a => a.TypeAttachment != 6 && a.TypeAttachment != 3 && a.TypeAttachment != 2 && a.TypeAttachment != 5))
                {
                    //Mev Gestione eccezioni - nell'inoltra il segnatura.xml non deve essere preso tra gli allegati
                    if (!string.IsNullOrEmpty(schedaDocumentoInoltro.interop)
                        && (schedaDocumentoInoltro.interop.Equals("S") || schedaDocumentoInoltro.interop.Equals("E"))
                        && allegato.descrizione != null && allegato.descrizione.ToLower().Contains("segnatura.xml"))
                        continue;

                    allegato.versionLabel = string.Format("A{0:0#}", allegati.Count + 1);
                    allegato.dataInserimento = string.Empty;
                    allegato.descrizione = string.Format("{0} {1}", Resources.Inoltro, allegato.descrizione);
                    if (allegato.descrizione.Length > 2000)
                        allegato.descrizione = string.Format("{0}...", allegato.descrizione.Substring(0, 1900));

                    if (!string.IsNullOrEmpty(allegato.fileSize) && allegato.fileSize != "0" && !string.IsNullOrEmpty(allegato.fileName))
                    {
                        FileDocumento file = (await this._mediator.Send(new Application.Requests.GetFileDocument(allegato, request.infoUtente))).output;
                        await _sessionRepositoryService.SetFile(output.repositoryContext, allegato, file);
                    }
                    allegato.repositoryContext = output.repositoryContext;
                    allegati.Add(allegato);
                }

                output.allegati = allegati.ToArray();

            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new DocumentoInoltraDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoInoltraDocHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly ISessionRepositoryService _sessionRepositoryService;

        #endregion
    }
}
