// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
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
using getStatoSuccessivoAutomaticoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getStatoSuccessivoAutomatico;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getStatoSuccessivoAutomatico
{
    public class getStatoSuccessivoAutomaticoHandler : IRequestHandler<getStatoSuccessivoAutomaticoRequest, getStatoSuccessivoAutomaticoResult>
    {
        #region Public Members

        public getStatoSuccessivoAutomaticoHandler(ILogger<getStatoSuccessivoAutomaticoHandler> logger, 
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

        public async Task<getStatoSuccessivoAutomaticoResult> Handle(getStatoSuccessivoAutomaticoRequest request, CancellationToken cancellationToken)
        {
            Stato output = null;
            try
            {
                var result_getStatoDoc = await this._mediator.Send(new Application.Requests.getStatoDoc(request.docNumber));
                if (result_getStatoDoc.output != null)
                {
                    var result_getDiagrammaById = await this._mediator.Send(new Application.Requests.getDiagrammaById(result_getStatoDoc.output.ID_DIAGRAMMA.ToString()));

                    Passo passo = result_getDiagrammaById.output.PASSI.ToArray().Cast<Passo>()
                        .Where(p => p.STATO_PADRE.SYSTEM_ID == result_getStatoDoc.output.SYSTEM_ID && !string.IsNullOrEmpty(p.ID_STATO_AUTOMATICO))
                        .FirstOrDefault();

                    if (passo != null)
                    {
                        long idStatoAutomatico = Convert.ToInt64(passo.ID_STATO_AUTOMATICO);
                        var statoEntity = await this._dbContext.StatoEntities.FirstOrDefaultAsync(s => s.SYSTEM_ID == idStatoAutomatico);

                        output = this._mapper.Map<Stato>(statoEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new getStatoSuccessivoAutomaticoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getStatoSuccessivoAutomaticoHandler> _logger;
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
