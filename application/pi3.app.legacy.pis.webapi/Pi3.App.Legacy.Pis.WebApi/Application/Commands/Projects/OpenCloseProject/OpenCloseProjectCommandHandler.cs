// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.OpenCloseProject
{
    // Richiede libreria MediatR
    public class OpenCloseProjectCommandHandler : IRequestHandler<OpenCloseProjectCommand, OpenCloseProjectCommandResponse>
    {
        #region Public Members

        public OpenCloseProjectCommandHandler(ILogger<OpenCloseProjectCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext, IWebMethodLoggerService loggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
            this._loggerService = loggerService;
        }

        public async Task<OpenCloseProjectCommandResponse> Handle(OpenCloseProjectCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("OpenCloseProject - START");

            OpenCloseProjectCommandResponse response = new OpenCloseProjectCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controllo parametri richiesta

                //Controllo validit� della richiesta
                if (string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject) && string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_CODE_OR_IDPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) || !string.IsNullOrEmpty(request.CodeProject)) && !string.IsNullOrEmpty(request.IdProject))
                {
                    throw new RestException("REQUIRED_ONLY_IDPROJECT_OR_CODEPROJECT");
                }

                if ((!string.IsNullOrEmpty(request.ClassificationSchemeId) && string.IsNullOrEmpty(request.CodeProject)) || (string.IsNullOrEmpty(request.ClassificationSchemeId) && !string.IsNullOrEmpty(request.CodeProject)))
                {
                    throw new RestException("REQUIRED_CODEPROJECT_AND_CLASSIFICATION");
                }

                if (String.IsNullOrEmpty(request.Action))
                {
                    throw new RestException("REQUIRED_ACTION");
                }else if(request.Action.ToUpper() != "O" && request.Action.ToUpper() != "C")
                {
                    throw new RestException("INVALID_PARAMETER");
                }    



                #endregion

                #region implementazione
                string idfascicolo = "";
                if (!string.IsNullOrWhiteSpace(request.CodeProject))
                {
                    idfascicolo = (from a in _pi3DbContext.ProjectEntities where a.VAR_CODICE.ToUpper() == request.CodeProject.ToUpper() && a.ID_TITOLARIO.ToString() == request.ClassificationSchemeId select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                else
                {
                    idfascicolo = request.IdProject;
                }

                if (string.IsNullOrWhiteSpace(idfascicolo) || idfascicolo == "0") throw new RestException("PROJECT_NOT_FOUND");
                try { await _pi3DbContext.AssertSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo, SecurityRightTypesEnum.Write); } catch (Exception ex) { throw new RestException("PROJECT_NOT_FOUND"); }

                try
                {
                    response.Project = DBUtils.getProjectFromDB(idfascicolo, _pi3DbContext);
                }
                catch (Exception ex) { throw new RestException("PROJECT_NOT_FOUND"); }

                if (response.Project != null)
                {
                    if (response.Project.Open && request.Action.ToUpper().Equals("O"))
                    {
                        throw new RestException("PROJECT_ALREADY_OPEN");
                    }

                    if (!response.Project.Open && request.Action.ToUpper().Equals("C"))
                    {
                        throw new RestException("PROJECT_ALREADY_CLOSE");
                    }
                    if (request.Action.ToUpper().Equals("O"))
                    {
                        var aperto = DBUtils.OpenProject(response.Project.Id, _pi3DbContext);
                        if (aperto)
                        {
                            response.Project.Open = true;
                            response.Project.OpeningDate = DateTime.Now.ToString("dd/MM/yyyy");
                            response.Project.ClosureDate = null;
                            await _loggerService.LogOK("FASCICOLOMODIFICA",
                        response.Project.Id, $"PIS REST: Apertura Fascicolo {response.Project.Code}",
                        null, infoUtente.codWorkingApplication);
                        }
                        else { throw new RestException("APPLICATION_ERROR"); }
                    }
                    else if (request.Action.ToUpper().Equals("C"))
                    {
                        var chiuso = DBUtils.CloseProject(response.Project.Id, infoUtente, _pi3DbContext);
                        if (chiuso)
                        {
                            response.Project.Open = false;
                            response.Project.ClosureDate = DateTime.Now.ToString("dd/MM/yyyy");
                            await _loggerService.LogOK("FASCICOLOMODIFICA",
                        response.Project.Id, $"PIS REST: Chiusura Fascicolo {response.Project.Code}",
                        null, infoUtente.codWorkingApplication);
                        }
                        else { throw new RestException("APPLICATION_ERROR"); }
                    }
                }
                else throw new RestException("PROJECT_NOT_FOUND");

                #endregion

                response.Code = GetProjectResponseCode.OK;

                _logger.LogInformation("end OpenCloseProject");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione OpenCloseProject: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new OpenCloseProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione OpenCloseProject");
                response = new OpenCloseProjectCommandResponse();
                response.Code = GetProjectResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<OpenCloseProjectCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;
        protected readonly IWebMethodLoggerService _loggerService;

        #endregion
    }

}