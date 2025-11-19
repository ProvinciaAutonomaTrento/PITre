// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.ClassificationSchemes;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectFolders
{
    // Richiede libreria MediatR
    public class GetProjectFoldersCommandHandler : IRequestHandler<GetProjectFoldersCommand, GetProjectFoldersCommandResponse>
    {
        #region Public Members

        public GetProjectFoldersCommandHandler(ILogger<GetProjectFoldersCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetProjectFoldersCommandResponse> Handle(GetProjectFoldersCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetProjectFolders - START");

            GetProjectFoldersCommandResponse response = new GetProjectFoldersCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controllo parametri richiesta
                if (string.IsNullOrEmpty(request.classificationSchemeId) && string.IsNullOrEmpty(request.codeProject) && string.IsNullOrEmpty(request.idProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.classificationSchemeId) || !string.IsNullOrEmpty(request.codeProject)) && !string.IsNullOrEmpty(request.idProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.classificationSchemeId) && string.IsNullOrEmpty(request.codeProject)) || (string.IsNullOrEmpty(request.classificationSchemeId) && !string.IsNullOrEmpty(request.codeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }
                #endregion
                #region implementazione
                string idfascicolo = "";
                if (!string.IsNullOrWhiteSpace(request.codeProject))
                {
                    idfascicolo = (from a in _pi3DbContext.ProjectEntities where a.VAR_CODICE.ToUpper() == request.codeProject.ToUpper() && a.ID_TITOLARIO.ToString() == request.classificationSchemeId select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                else
                {
                    idfascicolo = request.idProject;
                }

                if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");
                
                try { await _pi3DbContext.AssertSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo); } catch (Exception ex) { throw new RestException("PROJECT_NOT_FOUND"); }
                
                var project = DBUtils.getProjectLiteById(idfascicolo, _pi3DbContext);

                if (project != null) { 
                    response.ProjectDescription = project.Description;
                    response.IdProject = project.Id;
                    var folders = DBUtils.getProjectFolders(project.Id, _pi3DbContext);
                    response.Folders = folders.ToArray();
                }
                                
				#endregion
                
                response.Code = GetProjectFoldersResponseCode.OK;

                _logger.LogInformation("end GetProjectFolders");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetProjectFolders: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectFoldersCommandResponse();
                response.Code = GetProjectFoldersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetProjectFolders");
                response = new GetProjectFoldersCommandResponse();
                response.Code = GetProjectFoldersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectFoldersCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}