// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.Interoperabilita.Semplificata;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Infrastructure.EF.Oracle.Handlers.getStatoDoc
{
    public class getStatoDocHandler : IRequestHandler<Application.Requests.getStatoDoc, getStatoDocResult>
    {
        #region Public Members

        public getStatoDocHandler(ILogger<getStatoDocHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<getStatoDocResult> Handle(Application.Requests.getStatoDoc request, CancellationToken cancellationToken)
        {
            Stato output = null;

            try
            {
                if (!string.IsNullOrEmpty(request.docNumber))
                {
                    long docnumber = Convert.ToInt64(request.docNumber);

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

                    if(statoEntity != null)
                        output = this._mapper.Map<Stato>(statoEntity.stato);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getStatoDocResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getStatoDocHandler> _logger;
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
