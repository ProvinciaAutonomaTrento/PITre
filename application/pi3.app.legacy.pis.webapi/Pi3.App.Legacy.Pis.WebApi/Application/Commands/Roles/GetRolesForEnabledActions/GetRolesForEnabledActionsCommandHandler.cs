// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;


namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRolesForEnabledActions
{
    // Richiede libreria MediatR
    public class GetRolesForEnabledActionsCommandHandler : IRequestHandler<GetRolesForEnabledActionsCommand, GetRolesForEnabledActionsCommandResponse>
    {
        #region Public Members

        public GetRolesForEnabledActionsCommandHandler(ILogger<GetRolesForEnabledActionsCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator
            , IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetRolesForEnabledActionsCommandResponse> Handle(GetRolesForEnabledActionsCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetRolesForEnabledActions - START");

            GetRolesForEnabledActionsCommandResponse response = new GetRolesForEnabledActionsCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                if (string.IsNullOrEmpty(request.codeFunction))
                {
                    throw new RestException("REQUIRED_CODTIPOFUNC");
                }

                if (string.IsNullOrEmpty(request.userId))
                {
                    throw new RestException("REQUIRED_USERID");
                }

                Role[] roleResponse = null;

                DocsPaVO.utente.Utente utenteDaCercare = null;
                ArrayList arrayRuoli = new ArrayList();

                if (!string.IsNullOrEmpty(request.userId))
                {
                    utenteDaCercare = DBUtils.getUtente(request.userId, infoUtente.idAmministrazione,_pi3DbContext);
                }
                else
                {
                    throw new RestException("REQUIRED_USERID");
                }

                if (utenteDaCercare != null)
                {
                    arrayRuoli = DBUtils.getRuoliUtenteForEnabledActions(utenteDaCercare.systemId, request.codeFunction, infoUtente.idAmministrazione,_pi3DbContext);
                    if (arrayRuoli != null && arrayRuoli.Count > 0)
                    {
                        roleResponse = new Role[arrayRuoli.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Ruolo rol in arrayRuoli)
                        {
                            Role roleTemp = new Role();
                            roleTemp.Code = rol.codiceRubrica;
                            roleTemp.Description = rol.descrizione;
                            roleTemp.Id = rol.idGruppo;
                            roleResponse[i] = roleTemp;
                            i++;
                        }
                    }
                    else
                    {
                        throw new RestException("USER_NO_ROLES");
                    }
                }
                else
                {
                    //Utente non trovato
                    throw new RestException("USER_NO_EXIST");
                }

                response.Roles = roleResponse;

                response.Code = GetRolesResponseCode.OK;

                _logger.LogInformation("end GetRolesForEnabledActions");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetRolesForEnabledActions: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRolesForEnabledActionsCommandResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetRolesForEnabledActions");
                response = new GetRolesForEnabledActionsCommandResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRolesForEnabledActionsCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
