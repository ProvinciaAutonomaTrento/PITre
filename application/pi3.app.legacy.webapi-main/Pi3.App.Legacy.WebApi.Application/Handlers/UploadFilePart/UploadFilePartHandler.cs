// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadFilePartRequest = Pi3.App.Legacy.WebApi.Application.Requests.UploadFilePart;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UploadFilePart
{
    public class UploadFilePartHandler : IRequestHandler<UploadFilePartRequest, UploadFilePartResult>
    {
        #region Public Members

        public UploadFilePartHandler(
            ILogger<UploadFilePartHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
        }

        public async Task<UploadFilePartResult> Handle(UploadFilePartRequest request, CancellationToken cancellationToken)
        {
            bool uploaded = false;

            try
            {
                var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
                var tenantCode = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.TenantCode, true);

                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);

                var directory = Path.Combine(
                            repositoryRootPath,
                            tenantCode.ToUpper(),
                            "TemporaryUploads",
                            request.uuid).PathAsUnixPath();

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var filePath = Path.Combine(directory, $"{request.fileName}.part");

                if (request.partIndex > 1 && !File.Exists(filePath))
                    throw new FileNotFoundException(filePath);

                this._logger.LogDebug($"UploadFilePartHandler - FilePath:{filePath}");

                using (var fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None))
                using (var bw = new BinaryWriter(fs))
                    bw.Write(request.fileStream);

                this._logger.LogInformation($"UploadFilePartHandler - Scrittura part completata");
                uploaded = true;
            }
            catch (Pi3Exception pi3Ex)
            {
                this._logger.LogError(pi3Ex, null, null);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(ex, null, null);
            }

            return new UploadFilePartResult(uploaded);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadFilePartHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;

        #endregion
    }
}