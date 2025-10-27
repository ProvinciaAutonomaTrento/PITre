// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRoles
{
    // Richiede libreria MediatR
    public class GetRolesCommandHandler : IRequestHandler<GetRolesCommand, GetRolesCommandResponse>
    {
        #region Public Members

        public GetRolesCommandHandler(ILogger<GetRolesCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator
            , IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetRolesCommandResponse> Handle(GetRolesCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetRoles - START");

            GetRolesCommandResponse response = new GetRolesCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

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
                    //DocsPaVO.utente.InfoUtente infoUtenteDaCercare = new DocsPaVO.utente.InfoUtente(utenteDaCercare, DBUtils.getRuoloPreferito(utenteDaCercare.idPeople,_pi3DbContext));
                    arrayRuoli = DBUtils.getRuoliUtente(utente.systemId,_pi3DbContext);
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

                _logger.LogInformation("end GetRoles");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetRoles: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRolesCommandResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetRoles");
                response = new GetRolesCommandResponse();
                response.Code = GetRolesResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRolesCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
