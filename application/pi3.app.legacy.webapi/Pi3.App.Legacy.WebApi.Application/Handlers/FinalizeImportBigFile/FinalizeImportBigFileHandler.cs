// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.FinalizeUploadBigFile;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.File.Converters;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalizeImportBigFileRequest = Pi3.App.Legacy.WebApi.Application.Requests.FinalizeImportBigFile;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.FinalizeImportBigFile
{
    public class FinalizeImportBigFileHandler : IRequestHandler<FinalizeImportBigFileRequest, FinalizeImportBigFileResult>
    {
        #region Public Members

        public FinalizeImportBigFileHandler(
            ILogger<FinalizeImportBigFileHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
        }

        public async Task<FinalizeImportBigFileResult> Handle(FinalizeImportBigFileRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            string temporaryUploadPath = null!;
            string sourceFilePath = null!;
            string destinationFilePath = null!;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var idUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                temporaryUploadPath = sourceFilePath = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads",
                            request.uuid);

                sourceFilePath = Path.Combine(
                            temporaryUploadPath,
                            $"{request.fileName}.part")
                        .PathAsUnixPath();

                this._logger.LogDebug($"FinalizeImportBigFileHandler - sourceFilePath: {sourceFilePath}");

                if (!File.Exists(sourceFilePath))
                    throw new FileNotFoundException(sourceFilePath);

                destinationFilePath = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "Import",
                            "Documents",
                            idUser,
                            DateTime.Now.ToString("yyyyMMdd")).PathAsUnixPath();

                if (request.isAttachment)
                {
                    destinationFilePath = Path.Combine(
                                destinationFilePath,
                                "Attachments").PathAsUnixPath();
                }

                if (!Directory.Exists(destinationFilePath))
                {
                    Directory.CreateDirectory(destinationFilePath);
                }

                destinationFilePath = Path.Combine(
                        destinationFilePath,
                        request.fileName)
                        .PathAsUnixPath();

                this._logger.LogDebug($"FinalizeImportBigFileHandler - destinationFilePath: {destinationFilePath}");

                File.Move(sourceFilePath, destinationFilePath,true);

                this._logger.LogDebug($"FinalizeImportBigFileHandler - spostamento completato");

                output = true;
            }
            catch (Pi3Exception pi3Ex)
            {
                output = false;

                this._logger.LogError(pi3Ex, pi3Ex.Message);
            }
            catch (Exception ex)
            {
                output = false;

                this._logger.LogCritical(ex, ex.Message);
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(temporaryUploadPath) && Directory.Exists(temporaryUploadPath))
                        Directory.Delete(temporaryUploadPath, true);
                }
                catch
                { }
            }

            return new FinalizeImportBigFileResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<FinalizeImportBigFileHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}