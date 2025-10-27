// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Crea;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Aggregazioni.Aggiungi;

public class AggiungiAggregazioneCommandHandler : IRequestHandler<AggiungiAggregazioneCommand, AggiungiAggregazioneCommandResponse>
{
    #region Public Members
    public AggiungiAggregazioneCommandHandler(
        ILogger<AggiungiAggregazioneCommandHandler> logger,
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

    public async Task<AggiungiAggregazioneCommandResponse> Handle(AggiungiAggregazioneCommand request, CancellationToken cancellationToken)
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

        if (request?.Aggregazione?.Tipo == TipiAggregazioneEnum.Fascicolo)
            aggregate.AddAggFascicolo(request?.Aggregazione?.Id);

        if (request?.Aggregazione?.Tipo == TipiAggregazioneEnum.SerieDocumentale)
            aggregate.AddAggSerieDocumentale(request?.Aggregazione?.Id);

        if (request?.Aggregazione?.Tipo == TipiAggregazioneEnum.SerieDiFascicoli)
            aggregate.AddAggSerieDiFascicoli(request?.Aggregazione?.Id);


        await this._repository.Update(aggregate);

        return new AggiungiAggregazioneCommandResponse
        {
            DocumentoAmministrativo = this._mapper.Map<Documento>(aggregate)
        };
    }

    #endregion

    #region Private Members
    private readonly ILogger<AggiungiAggregazioneCommandHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IDocumentoAmministrativoRepository _repository;
    private readonly IPi3DbContext _context;

    protected IMapper _mapper = null;

    protected virtual void InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddDocumentoMapping();
        });
        _mapper = configuration.CreateMapper();
    }

    #endregion
}
