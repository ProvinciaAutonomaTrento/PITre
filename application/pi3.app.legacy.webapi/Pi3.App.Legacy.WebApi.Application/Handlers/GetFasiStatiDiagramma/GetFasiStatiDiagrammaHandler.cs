// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListaRuoliAOO;
using Pi3.App.Legacy.WebApi.Application.Handlers.FunzioneEsistente;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetFasiStatiDiagrammaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFasiStatiDiagramma;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFasiStatiDiagramma
{
    public class GetFasiStatiDiagrammaHandler : IRequestHandler<GetFasiStatiDiagrammaRequest, GetFasiStatiDiagrammaResult>
    {

        protected readonly ILogger<GetFasiStatiDiagrammaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;


        public GetFasiStatiDiagrammaHandler(
            ILogger<GetFasiStatiDiagrammaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
        }



        public async Task<GetFasiStatiDiagrammaResult> Handle(GetFasiStatiDiagrammaRequest request, CancellationToken cancellationToken)
        {
            List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma> faseStatiDiagramma = null;

            try
            {
                faseStatiDiagramma = (List<DocsPaVO.DiagrammaStato.AvanzamentoDiagramma.AssPhaseStatoDiagramma>)this._dbContext.AssociazioneStatiPhasesEntities.AsNoTracking().Join(
                    this._dbContext.PhaseEntities.AsNoTracking(),
                    sp => sp.ID_PHASES,
                    p => p.SYSTEM_ID,
                    (sp, p) => new
                    {
                        sp.ID_STATO,
                        sp.ID_PHASES,
                        DESCRIZIONE_PHASE = p.DESCRIPTION,
                        sp.POSITION_PHASE,
                        sp.CHA_INTERMEDIATE_PHASE,
                        sp.ID_DIAGRAMMA
                    }).Where(row => row.ID_DIAGRAMMA == request.idDiagramma.AsLong()).Join(this._dbContext.StatoEntities.AsNoTracking(),
                            nsp => nsp.ID_STATO,
                            s => s.SYSTEM_ID,
                            (nsp, s) => new
                            {
                                INTERMEDIATE_PHASE = nsp.CHA_INTERMEDIATE_PHASE == "0" ? false : true,
                                nsp.POSITION_PHASE,
                                STATO = new
                                {
                                    SYSTEM_ID = nsp.ID_STATO,
                                    STATO_INIZIALE = s.STATO_INIZIALE.ToString() == "0" ? false : true
                                },
                                PHASE = new
                                {
                                    SYSTEM_ID = nsp.ID_PHASES.ToString(),
                                    DESCRIZIONE = nsp.DESCRIZIONE_PHASE.ToString()
                                }
                            }).OrderBy(nsp => nsp.POSITION_PHASE);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);

            }

            return new GetFasiStatiDiagrammaResult(faseStatiDiagramma?.ToArray());
        }
    }
}
