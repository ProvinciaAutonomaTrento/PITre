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

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetUsersInRole
{
    // Richiede libreria MediatR
    public class GetUsersInRoleCommandHandler : IRequestHandler<GetUsersInRoleCommand, GetUsersInRoleCommandResponse>
    {
        #region Public Members

        public GetUsersInRoleCommandHandler(ILogger<GetUsersInRoleCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator
            , IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetUsersInRoleCommandResponse> Handle(GetUsersInRoleCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetUsersInRole - START");

            GetUsersInRoleCommandResponse response = new GetUsersInRoleCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                if (string.IsNullOrEmpty(request.codeRole))
                {
                    throw new RestException("REQUIRED_CODE_ROLE");
                }

                User[] userResponse = null;

                ruolo = null;

                if (!string.IsNullOrEmpty(request.codeRole))
                {
                    ruolo = DBUtils.getRuoloByCodice(request.codeRole,_pi3DbContext);
                    if (ruolo != null)
                    {
                        List<DocsPaVO.utente.UserMinimalInfo> userList = DBUtils.GetUsersInRoleMinimalInfo(ruolo.idGruppo,_pi3DbContext);
                        if (userList != null && userList.Count > 0)
                        {
                            int i = 0;
                            userResponse = new User[userList.Count];
                            foreach (DocsPaVO.utente.UserMinimalInfo minimalUser in userList)
                            {
                                User tempUser = new User();
                                DocsPaVO.utente.Utente utenteU = DBUtils.getUtenteById(minimalUser.SystemId,_pi3DbContext);
                                tempUser.Description = utenteU.descrizione;
                                tempUser.Name = utenteU.nome;
                                tempUser.Surname = utenteU.cognome;
                                tempUser.UserId = utenteU.userId;
                                tempUser.Id = utenteU.idPeople;
                                userResponse[i] = tempUser;
                                i++;
                            }
                        }
                    }
                    else
                    {
                        //Ruolo non trovato
                        throw new RestException("ROLE_NO_EXIST");
                    }
                }
                else
                {
                    throw new RestException("REQUIRED_CODE_ROLE");
                }

                response.Users = userResponse;

                response.Code = GetUsersResponseCode.OK;

                _logger.LogInformation("end GetUsersInRole");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetUsersInRole: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetUsersInRoleCommandResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetUsersInRole");
                response = new GetUsersInRoleCommandResponse();
                response.Code = GetUsersResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetUsersInRoleCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
