// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.MittentiMultipli;

public class CambioMittentiMultipliCommandHandler : IRequestHandler<CambioMittentiMultipliCommand, CambioMittentiMultipliCommandResponse>
{
    #region Public Members
    public CambioMittentiMultipliCommandHandler(
        ILogger<CambioMittentiMultipliCommandHandler> logger,
        IClaimsPrincipalService claimsPrincipalService,
        IDocumentoAmministrativoRepository repository,
        IPi3DbContext context)
    {
        _logger = logger;
        _claimsPrincipalService = claimsPrincipalService;
        _repository = repository;
        _context = context;

        InitializeMapper();
    }

    public async Task<CambioMittentiMultipliCommandResponse> Handle(CambioMittentiMultipliCommand request, CancellationToken cancellationToken)
    {
        var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        //var aggregate = await _repository.Get(idTenant, request.Id,
        //    new ILoadBehavior[1]
        //    {
        //            new GetDocumentoAmministrativoLoadBehavior()
        //            {
        //                LoadProfiles = true,
        //                LoadClassifications = true,
        //                LoadAllegati = true,
        //                LoadAggregazioni = true,
        //                LoadVersions = true,
        //                LoadPermissions = true,
        //                LoadMittentiDestinatari = true
        //            }
        //    });

        var aggregate = await Helpers.GetFullDocument(_repository, idTenant, request.Id);

        var idMittente = (await _context.CorrGlobaliEntities.Where(corr => corr.VAR_CODICE.ToUpper() == request.Mittente.ToUpper() &&
            corr.DTA_FINE == null).FirstOrDefaultAsync(cancellationToken: cancellationToken)).SYSTEM_ID;
        var mittente = new Mittente(idMittente.ToString());

        aggregate.AddMittenteMultiplo(mittente);

        await _repository.Update(aggregate);

        return new CambioMittentiMultipliCommandResponse
        {
            DocumentoAmministrativo = _mapper.Map<Documento>(aggregate)
        };
    }

    #endregion

    #region Private Members
    private readonly ILogger<CambioMittentiMultipliCommandHandler> _logger;
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
