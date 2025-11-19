// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.SignBook.GetSignProcessInstance
{
    // Richiede libreria MediatR
    public class GetSignProcessInstanceCommandHandler : IRequestHandler<GetSignProcessInstanceCommand, GetSignProcessInstanceCommandResponse>
    {
        #region Public Members

        public GetSignProcessInstanceCommandHandler(ILogger<GetSignProcessInstanceCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetSignProcessInstanceCommandResponse> Handle(GetSignProcessInstanceCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetSignProcessInstance - START");

            GetSignProcessInstanceCommandResponse response = new GetSignProcessInstanceCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                #region implementazione
                if (request == null || string.IsNullOrWhiteSpace(request.idProcessInstance)) throw new RestException("REQUIRED_ID");

                long idInst = 0;
                try
                {
                    idInst = request.idProcessInstance.AsLong();
                }
                catch
                {
                    idInst = 0;
                }
                var istanza = await SignBookUtils.GetIstanzaProcessoFirma(idInst, _pi3DbContext);
                if (istanza == null) throw new RestException("SIGN_PROCESS_NOT_FOUND");

                response.ProcessInstance = new SignatureProcessInstance(istanza);

				#endregion
                
                response.Code = GetSignProcessInstanceResponseCode.OK;

                _logger.LogInformation("end GetSignProcessInstance");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetSignProcessInstance: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetSignProcessInstanceCommandResponse();
                response.Code = GetSignProcessInstanceResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetSignProcessInstance");
                response = new GetSignProcessInstanceCommandResponse();
                response.Code = GetSignProcessInstanceResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetSignProcessInstanceCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}