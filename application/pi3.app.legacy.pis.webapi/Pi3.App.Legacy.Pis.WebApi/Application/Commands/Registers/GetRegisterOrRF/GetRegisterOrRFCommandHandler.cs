// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegisterOrRF
{
    // Richiede libreria MediatR
    public class GetRegisterOrRFCommandHandler : IRequestHandler<GetRegisterOrRFCommand, GetRegisterOrRFCommandResponse>
    {
        #region Public Members

        public GetRegisterOrRFCommandHandler(ILogger<GetRegisterOrRFCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = dbContext;
        }

        public async Task<GetRegisterOrRFCommandResponse> Handle(GetRegisterOrRFCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetRegisterOrRFCommand - START");

            GetRegisterOrRFCommandResponse response = new GetRegisterOrRFCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                if (string.IsNullOrEmpty(request.codeRegister) && string.IsNullOrEmpty(request.idRegister))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_REGISTER");
                }

                if (!string.IsNullOrEmpty(request.codeRegister) && !string.IsNullOrEmpty(request.idRegister))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_REGISTER");
                }


                Register registerResponse = new Register();

                DocsPaVO.utente.Registro registro = null;

                if (!string.IsNullOrEmpty(request.codeRegister))
                {
                    registro = DBUtils.getRegistroByCodAOO(request.codeRegister, infoUtente.idAmministrazione,_pi3DbContext);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.idRegister))
                    {
                        try
                        {
                            registro = DBUtils.getRegistro(request.idRegister,_pi3DbContext);
                        }
                        catch
                        {
                            //Registro non trovato
                            throw new RestException("REGISTER_NOT_FOUND");
                        }
                    }
                }

                if (registro != null)
                {
                    registerResponse = RestUtils.GetRegister(registro);
                }
                else
                {
                    //Registro non trovato
                    throw new RestException("REGISTER_NOT_FOUND");
                }

                response.Register = registerResponse;

                response.Code = GetRegisterOrRFResponseCode.OK;

                _logger.LogInformation("end GetRegisterOrRF");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetRegisterOrRF: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRegisterOrRFCommandResponse();
                response.Code = GetRegisterOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetRegisterOrRF");
                response = new GetRegisterOrRFCommandResponse();
                response.Code = GetRegisterOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRegisterOrRFCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
