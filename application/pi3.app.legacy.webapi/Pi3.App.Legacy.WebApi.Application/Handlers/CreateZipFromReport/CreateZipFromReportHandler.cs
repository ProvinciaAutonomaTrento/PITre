// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using DocsPaVO.PrjDocImport;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.DomainEventHandlers;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CreateZipFromReportRequest = Pi3.App.Legacy.WebApi.Application.Requests.CreateZipFromReport;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CreateZipFromReport
{
    public class CreateZipFromReportHandler : IRequestHandler<CreateZipFromReportRequest, CreateZipFromReportResult>
    {
        #region Public Members

        public CreateZipFromReportHandler(ILogger<CreateZipFromReportHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
        }

        public async Task<CreateZipFromReportResult> Handle(CreateZipFromReportRequest request, CancellationToken cancellationToken)
        {
            byte[] output = null;

            try
            {
                output = await this.CreateZipFromReport(request.report,request.infoUtente);
            }
            catch (Exception e)
            {
                this._logger.LogError(exception: e, message: e.Message);
            }

            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<CreateZipFromReportHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;


        private async Task<byte[]> CreateZipFromReport(ResultsContainer report, InfoUtente infoUtente)
        {

            using (var zipFileMemoryStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update))
                {
                    List<ImportResult> grayList = report.GrayDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
                    await AddEntry(grayList, "Documenti grigi", infoUtente, archive);
                    List<ImportResult> inList = report.InDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
                    await AddEntry(inList, "Documenti in arrivo", infoUtente, archive);
                    List<ImportResult> outList = report.OutDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
                    await AddEntry(outList, "Documenti in partenza", infoUtente, archive);
                    List<ImportResult> ownList = report.OwnDocument.FindAll(e => !string.IsNullOrEmpty(e.DocNumber));
                    await AddEntry(ownList, "Documenti interni", infoUtente, archive);

                }
                return zipFileMemoryStream.ToArray();

            }

        }


        private async Task AddEntry(List<ImportResult> results, string folder, InfoUtente infoUtente, ZipArchive archive)
        {
            foreach (ImportResult temp in results)
            {
                try
                {
                    List<BaseInfoDoc> infos = (await this._mediator.Send(new Application.Requests.GetBaseInfoForDocument(temp.DocNumber, null, null))).output;
                    BaseInfoDoc doc = infos[0];
                    FileRequest req = new FileRequest();
                    if (doc != null && !string.IsNullOrEmpty(doc.VersionLabel))
                        this._logger.LogDebug(string.Format(Resource.addMessage, doc.VersionLabel));
                    req.versionId = doc.VersionId;
                    req.docNumber = doc.IdProfile;
                    req.versionLabel = doc.VersionLabel;
                    req.path = doc.FileName;
                    req.version = doc.VersionLabel;
                    req.fileName = doc.FileName;
                    FileDocumento fileDocumento = (await this._mediator.Send(new Application.Requests.GetFileDocument(req, infoUtente))).output;
                    string extension = fileDocumento.fullName.Substring(fileDocumento.fullName.LastIndexOf(".") + 1);

                    ZipArchiveEntry entry = archive.CreateEntry(temp.DocNumber + "." + extension);

                    using (BinaryWriter writer = new (entry.Open()))
                    {
                        writer.Write(fileDocumento.content);
                    }


                }
                catch (Exception e)
                {
                    this._logger.LogInformation(string.Format(Resource.cantCreateZipEntry, temp.DocNumber));
                    this._logger.LogError(exception:e,message:e.Message);
                }
            }
        }

        #endregion
    }
}