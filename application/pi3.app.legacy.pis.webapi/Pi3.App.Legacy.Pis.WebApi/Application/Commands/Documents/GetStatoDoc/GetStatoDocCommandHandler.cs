// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Pi3.App.Legacy.Pis.WebApi.Infrastructure.Services.Principal;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.GetStatoDoc
{
    public class GetStatoDocCommandHandler : IRequestHandler<GetStatoDocCommand, GetStatoDocCommandResponse>
    {
        public GetStatoDocCommandHandler(
            ILogger<GetStatoDocCommandHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext
            )
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetStatoDocCommandResponse> Handle(GetStatoDocCommand request, CancellationToken cancellationToken)
        {
            Stato output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.DocNumber))
                {
                    long docnumber = Convert.ToInt64(request.DocNumber);

                    var statoEntity = await this._dbContext.DiagrammiEntities
                        .Join(
                            this._dbContext.StatoEntities,
                            diagramma => diagramma.ID_STATO,
                            stato => stato.SYSTEM_ID,
                            (diagramma, stato) => new
                            {
                                diagramma.DOC_NUMBER,
                                stato
                            }
                        )
                        .FirstOrDefaultAsync(d => d.DOC_NUMBER == docnumber);

                    if (statoEntity != null)
                        output = this._mapper.Map<Stato>(statoEntity.stato);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new()
            {
                Output = output
            };
        }


        #region Private Members
        protected readonly ILogger<GetStatoDocCommandHandler> _logger;
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
                    .ForMember(dest => dest.ID_PROCESSO_FIRMA, src => src.MapFrom(opt => opt.ID_PROCESSO_FIRMA));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
