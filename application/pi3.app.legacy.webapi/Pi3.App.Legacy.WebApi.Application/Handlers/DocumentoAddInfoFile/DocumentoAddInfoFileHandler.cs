// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoPutFileNoException;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Services.SessionRepository;
using Pi3.Core.AggregateModels.DocumentBlobAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.File.CAdES;
using Pi3.Core.Services.File.FileValidator;
using Pi3.Core.Services.File.PAdES;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentoAddInfoFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.DocumentoAddInfoFileRequest;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoAddInfoFile
{
    
    public class DocumentoAddInfoFileHandler : IRequestHandler<DocumentoAddInfoFileRequest, DocumentoAddInfoFileResult>
    {
        #region Public Members

        public DocumentoAddInfoFileHandler(ILogger<DocumentoAddInfoFileHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<DocumentoAddInfoFileResult> Handle(DocumentoAddInfoFileRequest request, CancellationToken cancellationToken)
        {
            long docnumber = request.fileRequest.docNumber.AsLong();
            long versionId = request.fileRequest.versionId.AsLong();

            try
            {
                var infoFileEntity = await this._pi3DbContext.InfoFileEntities
                               .Where(i => i.ID_PROFILE == docnumber)
                               .FirstOrDefaultAsync();

                if (infoFileEntity == null)
                {
                    infoFileEntity = new InfoFileEntity()
                    {
                        ID_PROFILE = docnumber,
                        ID_DOCUMENTO_PRINCIPALE = request.idDocumentoPrincipale,
                        VERSION_ID = versionId
                    };

                    this._pi3DbContext.InfoFileEntities.Add(infoFileEntity);
                }

                infoFileEntity.VERSION_ID = versionId;
                infoFileEntity.DTA_ACQUISIZIONE = null;
                infoFileEntity.VAR_ESTENSIONE = string.Empty;
                infoFileEntity.VAR_NOME_FILE = string.Empty;
                infoFileEntity.VAR_DESC_INFO_FILE = string.Empty;
                infoFileEntity.CHA_CONFORME = "1";
                infoFileEntity.CHA_ESTENSIONE_CONFORME = "1";
                infoFileEntity.CHA_PRESENZA_MACRO = "0";
                infoFileEntity.CHA_PRESENZA_FORMS = "0";
                infoFileEntity.CHA_PRESENZA_JAVASCRIPT = "0";

                var fileName = request.fileRequest.fileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    infoFileEntity.DTA_ACQUISIZIONE = !string.IsNullOrEmpty(request.fileRequest.dataAcquisizione) ? request.fileRequest.dataAcquisizione.AsDateTime() : null;
                    infoFileEntity.VAR_ESTENSIONE = Path.GetExtension(fileName).Replace(".", string.Empty);
                    infoFileEntity.VAR_NOME_FILE = fileName;
                }

                if (request.fileValidateResult != null)
                {
                    infoFileEntity.CHA_CONFORME = request.fileValidateResult.Compliance!.IsCompliantToFormat
                                                               && !request.fileValidateResult.Compliance.HasMacro.GetValueOrDefault()
                                                               && !request.fileValidateResult.Compliance.HasForms.GetValueOrDefault()
                                                               && !request.fileValidateResult.Compliance.HasJavascript.GetValueOrDefault() ? "1" : "0";
                    infoFileEntity.CHA_ESTENSIONE_CONFORME = request.fileValidateResult.Compliance.IsCompliantToFormat ? "1" : "0";
                    infoFileEntity.CHA_PRESENZA_MACRO = request.fileValidateResult.Compliance.HasMacro.GetValueOrDefault() ? "1" : "0";
                    infoFileEntity.CHA_PRESENZA_FORMS = request.fileValidateResult.Compliance.HasForms.GetValueOrDefault() ? "1" : "0";
                    infoFileEntity.CHA_PRESENZA_JAVASCRIPT = request.fileValidateResult.Compliance.HasJavascript.GetValueOrDefault() ? "1" : "0";
                    infoFileEntity.CHA_NOTIFICA = "0";

                    if (infoFileEntity.CHA_CONFORME == "0")
                    {
                        if (!request.fileValidateResult.Compliance.IsCompliantToFormat)
                            infoFileEntity.VAR_DESC_INFO_FILE = "NON_CONFORME";

                        if (request.fileValidateResult.Compliance.HasMacro.GetValueOrDefault())
                            infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "MACRO" : ",MACRO";

                        if (request.fileValidateResult.Compliance.HasForms.GetValueOrDefault())
                            infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "FormPDF" : ",FormPDF";

                        if (request.fileValidateResult.Compliance.HasJavascript.GetValueOrDefault())
                            infoFileEntity.VAR_DESC_INFO_FILE = string.IsNullOrEmpty(infoFileEntity.VAR_DESC_INFO_FILE) ? "JAVASCRIPT" : ",JAVASCRIPT";
                    }
                }
                
                await ((Pi3DbContext)this._pi3DbContext).SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new DocumentoAddInfoFileResult();

        }

        #endregion

        #region Private Members

        protected readonly ILogger<DocumentoAddInfoFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IFileValidatorService _fileValidatorService;

        #endregion
    }

}
