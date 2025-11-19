// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetDiagrammaById
{
    public class GetDiagrammaByIdCommandHandler : IRequestHandler<GetDiagrammaByIdCommand, GetDiagrammaByIdCommandResponse>
    {
        public GetDiagrammaByIdCommandHandler(ILogger<GetDiagrammaByIdCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this.InitializeMapper();
        }

        public async Task<GetDiagrammaByIdCommandResponse> Handle(GetDiagrammaByIdCommand request, CancellationToken cancellationToken)
        {
            DiagrammaStato output = null;

            try
            {
                long idDiagramma = Convert.ToInt64(request.IdDiagramma);

                var diagrammaEntity = await this._dbContext.DiagrammiStatoEntities.FirstOrDefaultAsync(d => d.SYSTEM_ID == idDiagramma);

                output = this._mapper.Map<DiagrammaStato>(diagrammaEntity);

                var statoEntities = await this._dbContext.StatoEntities.Where(s => s.ID_DIAGRAMMA == output.SYSTEM_ID).ToListAsync();

                List<Stato> statiList = new List<Stato>();
                statiList.AddRange(this._mapper.Map<Stato[]>(statoEntities));
                output.STATI = statiList.ToArray();


                foreach (Stato stato in output.STATI)
                {
                    var passoEntities = await this._dbContext.PassoEntities
                        .Join(
                            this._dbContext.StatoEntities,
                            passo => passo.ID_STATO,
                            stato => stato.SYSTEM_ID,
                            (passo, stato) => new { passo, stato }
                        )
                        .Join(
                            this._dbContext.StatoEntities,
                            passo => passo.passo.ID_NEXT_STATO,
                            statoNext => statoNext.SYSTEM_ID,
                            (passo, statoNext) => new { passo, statoNext }
                        )
                        .Where(p => p.passo.passo.ID_STATO == stato.SYSTEM_ID)
                        .ToListAsync();

                    if (passoEntities != null && passoEntities.Any())
                    {
                        Passo passo = this._mapper.Map<Passo>(passoEntities[0].passo.passo);
                        passo.STATO_PADRE = this._mapper.Map<Stato>(passoEntities[0].passo.stato);
                        List<Stato> passiSuccessiviList = new List<Stato>();
                        passoEntities.ForEach(x =>
                        {
                            passiSuccessiviList.Add(this._mapper.Map<Stato>(x.statoNext));
                            if (x.passo.passo.CHA_STATO_AUTOMATICO_LF == "1")
                                passo.ID_STATO_AUTOMATICO_LF = x.statoNext.SYSTEM_ID.ToString();
                        });

                        passo.SUCCESSIVI = passiSuccessiviList.ToArray();

                        List<Passo> passiList = output.PASSI != null ? output.PASSI.ToList() : new List<Passo>();
                        if (passo.STATO_PADRE != null)
                            passiList.Add(passo);

                        output.PASSI = passiList.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = output
            };
        }

        #region Private Members
        protected readonly ILogger<GetDiagrammaByIdCommandHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;


        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StatoEntity, Stato>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.ID_DIAGRAMMA, src => src.MapFrom(opt => opt.ID_DIAGRAMMA))
                    .ForMember(dest => dest.DESCRIZIONE, src => src.MapFrom(opt => opt.VAR_DESCRIZIONE))
                    .ForMember(dest => dest.STATO_INIZIALE, src => src.MapFrom(opt => opt.STATO_INIZIALE == 1))
                    .ForMember(dest => dest.STATO_FINALE, src => src.MapFrom(opt => opt.STATO_FINALE == 1))
                    .ForMember(dest => dest.CONVERSIONE_PDF, src => src.MapFrom(opt => opt.CONV_PDF == 1))
                    .ForMember(dest => dest.NON_RICERCABILE, src => src.MapFrom(opt => opt.NON_RICERCABILE == 1))
                    .ForMember(dest => dest.ID_PROCESSO_FIRMA, src => src.MapFrom(opt => opt.ID_PROCESSO_FIRMA))
                    .ForMember(dest => dest.PUBBLICAZIONE_FILES, src => src.MapFrom(opt => opt.CHA_PUBB_SELECT_FILES == "1"))
                    .ForMember(dest => dest.STATO_CONSOLIDAMENTO, src => src.MapFrom(opt => string.IsNullOrEmpty(opt.STATO_CONSOLIDAMENTO) ?
                        DocumentConsolidationStateEnum.None : (DocumentConsolidationStateEnum)Enum.Parse(typeof(DocumentConsolidationStateEnum), opt.STATO_CONSOLIDAMENTO)));

                cfg.CreateMap<PassoEntity, Passo>()
                   .ForMember(dest => dest.ID_DIAGRAMMA, src => src.MapFrom(opt => opt.ID_DIAGRAMMA))
                   .ForMember(dest => dest.ID_STATO_AUTOMATICO, src => src.MapFrom(opt => opt.ID_STATO_AUTO))
                   .ForMember(dest => dest.DESCRIZIONE_STATO_AUTOMATICO, src => src.MapFrom(opt => opt.DESC_STATO_AUTO));

                cfg.CreateMap<DiagrammiStatoEntity, DiagrammaStato>()
                   .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                   .ForMember(dest => dest.ID_AMM, src => src.MapFrom(opt => opt.ID_AMM))
                   .ForMember(dest => dest.DESCRIZIONE, src => src.MapFrom(opt => opt.VAR_DESCRIZIONE));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
