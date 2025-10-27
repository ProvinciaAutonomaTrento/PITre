// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Documenti.AddCollegato;

public class AddDocumentoCollegatoCommandHandler : IRequestHandler<AddDocumentoCollegatoCommand, AddDocumentoCollegatoCommandResponse>
{
    private readonly IPi3DbContext _context;
    private readonly IDocumentoAmministrativoRepository _repository;
    private readonly IClaimsPrincipalService _claimsPrincipalService;

    public AddDocumentoCollegatoCommandHandler(IPi3DbContext context,
        IDocumentoAmministrativoRepository repository,
        IClaimsPrincipalService claimsPrincipalService)
    {
        _context = context;
        _repository = repository;
        _claimsPrincipalService = claimsPrincipalService;

        InitializeMapper();
    }

    public async Task<AddDocumentoCollegatoCommandResponse> Handle(AddDocumentoCollegatoCommand request, CancellationToken cancellationToken)
    {
        var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true)!;

        var aggregate = await _repository.Get(idTenant, request.Id,
            new ILoadBehavior[1]
            {
                new GetDocumentoAmministrativoLoadBehavior()
                {
                    LoadProfiles = true,
                    LoadClassifications = true,
                    LoadAllegati = true,
                    LoadAggregazioni = true,
                    LoadVersions = true,
                    LoadPermissions = true,
                    LoadMittentiDestinatari = true
                }
            });

        aggregate.AddRelatedElement(request.IdDocumento, request.IsParent);

        await _repository.Update(aggregate);

        return new AddDocumentoCollegatoCommandResponse
        {
            DocumentoAmministrativo = _mapper.Map<Documento>(aggregate)
        };
    }

    protected IMapper _mapper = null;

    protected virtual void InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddDocumentoMapping();
        });
        _mapper = configuration.CreateMapper();

    }

}
