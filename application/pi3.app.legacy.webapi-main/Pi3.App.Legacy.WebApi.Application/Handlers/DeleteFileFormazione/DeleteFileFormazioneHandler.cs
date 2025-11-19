// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeleteFileFormazioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.DeleteFileFormazione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DeleteFileFormazione
{
    public class DeleteFileFormazioneHandler : IRequestHandler<DeleteFileFormazioneRequest, DeleteFileFormazioneResult>
    {
        #region Public Members

        

        public async Task<DeleteFileFormazioneResult> Handle(DeleteFileFormazioneRequest request, CancellationToken cancellationToken)
        {
            ///da completare l'implementazione
            DeleteFileFormazioneResult result = new(true);
            try
            {
                var repositoryRootPath = await this._configurationService.GetValue<string>("REPOSITORY_ROOT_PATH", true);
                string codiceAmm = request.infoUtente.idAmministrazione;
                string codiceUO = request.idUO;

                string docRootPath = Path.Combine(string.Empty, "Formazione");
                docRootPath = Path.Combine(docRootPath, codiceAmm.ToUpper());
                docRootPath = Path.Combine(docRootPath, codiceUO);

                if (Directory.Exists(docRootPath))
                {
                    Directory.Delete(docRootPath, true);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex, null);
                //return false;
            }

            throw new NotImplementedException("DeleteFileFormazione");
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DeleteFileFormazioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IConfigurationService _configurationService;

        public DeleteFileFormazioneHandler(ILogger<DeleteFileFormazioneHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator,
            IConfigurationService configurationService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            _configurationService = configurationService;
        }

        #endregion
    }
}