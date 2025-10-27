// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.DocumentoAmministrativo.WebApi.Application.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.DocumentoAmministrativo.WebApi.Application.Queries.GetTrasmissione;

public class GetTrasmissioneQueryHandler : IRequestHandler<GetTrasmissioneQuery, GetTrasmissioneQueryResponse>
{
    public GetTrasmissioneQueryHandler(
        ILogger<GetTrasmissioneQueryHandler> logger,
        IClaimsPrincipalService claimsPrincipalService,
        IMediator mediator,
        IPi3DbContext context,
        ITrasmissioneRepository trasmissioneRepository
        )
    {
        this._logger = logger;
        this._claimsPrincipalService = claimsPrincipalService;
        this._mediator = mediator;
        this._context = context;
        this._trasmissioneRepository = trasmissioneRepository;

        InitializeMapper();
    }

    public async Task<GetTrasmissioneQueryResponse> Handle(GetTrasmissioneQuery request, CancellationToken cancellationToken)
    {
        if (!request.Id.IsValidAggregateId())
            throw new InvalidIdDocumentPi3Exception(request.Id);

        var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);

        if (!await this._trasmissioneRepository.Exists(idTenant!, request.Id!))
            throw new TrasmissioneNotFoundPi3Exception(request.Id);

        var trasmissioneAggregate = await this._trasmissioneRepository.Get(idTenant!, request.Id!);
        
