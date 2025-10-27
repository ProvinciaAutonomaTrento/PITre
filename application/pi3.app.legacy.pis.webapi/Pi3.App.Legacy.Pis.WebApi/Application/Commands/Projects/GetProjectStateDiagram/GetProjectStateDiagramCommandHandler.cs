// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectStateDiagram
{
    // Richiede libreria MediatR
    public class GetProjectStateDiagramCommandHandler : IRequestHandler<GetProjectStateDiagramCommand, GetProjectStateDiagramCommandResponse>
    {
        #region Public Members

        public GetProjectStateDiagramCommandHandler(ILogger<GetProjectStateDiagramCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetProjectStateDiagramCommandResponse> Handle(GetProjectStateDiagramCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetProjectStateDiagram - START");

            GetProjectStateDiagramCommandResponse response = new GetProjectStateDiagramCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta
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

                try { await _pi3DbContext.AssertSecurityRights(idfascicolo, infoUtente.idPeople, infoUtente.idGruppo); } catch (Exception ex) { throw new RestException("PROJECT_NOT_FOUND"); }

                StateOfDiagram stateOfDiagramResponse = new StateOfDiagram();
                stateOfDiagramResponse = DBUtils.getStatoDiagrammaFascicolo(idfascicolo, _pi3DbContext);

                if (stateOfDiagramResponse != null)
                {
                    response.StateOfDiagram = stateOfDiagramResponse;
                }
                else
                {
                    throw new RestException("FASC_STATEOFDIAGRAM_NOT_FOUND");
                }

                #endregion

                response.Code = GetStateOfDiagramResponseCode.OK;

                _logger.LogInformation("end GetProjectStateDiagram");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetProjectStateDiagram: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectStateDiagramCommandResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetProjectStateDiagram");
                response = new GetProjectStateDiagramCommandResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectStateDiagramCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}