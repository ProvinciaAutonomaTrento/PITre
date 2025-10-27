// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;

using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Authentication;
public class AuthenticateUserHandler(
    ILogger<AuthenticateUserHandler> logger,
    IMapper mapper,
    IAuthenticationService authenticationService,
    IPeopleService peopleService,
    IRoleService roleService) : IRequestHandler<QUERY.AuthenticateUserQuery, DTO.Authentication.AuthenticateUserDTO>
{
    readonly ILogger<AuthenticateUserHandler> _logger = logger;
    readonly IMapper _mapper = mapper;
    readonly IAuthenticationService _authenticationService = authenticationService;
    readonly IPeopleService _peopleService = peopleService;
    readonly IRoleService _roleService = roleService;

    public async Task<DTO.Authentication.AuthenticateUserDTO> Handle( QUERY.AuthenticateUserQuery request, CancellationToken cancellationToken )
    {
        if(String.IsNullOrEmpty(request.Username))
        {
            this._logger.LogError("Parametro richiesto assente: Username");
            throw new EXCEPTIONS.RequestParamNotFoundException("Username");
        }

        if ( String.IsNullOrEmpty(request.Password) )
        {
            throw new EXCEPTIONS.RequestParamNotFoundException("Password");
        }


        // ToDo gestisci multi amm
        long? idAmministrazione;
        long tempIdAmministrazione = 0;
        if ( !String.IsNullOrWhiteSpace(request.IdAmministrazione) && !long.TryParse(request.IdAmministrazione, out tempIdAmministrazione) )
        {
            throw new EXCEPTIONS.RequestParamNotFoundException("IdAmministrazione");
        }
        else
        {
            idAmministrazione = tempIdAmministrazione > 0 ? tempIdAmministrazione : null;
        }

        SERVICE_REQUESTS.AuthenticateUserRequest serviceRequest = new()
        {
            Username = request.Username,
            Password = request.Password,
            IdAmministrazione = idAmministrazione
        };

        SERVICE_DTO.AuthenticationResult authenticationResult 
            = await this._authenticationService.AuthenticateUser(serviceRequest, cancellationToken);


        DTO.Authentication.AuthenticateUserDTO result = this._mapper.Map<DTO.Authentication.AuthenticateUserDTO>(authenticationResult);
        if ( authenticationResult.IdPeople.HasValue )
        {
            IEnumerable<SERVICE_DTO.GetRuoliUtenteResult> ruoliUtente
                = await this._peopleService.GetRuoliUtenteAsync(authenticationResult.IdPeople.Value, cancellationToken)
                    ?? throw new EXCEPTIONS.ExpectedResultNotFoundException("Ruoli utente non recuperati");

            result.UserInfo!.Ruoli = this._mapper.Map<IEnumerable<DTO.Roles.RoleDetails>>(ruoliUtente);

            DTO.Roles.RoleDetails? ruoloPredefinito = result.UserInfo.Ruoli.First();

            result.OTPAllowed = await this._roleService.CheckFunctionExistsForRoleByCode(ruoloPredefinito.Id!.Value, "TO_GET_OTP", cancellationToken);
            result.ShareAllowed = await this._roleService.CheckFunctionExistsForRoleByCode(ruoloPredefinito.Id!.Value, "DO_CONDIVIDI_MOBILE", cancellationToken);

            result.UserInfo.Token = Shared.Helpers.AuthenticationHelper.GeneraToken(
                ruoloPredefinito.Id!.Value,
                result.UserInfo.IdPeople,
                ruoloPredefinito.IdGruppo ?? 0L,
                result.UserInfo.Dst!,
                result.UserInfo.IdAmministrazione!.Value,
                result.UserInfo.UserId!,
                "",
                ""
                );
        }

        return result;
    }
}
