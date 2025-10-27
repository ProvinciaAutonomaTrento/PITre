// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System.Collections;
using System.Security.Cryptography.Xml;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDocStateDiagram
{
    // Richiede libreria MediatR
    public class GetDocStateDiagramCommandHandler : IRequestHandler<GetDocStateDiagramCommand, GetDocStateDiagramCommandResponse>
    {
        #region Public Members

        public GetDocStateDiagramCommandHandler(ILogger<GetDocStateDiagramCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetDocStateDiagramCommandResponse> Handle(GetDocStateDiagramCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetDocStateDiagram - START");

            GetDocStateDiagramCommandResponse response = new GetDocStateDiagramCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                if (string.IsNullOrEmpty(request.idDocument) && string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ID_OR_SIGNATURE");
                }

                if (!string.IsNullOrEmpty(request.idDocument) && !string.IsNullOrEmpty(request.signature))
                {
                    throw new RestException("REQUIRED_ONLY_ID_OR_SIGNATURE");
                }

                StateOfDiagram stateOfDiagramResponse = new StateOfDiagram();
                string idDoc = request.idDocument;
                if (string.IsNullOrEmpty(request.idDocument) && string.IsNullOrEmpty(request.signature))
                {
                    idDoc= DBUtils.getDocIdFromProtocolSignature(request.signature, _pi3DbContext);
                }

                try
                {
                   await _pi3DbContext.AssertSecurityRights(idDoc, infoUtente.idPeople, infoUtente.idGruppo);
                   
                }
                catch (Exception ex)
                {
                    throw new RestException("DOCUMENT_NOT_FOUND");
                }

                stateOfDiagramResponse = DBUtils.getStatoDiagrammaDoc(idDoc, _pi3DbContext);

                if (stateOfDiagramResponse != null) 
                {
                    response.StateOfDiagram = stateOfDiagramResponse;
                }
                else
                {
                    throw new RestException("DOC_STATEOFDIAGRAM_NOT_FOUND");
                }
                #endregion

                response.Code = GetStateOfDiagramResponseCode.OK;

                _logger.LogInformation("end GetDocStateDiagram");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetDocStateDiagram: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetDocStateDiagramCommandResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetDocStateDiagram");
                response = new GetDocStateDiagramCommandResponse();
                response.Code = GetStateOfDiagramResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetDocStateDiagramCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}