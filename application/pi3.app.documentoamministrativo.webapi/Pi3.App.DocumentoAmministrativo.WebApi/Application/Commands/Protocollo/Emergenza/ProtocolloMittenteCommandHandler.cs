// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Protocollo.Emergenza;

public class ProtocolloEmergenzaCommandHandler : IRequestHandler<ProtocolloEmergenzaCommand, ProtocolloEmergenzaCommandResponse>
{
    #region Public Members
    public ProtocolloEmergenzaCommandHandler(
        ILogger<ProtocolloEmergenzaCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService,
        IDocumentoAmministrativoRepository repository,
        IPi3DbContext context)
    {
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._repository = repository;
        this._context = context;


        this.InitializeMapper();
    }

    public async Task<ProtocolloEmergenzaCommandResponse> Handle(ProtocolloEmergenzaCommand request, CancellationToken cancellationToken)
    {
        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        var aggregate = await this._repository.Get(idTenant, request.Id,
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

        if (request.Protocollo != null)
            aggregate.AssignProtocolloEmergenza(_mapper.Map<Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>(request.Protocollo));

        await this._repository.Update(aggregate);

        return new ProtocolloEmergenzaCommandResponse
        {
            DocumentoAmministrativo = this._mapper.Map<Documento>(aggregate)
        };
    }

    #endregion

    #region Private Members
    private readonly ILogger<ProtocolloEmergenzaCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IDocumentoAmministrativoRepository _repository;
    private readonly IPi3DbContext _context;

    protected IMapper _mapper = null;

    protected virtual void InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddDocumentoMapping();
            cfg.CreateMap<ProtocolloEmergenza, Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects.ProtocolloEmergenza>();
        });
        _mapper = configuration.CreateMapper();

    }

    #endregion
}
