// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.documento;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalizeUploadBigFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.FinalizeUploadBigFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FinalizeUploadBigFile
{
    public class FinalizeUploadBigFileHandler : IRequestHandler<FinalizeUploadBigFileRequest, FinalizeUploadBigFileResult>
    {
        #region Public Members

        public FinalizeUploadBigFileHandler(
            ILogger<FinalizeUploadBigFileHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator, 
            IConfigurationService configurationService,
            IFileConverterFactory fileConverterFactory)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._fileConverterFactory = fileConverterFactory;
        }

        public async Task<FinalizeUploadBigFileResult> Handle(FinalizeUploadBigFileRequest request, CancellationToken cancellationToken)
        {
            bool finalized = false;
            DocsPaVO.documento.FileRequest? finalizedFileRequest = request.fileRequest;
            string directory = String.Empty;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                directory = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads",
                            request.uuid)
                        .PathAsUnixPath();

                var filePath = Path.Combine(
                            directory,
                            $"{request.fileName}.part")
                        .PathAsUnixPath();

                this._logger.LogDebug($"FinalizeUploadBigFileHandler - filePath: {filePath}");

                if (!File.Exists(filePath))
                    throw new FinalizeUploadBigFilePi3Exception($"File '{filePath}' non trovato.");

                var destinationFilePath = Path.Combine(
                        directory,
                        $"{request.fileName}")
                    .PathAsUnixPath();

                this._logger.LogDebug($"FinalizeUploadBigFileHandler - destinationFilePath: {destinationFilePath}");

                File.Move(filePath, destinationFilePath);

                if (request.convertPdfSync)
                {
                    this._logger.LogDebug($"FinalizeUploadBigFileHandler - richiesta conversione pdf sincrona per file {destinationFilePath}");
                    
                    var creation = await this._fileConverterFactory.TryCreate(destinationFilePath);

                    if (creation.Success)
                    {
                        try
                        {
                            using var stream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            var converted = await creation.Service.Convert(destinationFilePath, stream, FileConverterOutputFormatsEnum.ToPdf);

                            destinationFilePath = Path.Combine(directory, converted.FileName);

                            var originalFileName = System.IO.Path.GetFileNameWithoutExtension(request.fileDocument.name) + ".pdf";

                            request.fileDocument.fullName = destinationFilePath;
                            request.fileDocument.name = originalFileName;
                            request.fileDocument.contentType = converted.ContentType;
                            finalizedFileRequest.fileName = converted.FileName;

                            this._logger.LogDebug($"FinalizeUploadBigFileHandler - conversione pdf sincrona completata, file {destinationFilePath}");

                            File.WriteAllBytes(destinationFilePath, converted.Content);
                        }
                        catch (Pi3Exception pi3ExConv)
                        {
                            this._logger.LogError(pi3ExConv, pi3ExConv.Message);
                        }
                        catch (Exception exConv)
                        {
                            this._logger.LogCritical(exConv, exConv.Message);
                        }
                    }
                    else
                    {
                        this._logger.LogDebug($"FinalizeUploadBigFileHandler - conversione pdf non supportata per file {destinationFilePath}");
                    }
                }

                request.fileDocument.content = System.IO.File.ReadAllBytes(destinationFilePath);                
                request.fileDocument.length = request.fileDocument.content.Length;

                var putFileResult = await this._mediator.Send(
                    new Requests.DocumentoPutFileNoException(
                        request.fileRequest,
                        new DocsPaVO.documento.BigFileDocumento(request.fileDocument, destinationFilePath), 
                        request.infoUtente));

                if (!putFileResult.output)
                    throw new FinalizeUploadBigFilePi3Exception(putFileResult.errorMessage);

                finalizedFileRequest = putFileResult.fileRequest;

                finalized = true;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, pi3Ex.Message);
                
                finalized = false;
                //finalizedFileRequest = null;
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, ex.Message);

                finalized = false;
                //finalizedFileRequest = null;
            }
            finally
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }

            return new FinalizeUploadBigFileResult(finalized, finalizedFileRequest);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FinalizeUploadBigFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IFileConverterFactory _fileConverterFactory;

        #endregion
    }
}