        return new GetTrasmissioneQueryResponse()
        {
            Trasmissione = this._mapper.Map<Trasmissione>(trasmissioneAggregate)
        };
    }

    #region Private Methods

    private readonly ILogger<GetTrasmissioneQueryHandler> _logger;
    private readonly IClaimsPrincipalService _claimsPrincipalService;
    private readonly IMediator _mediator;
    private readonly IPi3DbContext _context;
    private readonly ITrasmissioneRepository _trasmissioneRepository;
    private IMapper _mapper;

    protected void InitializeMapper()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Pi3.Core.AggregateModels.TrasmissioneAggregate.Trasmissione, Trasmissione>()
                .ForMember(dest => dest.NoteGenerali, opt => opt.MapFrom(src => (src.NoteGenerali! != null! ? src.NoteGenerali.ToString() : null)))
                .ForMember(dest => dest.Autore, opt => opt.MapFrom(src => src.Autore))
                .AfterMap((src, dest) =>
                {
                    List<GruppoDestinatario> gruppiDestinatari = null!;
                    List<UtenteDestinatario> utentiDestinatari = null!;

                    foreach (var ts in src.TrasmissioniSingole)
                    {
                        if (ts.GetType() == typeof(Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaGruppo))
                        {
                            var tsGruppo = (Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaGruppo)ts;
                            
                            if (gruppiDestinatari == null)
                                gruppiDestinatari = new List<GruppoDestinatario>();

                            gruppiDestinatari.Add(this._mapper.Map<GruppoDestinatario>(ts));
                        }
                        else if (ts.GetType() == typeof(Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaUtente))
                        {
                            var tsUtente = (Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaUtente)ts;

                            if (utentiDestinatari == null)
                                utentiDestinatari = new List<UtenteDestinatario>();

                            utentiDestinatari.Add(this._mapper.Map<UtenteDestinatario>(ts));
                        }
                    }

                    if (gruppiDestinatari != null)
                        dest.GruppiDestinatari = gruppiDestinatari;

                    if (utentiDestinatari != null)
                        dest.UtentiDestinatari = utentiDestinatari;
                });


            cfg.CreateMap<Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects.Autore, Autore>()
                .AfterMap(async (src, dest) =>
                {
                    if (!string.IsNullOrWhiteSpace(src.IdUtente) && string.IsNullOrWhiteSpace(src.UserId))
                    {
                        await LookUpUser(src.IdUtente,
                            (userInfo) =>
                            {
                                dest.UserId = userInfo.UserId;
                                dest.Cognome = userInfo.Cognome;
                                dest.Nome = userInfo.Nome;
                            });
                    }

                    if (!string.IsNullOrWhiteSpace(src.IdUtenteDelegato) && string.IsNullOrWhiteSpace(src.IdUtenteDelegato))
                    {
                        await LookUpUser(src.IdUtenteDelegato,
                            (userInfo) =>
                            {
                                dest.UserId = userInfo.UserId;
                                dest.Cognome = userInfo.Cognome;
                                dest.Nome = userInfo.Nome;
                            });
                    }

                    if (!string.IsNullOrWhiteSpace(src.IdGruppo) && string.IsNullOrWhiteSpace(src.CodiceGruppo))
                    {
                        var groupInfo = await this._context
                            .GroupEntities
                            .AsNoTracking()
                            .Where(g => g.SYSTEM_ID == src.IdGruppo.AsLong())
                            .Select(g => new
                            {
                                g.GROUP_ID,
                                g.GROUP_NAME
                            })
                            .FirstOrDefaultAsync();

                        if (groupInfo != null)
                        {
                            dest.CodiceGruppo = groupInfo.GROUP_ID!;
                            dest.DescrizioneGruppo = groupInfo.GROUP_NAME!;
                        }
                    }
                });

            cfg.CreateMap<Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaGruppo, GruppoDestinatario>()
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo.ToString()))
                .ForMember(dest => dest.CodiceGruppo, opt => opt.MapFrom(src => src.GruppoDestinatario.Codice))
                .ForMember(dest => dest.DescrizioneGruppo, opt => opt.MapFrom(src => src.GruppoDestinatario.Descrizione!.ToString()))
                .ForMember(dest => dest.RagioneTrasmissione, opt => opt.MapFrom(src => src.RagioneTrasmissione.Nome))
                .ForMember(dest => dest.NoteTrasmissione, opt => opt.MapFrom(src => (src.Note! != null! ? src.Note.ToString() : null)))
                .ForMember(dest => dest.CodiceGruppo, opt => opt.MapFrom(src => src.GruppoDestinatario.Codice))
                .ForMember(dest => dest.DescrizioneGruppo, opt => opt.MapFrom(src => src.GruppoDestinatario.Descrizione))
                .ForMember(dest => dest.DataScadenza, opt => opt.MapFrom(src => src.DataScadenza.AsDateTimeFormat()))
                .ForMember(dest => dest.UtentiNotificati, opt => opt.MapFrom(src => 
                    src.TrasmissioniUtente.Select(tu => new UtenteNotificato()
                    {
                        UserId = tu.UtenteDestinatario.UserId!,
                        Nome = tu.UtenteDestinatario.Nome!,
                        Cognome = tu.UtenteDestinatario.Cognome!
                    })));

            cfg.CreateMap<Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities.TrasmissioneSingolaUtente, UtenteDestinatario>()
                .ForMember(dest => dest.RagioneTrasmissione, opt => opt.MapFrom(src => src.RagioneTrasmissione.Nome))
                .ForMember(dest => dest.NoteTrasmissione, opt => opt.MapFrom(src => (src.Note! != null! ? src.Note.ToString() : null)))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UtenteDestinatario.UserId))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.UtenteDestinatario.Nome))
                .ForMember(dest => dest.Cognome, opt => opt.MapFrom(src => src.UtenteDestinatario.Cognome))
                .ForMember(dest => dest.DataScadenza, opt => opt.MapFrom(src => src.DataScadenza));
        });

        _mapper = configuration.CreateMapper();

        async Task LookUpUser(string idUtente, Action<(string UserId, string Cognome, string Nome)> callback)
        {
            var userInfo = await this._context
                                    .PeopleEntities
                                    .AsNoTracking()
                                    .Where(p => p.SYSTEM_ID == idUtente.AsLong())
                                    .Select(p => new
                                    {
                                        p.USER_ID,
                                        p.VAR_COGNOME,
                                        p.VAR_NOME
                                    })
                                    .FirstOrDefaultAsync();

            if (userInfo != null)
                callback(new (userInfo!.USER_ID!, userInfo.VAR_COGNOME!, userInfo.VAR_NOME!));
        }
    }

    #endregion
}
