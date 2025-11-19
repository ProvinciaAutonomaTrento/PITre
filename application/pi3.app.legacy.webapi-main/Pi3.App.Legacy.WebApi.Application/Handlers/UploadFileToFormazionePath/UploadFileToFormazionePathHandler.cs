// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UploadFileToFormazionePathRequest = Pi3.App.Legacy.WebApi.Application.Requests.UploadFileToFormazionePath;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UploadFileToFormazionePath
{
    public class UploadFileToFormazionePathHandler : IRequestHandler<UploadFileToFormazionePathRequest, UploadFileToFormazionePathResult>
    {
        #region Public Members

        public UploadFileToFormazionePathHandler(IPi3DbContext dbContext, IConfigurationService configurationService, ILogger<UploadFileToFormazionePathHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._configurationService = configurationService;
        }

        public async Task<UploadFileToFormazionePathResult> Handle(UploadFileToFormazionePathRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.UploadFile(request.fileStream,request.fileName,request.idUO,request.infoUtente,request.isAllegato);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadFileToFormazionePathHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IPi3DbContext _dbContext;

        private async Task<bool> UploadFile(byte[] fileStream, string fileName, string idUo, InfoUtente infoUtente, bool isAllegato)
        {
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
            string codiceAmm, codiceUo = string.Empty;
            bool result = true;

            try
            {
                if (!(fileStream != null && fileStream.Length > 0)) { throw new ArgumentNullException(Resources.EmptyStream); }

                codiceAmm = await this.GetCodAmmById(infoUtente.idAmministrazione);
                codiceUo = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(idUo))).output.codiceRubrica;

                if (string.IsNullOrEmpty(codiceAmm) || string.IsNullOrEmpty(codiceUo))
                    return false;

                var pathDirImportDoc = Path.Combine(
                repositoryRootPath,
                "Formazione",
                codiceAmm.ToUpper(),
                codiceUo, "File").PathAsUnixPath();


                if (!System.IO.Directory.Exists(pathDirImportDoc))
                    System.IO.Directory.CreateDirectory(pathDirImportDoc);

                if (isAllegato)
                {
                    pathDirImportDoc = System.IO.Path.Combine(pathDirImportDoc, "Allegati").PathAsUnixPath();
                    if (!System.IO.Directory.Exists(pathDirImportDoc))
                    {
                        System.IO.Directory.CreateDirectory(pathDirImportDoc);
                    }
                }
                string filePath = System.IO.Path.Combine(pathDirImportDoc, fileName).PathAsUnixPath();

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                System.IO.File.WriteAllBytes(filePath, fileStream);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                result = false;
            }

            return result;
        }


        private async Task<string> GetCodAmmById(string idAmm)
        {
            var codAmm = await this._dbContext.AmministraEntities.AsNoTracking().Where(a => a.SYSTEM_ID == idAmm.AsLong()).Select(a => a.VAR_CODICE_AMM).FirstOrDefaultAsync();

            return codAmm;
        }


        #endregion
    }
}