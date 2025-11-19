// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.HMDiritti;
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getTipoAttoPDInsRicWithStateDiagramRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTipoAttoPDInsRicWithStateDiagram;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTipoAttoPDInsRicWithStateDiagram
{
    public class getTipoAttoPDInsRicWithStateDiagramHandler : IRequestHandler<getTipoAttoPDInsRicWithStateDiagramRequest, getTipoAttoPDInsRicWithStateDiagramResult>
    {
        #region Public Members

        public getTipoAttoPDInsRicWithStateDiagramHandler(ILogger<getTipoAttoPDInsRicWithStateDiagramHandler> logger,
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

        public async Task<getTipoAttoPDInsRicWithStateDiagramResult> Handle(getTipoAttoPDInsRicWithStateDiagramRequest request, CancellationToken cancellationToken)
        {
            TipologiaAtto[] output = null;

            try
            {
                var idAmministrazione = request.idAmministrazione.AsLong();
                var idGruppo = request.idGruppo.AsLong();

                var queryable = this._dbContext.TipoAttoEntities
                    .Join(this._dbContext.VisTipoDocEntities, atto => atto.SYSTEM_ID, vis => vis.ID_TIPO_DOC, (atto, vis) => new { atto, vis })
                    .Where(j => this._dbContext.AssDiagrammiEntities.Any(d => d.ID_TIPO_DOC == j.atto.SYSTEM_ID) && (j.atto.ID_AMM == idAmministrazione || j.atto.ID_AMM == null));

                //Ricerca
                if(request.diritti == "1")
                {
                    long?[] diritti = new long?[] { 1, 2 };
                    queryable = queryable.Where(j => (j.vis.ID_RUOLO == 0 || (j.vis.ID_RUOLO == idGruppo && diritti.Contains(j.vis.DIRITTI)) || (j.atto.IPERDOCUMENTO == 1)));
                }

                //Inserimento
                if (request.diritti == "2")
                {
                    queryable = queryable.Where(j => (j.atto.IN_ESERCIZIO != "NO" || j.atto.IN_ESERCIZIO == null) && (j.atto.ABILITATO_SI_NO != 0 || j.atto.ABILITATO_SI_NO == null) && (j.vis.ID_RUOLO == 0 || (j.vis.ID_RUOLO == idGruppo && j.vis.DIRITTI == 2)));
                }

                var tipoAttoEntities = await queryable
                    .Select(j => new TipoAttoEntity()
                    {
                         SYSTEM_ID = j.atto.SYSTEM_ID,
                         VAR_DESC_ATTO = j.atto.VAR_DESC_ATTO,
                         PESO = j.atto.PESO,
                    })
                    .AsNoTracking()
                    .Distinct()
                    .OrderByDescending(j => j.PESO)
                    .ThenBy(j => j.VAR_DESC_ATTO)
                    .ToListAsync();

                output = this._mapper.Map<TipologiaAtto[]>(tipoAttoEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new getTipoAttoPDInsRicWithStateDiagramResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTipoAttoPDInsRicWithStateDiagramHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoEntity, TipologiaAtto>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_ATTO));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
