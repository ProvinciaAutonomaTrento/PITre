// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;

using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;


namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.QueryHandlers.Users;

public class GetUserHandler(
    ILogger<GetUserHandler> logger,
    IMapper mapper,
    IPeopleService peopleService
    ) : IRequestHandler<QUERY.GetUserQuery, DTO.Users.GetUserDto>
{
    private readonly ILogger<GetUserHandler> _logger = logger;
    private readonly IPeopleService _peopleService = peopleService;
    private readonly IMapper _mapper = mapper;

    public async Task<DTO.Users.GetUserDto> Handle( QUERY.GetUserQuery request, CancellationToken cancellationToken )
    {
        this._logger.LogInformation("Handle GetUser");

        if ( String.IsNullOrWhiteSpace(request.Username) )
        {
            throw new EXCEPTIONS.RequestParamNotFoundException(nameof(request.Username));
        }

        SERVICE_REQUESTS.GetUserClaimsRequest serviceRequest = new()
        {
            IdAmministrazione = request.IdAmministrazione,
            Username = request.Username,
            GroupId = request.IdGroup
        };

        SERVICE_DTO.UserClaims? userWithClaimsInfo = await this._peopleService.GetUserClaims(serviceRequest, cancellationToken) 
            ?? throw new EXCEPTIONS.UserNotFoundException(request.Username);

        DTO.Users.GetUserDto? utente = this._mapper.Map<DTO.Users.GetUserDto>(userWithClaimsInfo.UserDetails);
        utente.Functions = userWithClaimsInfo.Functions;

        return utente;
    }
}
