// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Pi3.App.Legacy.Mobile.Data.Repositories.Interfaces;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;
using MODELS = Pi3.App.Legacy.Mobile.Models;
using SERVICE_REQUESTS = Pi3.App.Legacy.Mobile.Models.ServiceRequests;
using SELECT_TEMPLATES = Pi3.App.Legacy.Mobile.Models.SelectTemplates;
using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;

namespace Pi3.App.Legacy.Mobile.Data.Services;
public class PeopleService(
    IMapper mapper,
    IPeopleRepository peopleRepository,
    IRoleRepository roleRepository,
    IRegisterRepository registerRepository ) : IPeopleService
{
    readonly IMapper _mapper = mapper;
    readonly IPeopleRepository _peopleRepository = peopleRepository;
    readonly IRoleRepository _roleRepository = roleRepository;
    readonly IRegisterRepository _registerRepository = registerRepository;


    public async Task<SERVICE_DTO.UserClaims?> GetUserClaims( 
        SERVICE_REQUESTS.GetUserClaimsRequest request, 
        CancellationToken cancellationToken )
    {
        SERVICE_DTO.UserClaims user = new();

        SELECT_TEMPLATES.UserDetails? utenteDB = await this._peopleRepository.GetUserInformationForClaims(
            request.Username!, request.IdAmministrazione, request.GroupId, cancellationToken)
                ?? throw new EXCEPTIONS.UserNotFoundException(request.Username!);
        
        user.UserDetails = utenteDB;
        IEnumerable<string> functions = await this._peopleRepository.GetUserFunctions(utenteDB.GroupCorrGlobaliId, cancellationToken);
        user.Functions = functions;

        return user;
    }

    public void GetUserFunctions()
    {

    }

    public async Task<IEnumerable<SERVICE_DTO.GetRuoliUtenteResult>?> GetRuoliUtenteAsync(
        long id, 
        CancellationToken cancellationToken)
    {
        IEnumerable<SELECT_TEMPLATES.RuoloUtente> ruoliDB = await this._roleRepository.GetRoleByIdPeopleAsync(id, cancellationToken);

        IEnumerable<SERVICE_DTO.GetRuoliUtenteResult> ruoli = this._mapper.Map<IEnumerable<SERVICE_DTO.GetRuoliUtenteResult>>(ruoliDB);

        foreach ( var item in ruoli )
        {
            item.Registri = await this._registerRepository.GetRegistriIdByGroupIdAsync(item.Id, cancellationToken);
        }

        return ruoli;
    }

}
