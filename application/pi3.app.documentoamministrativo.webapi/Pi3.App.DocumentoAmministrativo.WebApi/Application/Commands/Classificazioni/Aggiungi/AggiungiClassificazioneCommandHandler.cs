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

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Commands.Classificazioni.Aggiungi;

public class AggiungiClassificazioneCommandHandler : IRequestHandler<AggiungiClassificazioneCommand, AggiungiClassificazioneCommandResponse>
{
    #region Public Members
    public AggiungiClassificazioneCommandHandler(
        ILogger<AggiungiClassificazioneCommandHandler> logger,
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

    public async Task<AggiungiClassificazioneCommandResponse> Handle(AggiungiClassificazioneCommand request, CancellationToken cancellationToken)
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

        await Helpers.DecodificaEAggiungiClassificazioneNonProtocollato(request.Classificazione, _claimsPrincipalService, _context, idTenant, aggregate);


        await this._repository.Update(aggregate);

        return new AggiungiClassificazioneCommandResponse
        {
            DocumentoAmministrativo = this._mapper.Map<Documento>(aggregate)
        };

        //async Task DecodificaEAggiungiClassificazioneNonProtocollato(Classification classificazione)
        //{
        //    var sessionGroupId = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

        //    // -- step 1: REPERIMENTO REGISTRO DEL RUOLO CORRENTE
        //    var groupEntity = await this._context.CorrGlobaliEntities.AsNoTracking().FirstOrDefaultAsync(cge => cge.ID_GRUPPO == sessionGroupId);
        //    if (groupEntity is null)
        //        throw new ApplicationException();
        //    var groupId = groupEntity.SYSTEM_ID;

        //    var systemIdRegistro = await (from der in this._context.RegistroEntities.AsNoTracking()
        //                                  join dlrr in this._context.RuoloRegistroEntities.AsNoTracking() on der.SYSTEM_ID equals dlrr.ID_REGISTRO
        //                                  where dlrr.ID_RUOLO_IN_UO == groupId && der.DTA_CLOSE == null
        //                                  select der.SYSTEM_ID).FirstOrDefaultAsync();
        //    // -- step 2: REPERIEMNTO TITOLARIO ATTIVO
        //    var systemIdTitolario = await this._context.ProjectEntities.Where(prj => prj.ID_AMM == Convert.ToInt32(idTenant)
        //        && prj.CHA_TIPO_PROJ == "T" && prj.ID_TITOLARIO == 0 && prj.CHA_STATO == "A").Select(prj => prj.SYSTEM_ID).FirstOrDefaultAsync();

        //    // --step 3: REPERIMENTO DEL NODO TITOLARIO
        //    var titolario = await (this._context.ProjectEntities.Where(prj => prj.VAR_CODICE == classificazione.CodiceClassificazione
        //        && prj.CHA_TIPO_PROJ == "T" && prj.ID_REGISTRO == systemIdRegistro && prj.ID_TITOLARIO == systemIdTitolario)
        //        .Select(prj => new { prj.SYSTEM_ID, prj.DESCRIPTION, prj.VAR_CODICE })).FirstOrDefaultAsync();

        //    aggregate.AddClassification(titolario.SYSTEM_ID.ToString(),
        //        new TextValue(titolario.DESCRIPTION),
        //        titolario.VAR_CODICE.ToString());

        //}

    }

    #endregion

    #region Private Members
    private readonly ILogger<AggiungiClassificazioneCommandHandler> _logger;
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
