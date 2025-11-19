// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.Import.Pregressi;
using DocsPaVO.InstanceAccess;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForwardsInstanceAccessRequest = Pi3.App.Legacy.WebApi.Application.Requests.ForwardsInstanceAccess;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ForwardsInstanceAccess
{
    public class ForwardsInstanceAccessHandler : IRequestHandler<ForwardsInstanceAccessRequest, ForwardsInstanceAccessResult>
    {
        protected readonly ILogger<ForwardsInstanceAccessHandler> _logger;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IMediator _mediator;
        protected readonly ISessionRepositoryService _sessionRepositoryService;


        private string FormatCodiceAllegato(int indexAllegato)
        {
            return string.Format("A{0:0#}", indexAllegato);
        }

        protected class InfoForward
        {
            public DocsPaVO.documento.SchedaDocumento Documento { get; set; }
            public int TotalFileSizeInstance { get; set; }
        }
        private async Task<InfoForward> ForwardsInstance(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo)
        {
            DocsPaVO.documento.SchedaDocumento documento = null;
            int fileSize;
            int totalFileSizeInstance = 0;


            try
            {
                DocsPaVO.ProfilazioneDinamica.Templates template = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.GetTemplateInstanceAccess(infoUtente))).output;
                documento = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.NewSchedaDocumento(infoUtente))).output;
                documento.tipoProto = "P";
                documento.predisponiProtocollazione = true;
                documento.protocollo = new DocsPaVO.documento.ProtocolloUscita();
                documento.protocollo.daProtocollare = "1";
                DocsPaVO.utente.Corrispondente corr = ruolo.uo;
                ((DocsPaVO.documento.ProtocolloUscita)documento.protocollo).mittente = corr;

                SessionRepositoryContext? repositoryFileManager = await this._sessionRepositoryService.CreateRepository(infoUtente);
                documento.oggetto.descrizione = instance.DESCRIPTION;
                List<Allegato> allegati = new();

                if (instance.DOCUMENTS != null)
                {
                    foreach (InstanceAccessDocument doc in instance.DOCUMENTS)
                    {
                        DocsPaVO.documento.SchedaDocumento schedaDoc = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetDettaglioDocumentoNoSecurity(infoUtente, doc.INFO_DOCUMENT.DOCNUMBER, doc.INFO_DOCUMENT.DOCNUMBER))).output;
                        if (doc.ENABLE)
                        {
                            DocsPaVO.documento.FileRequest versioneCorrente = (DocsPaVO.documento.FileRequest)schedaDoc.documenti[0];
                            Int32.TryParse(versioneCorrente.fileSize, out fileSize);

                            DocsPaVO.documento.Allegato allegatoPrincipale = new DocsPaVO.documento.Allegato
                            {
                                descrizione = "Inoltro documento principale " + schedaDoc.oggetto.descrizione,
                                fileName = versioneCorrente.fileName,
                                fileSize = versioneCorrente.fileSize,
                                path = versioneCorrente.path,
                                repositoryContext = versioneCorrente.repositoryContext,
                                subVersion = versioneCorrente.subVersion,
                                version = "1",
                                versionId = versioneCorrente.versionId,
                                versionLabel = this.FormatCodiceAllegato(documento.allegati.Count() + 1),
                                numeroPagine = 0,
                                dataInserimento = string.Empty
                            };

                            if (fileSize > 0 && !string.IsNullOrEmpty(versioneCorrente.fileName))
                            {
                                totalFileSizeInstance += Convert.ToInt32(versioneCorrente.fileSize);
                                if (((double)totalFileSizeInstance / 1048576) > 20)
                                {
                                    return null;
                                }
                                // Reperimento del file associato alla versione corrente
                                DocsPaVO.documento.FileDocumento file = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato(versioneCorrente, infoUtente))).output;

                                // Copia del file nel repository temporaneo
                                await this._sessionRepositoryService.SetFile(repositoryFileManager, allegatoPrincipale, file);


                            }

                            allegatoPrincipale.repositoryContext = documento.repositoryContext;
                            allegati.Add(allegatoPrincipale);

                        }
                        documento.allegati = allegati.ToArray();
                        if ((schedaDoc.allegati != null && schedaDoc.allegati.Count() > 0) &&
                            (template == null || !doc.INFO_DOCUMENT.DESCRIPTION_TIPOLOGIA_ATTO.Equals(template.DESCRIZIONE)))
                        {
                            foreach (Allegato allegato in schedaDoc.allegati)
                            {
                                if ((from att in doc.ATTACHMENTS where att.ID_ATTACH.Equals(allegato.docNumber) && att.ENABLE select att).FirstOrDefault() != null)
                                {
                                    allegato.descrizione = string.Format("Inoltro {0}", allegato.descrizione);
                                    allegato.dataInserimento = string.Empty;

                                    if (allegato.descrizione.Length > 2000)
                                        allegato.descrizione = string.Format("{0}...", allegato.descrizione.Substring(0, 1900));

                                    // Reperimento versionlabel per l'allegato
                                    allegato.versionLabel = this.FormatCodiceAllegato(documento.allegati.Count() + 1);
                                    Int32.TryParse(allegato.fileSize, out fileSize);

                                    if (fileSize > 0 && !string.IsNullOrEmpty(allegato.fileName))
                                    {
                                        totalFileSizeInstance += Convert.ToInt32(allegato.fileSize);
                                        if (((double)totalFileSizeInstance / 1048576) > 20)
                                        {
                                            return null;
                                        }
                                        // Reperimento del file associato alla versione corrente
                                        DocsPaVO.documento.FileDocumento file = (await this._mediator.Send(new Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetFileFirmato(allegato, infoUtente))).output;

                                        // Copia del file nel repository temporaneo
                                        await this._sessionRepositoryService.SetFile(repositoryFileManager, allegato, file);

                                    }

                                    allegato.repositoryContext = documento.repositoryContext;
                                    allegati.Add(allegato);
                                }
                            }
                            documento.allegati = allegati.ToArray();

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                return null;

            }
            return new()
            {
                Documento = documento,
                TotalFileSizeInstance = totalFileSizeInstance
            };

        }



        public ForwardsInstanceAccessHandler(
            ILogger<ForwardsInstanceAccessHandler> logger,
            IPi3DbContext dbContext,
            IMediator mediator,
            ISessionRepositoryService sessionRepositoryService
            )
        {
            this._dbContext = dbContext;
            this._logger = logger;
            this._mediator = mediator;
            this._sessionRepositoryService = sessionRepositoryService;
        }

        public async Task<ForwardsInstanceAccessResult> Handle(ForwardsInstanceAccessRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.documento.SchedaDocumento output = null;
            int totalFileSizeInstance = 0;
            try
            {
                var fwResult = await this.ForwardsInstance(request.instance,request.infoUtente,request.ruolo);
                output = fwResult.Documento;
                totalFileSizeInstance = fwResult.TotalFileSizeInstance;
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }
            return new(output, totalFileSizeInstance);

        }

    }
}
