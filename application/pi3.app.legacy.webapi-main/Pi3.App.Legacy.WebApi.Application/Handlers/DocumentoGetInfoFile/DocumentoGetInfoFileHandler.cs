// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.utente.Repertori;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoGetInfoFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoGetInfoFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoGetInfoFile
{
    public class DocumentoGetInfoFileHandler : IRequestHandler<DocumentoGetInfoFileRequest, DocumentoGetInfoFileResult>
    {
        #region Public Members

        public DocumentoGetInfoFileHandler(ILogger<DocumentoGetInfoFileHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }

        public async Task<DocumentoGetInfoFileResult> Handle(DocumentoGetInfoFileRequest request, CancellationToken cancellationToken)
        {
            FileDocumento output = new FileDocumento();

            try
            {
                var versionId = !string.IsNullOrEmpty(request.fileRequest.versionId) ? request.fileRequest.versionId.AsLong() : 0;
                var docnumber = !string.IsNullOrEmpty(request.fileRequest.docNumber) ? request.fileRequest.docNumber.AsLong() : 0;

                output.path = request.fileRequest.docServerLoc + request.fileRequest.path;
                output.name = request.fileRequest.fileName != null ? request.fileRequest.fileName : string.Empty;

                int indice = output.name.LastIndexOf(@"\");
                if (indice < (output.name.Length - 1))
                    output.name = output.name.Substring(indice + 1);

                if (!string.IsNullOrEmpty(request.fileRequest.versionId) && !string.IsNullOrEmpty(request.fileRequest.docNumber))
                {
                    var nomeOriginale = await this._dbContext.ComponentEntities.AsNoTracking().Where(c => c.VERSION_ID == versionId && c.DOCNUMBER == docnumber).Select(c => c.VAR_NOMEORIGINALE).FirstOrDefaultAsync();
                    output.nomeOriginale = removeIllegalChars(nomeOriginale);
                }

                output.fullName = string.IsNullOrEmpty(request.fileRequest.path) ? '\u005C'.ToString() + output.name : request.fileRequest.fileName;

                string[] extArr = output.name.Split('.');
                var ext = extArr[extArr.Length - 1].ToLower();

                List<Applicazione> apps = await GetApplications(ext);
                if (apps != null && apps.Count > 0)
                {
                    var application =  apps[0];
                    output.contentType = application != null && !string.IsNullOrEmpty(application.mimeType) ? application.mimeType : "application/x-" + ext;
                }

                var extIntoSignedFile = string.Empty;
                var fileName = new FileInfo(output.fullName).Name;
                string[] items = fileName.Split('.');
                for (int i = (items.Length - 1); i >= 0; i--)
                {
                    if (!(items[i].ToUpper().EndsWith("P7M") || items[i].ToUpper().EndsWith("TSD") || items[i].ToUpper().EndsWith("M7M")))
                    {
                        extIntoSignedFile = items[i];
                        break;
                    }
                }
                output.estensioneFile = extIntoSignedFile;

                output.firmaElettronica = (await _mediator.Send(new Requests.GetElectronicSignatureDocument(request.fileRequest.docNumber, request.fileRequest.versionId, request.infoUtente))).output;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new DocumentoGetInfoFileResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoGetInfoFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected string removeIllegalChars(string filename)
        {

            if (string.IsNullOrEmpty(filename))
                return filename;

            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
                filename = filename.Replace(c.ToString(), "_");

            return filename;
        }

        protected virtual async Task<List<Applicazione>> GetApplications(string ext)
        {
            this._logger.LogDebug($"DocumentoGetInfoFile > GetApplications > ext: {ext}");
            List<Applicazione> output = new List<Applicazione>();

            if (!string.IsNullOrEmpty(ext))
            {
                var appsEntity = await this._dbContext.AppEntities.AsNoTracking()
                    .Where(a => a.DEFAULT_EXTENSION.ToUpper() == ext.ToUpper())
                    .Select(a => new
                    {
                        a.DEFAULT_EXTENSION,
                        a.MIME_TYPE
                    })
                    .ToListAsync();

                appsEntity.ForEach(a =>
                 output.Add(new Applicazione
                 {
                     estensione = a.DEFAULT_EXTENSION,
                     mimeType = a.MIME_TYPE
                 })
                );

                if (output.Count == 0)
                {
                    this._logger.LogDebug($"DocumentoGetInfoFile > GetApplications > inserisco ext: {ext}");
                    var appEntity = new AppEntity
                    {
                        APPLICATION = "GEN_" + ext,
                        DESCRIPTION = "GEN_" + ext,
                        FILING_SCHEME = 2,
                        DEFAULT_EXTENSION = ext
                    };

                    await this._dbContext.AppEntities.AddAsync(appEntity);

                    await ((DbContext)_dbContext).SaveChangesAsync();

                    output.Add(new Applicazione
                    {
                        estensione = appEntity.DEFAULT_EXTENSION,
                        mimeType = appEntity.MIME_TYPE
                    });
                }

            }
            return output;
        }
        #endregion
    }
}
