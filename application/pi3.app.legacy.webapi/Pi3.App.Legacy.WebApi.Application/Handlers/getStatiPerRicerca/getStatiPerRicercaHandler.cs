// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetVersionsMainDocument;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getStatiPerRicercaRequest = Pi3.App.Legacy.WebApi.Application.Requests.getStatiPerRicerca;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getStatiPerRicerca
{
    public class getStatiPerRicercaHandler : IRequestHandler<getStatiPerRicercaRequest, getStatiPerRicercaResult>
    {
        #region Public Members

        public getStatiPerRicercaHandler(ILogger<getStatiPerRicercaHandler> logger,
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

        public async Task<getStatiPerRicercaResult> Handle(getStatiPerRicercaRequest request, CancellationToken cancellationToken)
        {
            Stato[] output = null;

            try
            {
                var idDiagrammaAsLong = request.idDiagramma.AsLong();

                var statiQueryable = this._dbContext.DiagrammiEntities.AsNoTracking()
                    .Join(this._dbContext.StatoEntities.AsNoTracking(), diagramma => diagramma.ID_STATO, stato => stato.SYSTEM_ID, (diagramma, stato) => new { diagramma, stato })
                    .Where(j => j.diagramma.ID_DIAGRAMMA == idDiagrammaAsLong && (j.stato.NON_RICERCABILE != 1 || j.stato.NON_RICERCABILE == null));

                statiQueryable = (request.docOrFasc == "D" ? statiQueryable.Where(j => j.diagramma.DOC_NUMBER != null) : statiQueryable.Where(j => j.diagramma.ID_PROJECT != null));

                var statiEntity = await statiQueryable
                    .Select(j => new StatoEntity
                    {
                        SYSTEM_ID = j.stato.SYSTEM_ID,
                        VAR_DESCRIZIONE = j.stato.VAR_DESCRIZIONE
                    })
                    .OrderBy(j => j.VAR_DESCRIZIONE)
                    .Distinct()
                    .ToListAsync();

                if(statiEntity != null)
                    output = _mapper.Map<Stato[]>(statiEntity);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                output = null;
            }

            return new getStatiPerRicercaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getStatiPerRicercaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StatoEntity, Stato>()
                    .ForMember(dest => dest.SYSTEM_ID, src => src.MapFrom(opt => Convert.ToInt32(opt.SYSTEM_ID)))
                    .ForMember(dest => dest.DESCRIZIONE, src => src.MapFrom(opt => opt.VAR_DESCRIZIONE));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
