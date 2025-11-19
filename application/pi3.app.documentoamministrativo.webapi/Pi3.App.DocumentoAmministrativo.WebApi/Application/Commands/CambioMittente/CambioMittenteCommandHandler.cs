// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Repositories;
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.ValueObjects;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.CambioMittente;

public class CambioMittenteCommandHandler : IRequestHandler<CambioMittenteCommand, CambioMittenteCommandResponse>
{
    #region Public Members
    public CambioMittenteCommandHandler(
        ILogger<CambioMittenteCommandHandler> logger,
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

    public async Task<CambioMittenteCommandResponse> Handle(CambioMittenteCommand request, CancellationToken cancellationToken)
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
            corr.DTA_FINE == null).FirstOrDefaultAsync()).SYSTEM_ID;
        var mittente = new Mittente(idMittente.ToString());

        aggregate.AssignMittente(mittente);

        await _repository.Update(aggregate);

        return new CambioMittenteCommandResponse
        {
            DocumentoAmministrativo = _mapper.Map<Documento>(aggregate)
        };
    }

    #endregion

    #region Private Members
    private readonly ILogger<CambioMittenteCommandHandler> _logger;
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
