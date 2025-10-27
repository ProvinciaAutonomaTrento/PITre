// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Projects.GetProjectsByDocument
{
    // Richiede libreria MediatR
    public class GetProjectsByDocumentCommandHandler : IRequestHandler<GetProjectsByDocumentCommand, GetProjectsByDocumentCommandResponse>
    {
        #region Public Members

        public GetProjectsByDocumentCommandHandler(ILogger<GetProjectsByDocumentCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetProjectsByDocumentCommandResponse> Handle(GetProjectsByDocumentCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetProjectsByDocument - START");

            GetProjectsByDocumentCommandResponse response = new GetProjectsByDocumentCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region controlli preliminari su richiesta

                if (string.IsNullOrEmpty(request.idDocument) && string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                #endregion

                #region implementazione
                string idDocument = "";
                if (string.IsNullOrEmpty(request.idDocument))
                {
                    idDocument = (from a in _pi3DbContext.ProfileEntities where a.VAR_SEGNATURA != null && a.VAR_SEGNATURA.ToUpper() == request.signature.ToUpper() select a.SYSTEM_ID).FirstOrDefault().ToString();
                }
                else
                {
                    idDocument = request.idDocument;
                }

                if (string.IsNullOrWhiteSpace(idDocument) || idDocument == "0") throw new RestException("DOCUMENT_NOT_FOUND");
                // verificare controllo security su documento allegato
                try
                {
                    await _pi3DbContext.AssertSecurityRights(idDocument, infoUtente.idPeople, infoUtente.idGruppo);
                }
                catch (Exception security) { throw new RestException("DOCUMENT_NOT_FOUND"); }

                var idFascicoli = (from a in _pi3DbContext.ProjectEntities
                                   join b in _pi3DbContext.ProjectComponentEntities on a.SYSTEM_ID equals b.PROJECT_ID
                                   where b.LINK == idDocument.AsLong()
                                   orderby a.ID_FASCICOLO
                                   select a.ID_FASCICOLO).Distinct();
                if (idFascicoli == null || !idFascicoli.Any()) throw new RestException("PROJECTS_NOT_FOUND");
                List<Project> fascicoli = new List<Project>();
                foreach(var idFasc in idFascicoli)
                {
                    fascicoli.Add(DBUtils.getProjectFromDB(idFasc.ToString(), _pi3DbContext));
                }
                response.TotalProjectsNumber = fascicoli.Count;
                response.Projects = fascicoli.ToArray();

                #endregion

                response.Code = SearchProjectsResponseCode.OK;

                _logger.LogInformation("end GetProjectsByDocument");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetProjectsByDocument: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetProjectsByDocumentCommandResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetProjectsByDocument");
                response = new GetProjectsByDocumentCommandResponse();
                response.Code = SearchProjectsResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetProjectsByDocumentCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}