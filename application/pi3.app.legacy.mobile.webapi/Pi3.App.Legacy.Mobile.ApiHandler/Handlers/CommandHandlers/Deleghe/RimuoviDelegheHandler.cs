// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.Legacy.Mobile.Data.Services.Interfaces;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Handlers.CommandHandlers.Deleghe;
public class RimuoviDelegheHandler(
    IDelegheService delegheService,
    IMapper mapper
    ) : IRequestHandler<COMMAND.RevocaDelegheRequest, RESULT.Deleghe.RevocaDelegheResult>
{
    private readonly IDelegheService _delegheService = delegheService;
    private readonly IMapper _mapper = mapper;

    public async Task<RESULT.Deleghe.RevocaDelegheResult> Handle( COMMAND.RevocaDelegheRequest request, CancellationToken cancellationToken )
    {
        IEnumerable<SERVICE_DTO.Delega> deleghe = this._mapper.Map<IEnumerable<SERVICE_DTO.Delega>>(request.Deleghe);
        bool serviceResult = await this._delegheService.RimuoviDeleghe(deleghe);

        return new RESULT.Deleghe.RevocaDelegheResult() { Success = serviceResult };
    }
}
