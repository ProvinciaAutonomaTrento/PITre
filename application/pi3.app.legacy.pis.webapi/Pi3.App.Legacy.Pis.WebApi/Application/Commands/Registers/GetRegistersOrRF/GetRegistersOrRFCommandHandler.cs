// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Pi3.App.Legacy.Pis.WebApi.Application.Commands.Utils;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System.Collections;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Registers.GetRegistersOrRF
{
    // Richiede libreria MediatR
    public class GetRegistersOrRFCommandHandler : IRequestHandler<GetRegistersOrRFCommand, GetRegistersOrRFCommandResponse>
    {
        #region Public Members

        public GetRegistersOrRFCommandHandler(ILogger<GetRegistersOrRFCommandHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._httpContextAccessor= httpContextAccessor;
            this._pi3DbContext = dbContext;
        }

        public async Task<GetRegistersOrRFCommandResponse> Handle(GetRegistersOrRFCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("GetRegistersOrRF - START");

            GetRegistersOrRFCommandResponse response = new GetRegistersOrRFCommandResponse();
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

                Register[] registerResponse = null;

                DocsPaVO.utente.Registro[] registri = null;

                DocsPaVO.utente.Ruolo ruoloX = null;
                ArrayList registriLista = new ArrayList();

                if (!string.IsNullOrEmpty(request.codeRole))
                {
                    ruoloX = DBUtils.getRuoloByCodice(request.codeRole, _pi3DbContext);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.idRole))
                    {
                        try
                        {
                            //Id gruppo
                            ruoloX = DBUtils.getRuoloByIdGruppo(request.idRole, _pi3DbContext);
                        }
                        catch
                        {
                            //Ruolo non trovato
                            throw new RestException("ROLE_NO_EXIST");
                        }
                    }
                    else ruoloX = ruolo;
                }

                if (ruoloX != null)
                {
                    //Prendi i registri e gli rf
                    if (string.IsNullOrWhiteSpace(request.RegOrRF) || (request.RegOrRF.ToUpper() != "RF" && request.RegOrRF.ToUpper() != "REG"))
                    {
                        registriLista = DBUtils.getListaRegistriRfRuolo(ruolo.systemId, string.Empty, _pi3DbContext);
                    }
                    else
                    {
                        //Prendi solo i registri
                        if (request.RegOrRF.ToUpper() == "REG")
                        {
                            registriLista = DBUtils.getListaRegistriRfRuolo(ruolo.systemId, "0", _pi3DbContext);
                        }
                        else
                        {
                            //Prendi solo gli RF
                            if (request.RegOrRF.ToUpper() == "RF")
                            {
                                registriLista = DBUtils.getListaRegistriRfRuolo(ruolo.systemId, "1", _pi3DbContext);
                            }
                        }
                    }
                    if (registriLista != null && registriLista.Count > 0)
                    {
                        registerResponse = new Register[registriLista.Count];
                        int i = 0;
                        foreach (DocsPaVO.utente.Registro reg in registriLista)
                        {
                            Register regTemp = new Register();
                            regTemp = RestUtils.GetRegister(reg);
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

                response.Registers = registerResponse;

                response.Code = GetRegistersOrRFResponseCode.OK;

                _logger.LogInformation("end GetRegistersOrRF");


            }
            catch (RestException pisEx)
            {
                _logger.LogError("Eccezione GetRegistersOrRF: {errorCode}, {errorDescription}", pisEx.ErrorCode, pisEx.Description);
                response = new GetRegistersOrRFCommandResponse();
                response.Code = GetRegistersOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = pisEx.Description;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "eccezione GetRegistersOrRF");
                response = new GetRegistersOrRFCommandResponse();
                response.Code = GetRegistersOrRFResponseCode.SYSTEM_ERROR;
                response.ErrorMessage = e.Message;

            }

            return response;

        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRegistersOrRFCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IPi3DbContext _pi3DbContext;

        #endregion
    }

}
