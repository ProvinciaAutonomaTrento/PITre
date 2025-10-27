// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetRole
{
    // Richiede libreria MediatR
    public class GetRoleCommandHandler : IRequestHandler<GetRoleCommand, GetRoleCommandResponse>
    {
        #region Public Members

        public GetRoleCommandHandler(ILogger<GetRoleCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IHttpContextAccessor httpContextAccessor, IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor = httpContextAccessor;
            this._pi3DbContext = pi3DbContext;
        }

        public async Task<GetRoleCommandResponse> Handle(GetRoleCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetRole - START");

            GetRoleCommandResponse response = new GetRoleCommandResponse();
            try
            {
                #region token e autenticazione
                DocsPaVO.utente.Utente utente = null;
                DocsPaVO.utente.Ruolo ruolo = new DocsPaVO.utente.Ruolo();
                DocsPaVO.utente.InfoUtente infoUtente = RestUtils.ControlloToken(_httpContextAccessor, _pi3DbContext, out utente, out ruolo);
                #endregion

                if (string.IsNullOrEmpty(request.codeRole) && string.IsNullOrEmpty(request.idRole))
                {
                    throw new RestException("REQUIRED_CODE_OR_ID_ROLE");
                }

                if (!string.IsNullOrEmpty(request.codeRole) && !string.IsNullOrEmpty(request.idRole))
                {
                    throw new RestException("REQUIRED_ONLY_CODE_OR_ID_ROLE");
                }

                Role roleResponse = new Role();
                Register[] registerResponse = null;

                ruolo = null;
                ArrayList registri = new ArrayList();
                if (!string.IsNullOrEmpty(request.codeRole))
                {
                    ruolo = DBUtils.getRuoloByCodice(request.codeRole,_pi3DbContext);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.idRole))
                    {
                        //Id gruppo
                        ruolo = DBUtils.getRuoloByIdGruppo(request.idRole, _pi3DbContext);
                    }
                }

                if (ruolo != null)
                {
                    registri = DBUtils.getListaRegistriRfRuolo(ruolo.systemId, string.Empty, _pi3DbContext);
                    roleResponse.Code = ruolo.codiceRubrica;
                    roleResponse.Description = ruolo.descrizione;
                    roleResponse.Id = ruolo.idGruppo;
                    if (registri != null && registri.Count > 0)
                    {
                        registerResponse = new Register[registri.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registri)
                        {
                            Register regTemp = new Register();
                            regTemp.Code = reg.codRegistro;
                            regTemp.Description = reg.descrizione;
                            regTemp.Id = reg.systemId;
                            if (!string.IsNullOrEmpty(reg.chaRF) && (reg.chaRF).Equals("1"))
                            {
                                regTemp.IsRF = true;
                            }
                            else
                            {
                                regTemp.IsRF = false;
                            }
                            if (!string.IsNullOrEmpty(reg.stato))
                            {
                                if (reg.stato.Equals("A"))
                                {
                                    regTemp.State = "Open";
                                }
                                else
                                {
                                    regTemp.State = "Closed";
                                }
                            }
                            registerResponse[i] = regTemp;
                            i++;
                        }
                    }
                }
                else
                {
                    //Ruolo non trovato
                    throw new RestException("ROLE_NO_EXIST");
                }

                response.Role = roleResponse;
                response.Role.Registers = registerResponse;

                response.Code = GetRoleResponseCode.OK;

                _logger.LogInformation("end GetRole");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetRole: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRoleCommandResponse();
                response.Code = GetRoleResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetRole");
                response = new GetRoleCommandResponse();
                response.Code = GetRoleResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;


        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRoleCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
