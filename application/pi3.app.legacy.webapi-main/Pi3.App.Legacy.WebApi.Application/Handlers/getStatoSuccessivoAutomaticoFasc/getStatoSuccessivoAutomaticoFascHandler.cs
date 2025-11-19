// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getStatoSuccessivoAutomaticoFascRequest = Pi3.App.Legacy.WebApi.Application.Requests.getStatoSuccessivoAutomaticoFasc;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getStatoSuccessivoAutomaticoFasc
{
    public class getStatoSuccessivoAutomaticoFascHandler : IRequestHandler<getStatoSuccessivoAutomaticoFascRequest, getStatoSuccessivoAutomaticoFascResult>
    {
        #region Public Members

        public getStatoSuccessivoAutomaticoFascHandler(ILogger<getStatoSuccessivoAutomaticoFascHandler> logger,
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

        public async Task<getStatoSuccessivoAutomaticoFascResult> Handle(getStatoSuccessivoAutomaticoFascRequest request, CancellationToken cancellationToken)
        {
            Stato output = null;
            try
            {
                var result_getStatoFasc = await this._mediator.Send(new Application.Requests.getStatoFasc(request.idProject));
                var result_getDiagrammaById = await this._mediator.Send(new Application.Requests.getDiagrammaById(result_getStatoFasc.output.ID_DIAGRAMMA.ToString()));

                Passo passo = result_getDiagrammaById.output.PASSI.ToArray().Cast<Passo>()
                    .Where(p => p.STATO_PADRE.SYSTEM_ID == result_getStatoFasc.output.SYSTEM_ID && !string.IsNullOrEmpty(p.ID_STATO_AUTOMATICO))
                    .FirstOrDefault();

                if (passo != null)
                {
                    var idStatoAutomatico = passo.ID_STATO_AUTOMATICO.AsLong();
                    var statoEntity = await this._dbContext.StatoEntities.FirstOrDefaultAsync(s => s.SYSTEM_ID == idStatoAutomatico);

                    output = this._mapper.Map<Stato>(statoEntity);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new getStatoSuccessivoAutomaticoFascResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getStatoSuccessivoAutomaticoFascHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StatoEntity, Stato>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESCRIZIONE))
                     .ForMember(dest => dest.STATO_INIZIALE, opt => opt.MapFrom(src => src.STATO_INIZIALE == 1))
                     .ForMember(dest => dest.STATO_FINALE, opt => opt.MapFrom(src => src.STATO_FINALE == 1))
                     .ForMember(dest => dest.ID_PROCESSO_FIRMA, opt => opt.MapFrom(src => src.ID_PROCESSO_FIRMA))
                     .ForMember(dest => dest.ID_DIAGRAMMA, opt => opt.MapFrom(src => src.ID_DIAGRAMMA))
                     .ForMember(dest => dest.CONVERSIONE_PDF, opt => opt.MapFrom(src => src.CONV_PDF == 1))
                     .ForMember(dest => dest.NON_RICERCABILE, opt => opt.MapFrom(src => src.NON_RICERCABILE == 1))
                     .ForMember(dest => dest.PUBBLICAZIONE_FILES, opt => opt.MapFrom(src => src.CHA_PUBB_SELECT_FILES == "1"))
                     .ForMember(dest => dest.STATO_CONSOLIDAMENTO, src => src.MapFrom(opt => string.IsNullOrEmpty(opt.STATO_CONSOLIDAMENTO) ?
                        DocsPaVO.documento.DocumentConsolidationStateEnum.None : (DocsPaVO.documento.DocumentConsolidationStateEnum)Enum.Parse(typeof(DocsPaVO.documento.DocumentConsolidationStateEnum), opt.STATO_CONSOLIDAMENTO)))
                    .ForMember(dest => dest.STATO_SISTEMA, opt => opt.MapFrom(src => src.CHA_STATO_SISTEMA == "1"));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
