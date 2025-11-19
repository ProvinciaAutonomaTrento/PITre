// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using UploadTemplateFormazionePathRequest = Pi3.App.Legacy.WebApi.Application.Requests.UploadTemplateFormazionePath;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UploadTemplateFormazionePath
{
    public class UploadTemplateFormazionePathHandler : IRequestHandler<UploadTemplateFormazionePathRequest, UploadTemplateFormazionePathResult>
    {
        #region Public Members

        public UploadTemplateFormazionePathHandler(IPi3DbContext dbContext, IConfigurationService configurationService, ILogger<UploadTemplateFormazionePathHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._configurationService = configurationService;
            this._dbContext = dbContext;
        }

        public async Task<UploadTemplateFormazionePathResult> Handle(UploadTemplateFormazionePathRequest request, CancellationToken cancellationToken)
        {
            bool output = false;
            try
            {
                output = await this.UploadTemplate(request.fileStream,request.idUO,request.infoUtente);
            }
            catch(Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }
            return new(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UploadTemplateFormazionePathHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;
        protected readonly IPi3DbContext _dbContext;

        private async Task<bool> UploadTemplate(byte[] fileStream, string idUo, InfoUtente infoUtente)
        {
            var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
            string codiceAmm, codiceUo = string.Empty;
            bool result = true;

            try
            {
                codiceAmm = await this.GetCodAmmById(infoUtente.idAmministrazione);
                codiceUo = (await this._mediator.Send(new Application.Requests.AddressbookGetCorrispondenteBySystemId(idUo))).output.codiceRubrica;

                if (string.IsNullOrEmpty(codiceAmm) || string.IsNullOrEmpty(codiceUo))
                    return false;

                var pathDirImportDoc = Path.Combine(
                repositoryRootPath,
                "Formazione",
                codiceAmm.ToUpper(),
                codiceUo).PathAsUnixPath();

                var pathImportDocumentiUo = Path.Combine(pathDirImportDoc,Resources.FileName).PathAsUnixPath();

                if (!System.IO.Directory.Exists(pathDirImportDoc))
                    System.IO.Directory.CreateDirectory(pathDirImportDoc);

                if(System.IO.File.Exists(pathImportDocumentiUo))
                    System.IO.File.Delete(pathImportDocumentiUo);

                System.IO.File.WriteAllBytes(pathImportDocumentiUo, fileStream);

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