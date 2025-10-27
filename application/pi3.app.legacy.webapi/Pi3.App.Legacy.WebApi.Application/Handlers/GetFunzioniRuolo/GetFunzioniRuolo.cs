// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.LibroFirma;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Handlers.GetProcessoDiFirma;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetFunzioniRuoloRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetFunzioniRuolo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetFunzioniRuolo
{
    public class GetFunzioniRuolo : IRequestHandler<GetFunzioniRuoloRequest, GetFunzioniRuoloResult>
    {
        #region Public Members

        public GetFunzioniRuolo(ILogger<GetFunzioniRuolo> logger,
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

        public async Task<GetFunzioniRuoloResult> Handle(GetFunzioniRuoloRequest request, CancellationToken cancellationToken)
        {
            Funzione[] output = null;

            try
            {
                var idCorrGlobaliAsLong = request.idCorrGlobali.AsLong();

                var funzioniEntities = await this._dbContext.FunzioneEntities.AsNoTracking()
                           .Join(this._dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                           .Join(this._dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                           .Where(j => j.r.ID_RUOLO_IN_UO == idCorrGlobaliAsLong)
                           .Select(j => new FunzioniEntity()
                           {
                               Funzione = new FunzioneEntity()
                               {
                                   SYSTEM_ID = j.f.SYSTEM_ID,
                                   COD_FUNZIONE = j.f.COD_FUNZIONE,
                                   VAR_DESC_FUNZIONE= j.f.VAR_DESC_FUNZIONE,
                                   ID_TIPO_FUNZIONE = j.f.ID_TIPO_FUNZIONE,
                               },
                               TipoFunzione = new TipoFunzioneEntity()
                               {

                                   VAR_COD_TIPO = j.t.VAR_COD_TIPO,
                                   VAR_DESC_TIPO_FUN = j.t.VAR_DESC_TIPO_FUN
                               }
                           })
                    .ToListAsync();

                if(funzioniEntities != null && funzioniEntities.Count > 0)
                {
                    output = this._mapper.Map<Funzione[]>(funzioniEntities);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new GetFunzioniRuoloResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetFunzioniRuolo> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FunzioniEntity, Funzione>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.Funzione.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.Funzione.VAR_DESC_FUNZIONE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.Funzione.COD_FUNZIONE))
                    .ForMember(dest => dest.idTipoFunzione, src => src.MapFrom(opt => opt.Funzione.ID_TIPO_FUNZIONE))
                    .ForMember(dest => dest.codTipoFunzione, src => src.MapFrom(opt => opt.TipoFunzione.VAR_COD_TIPO))
                    .ForMember(dest => dest.descTipoFunzione, src => src.MapFrom(opt => opt.TipoFunzione.VAR_DESC_TIPO_FUN));
            });

            _mapper = configuration.CreateMapper();
        }

        protected class FunzioniEntity
        {
            public FunzioneEntity Funzione { get; set; }
            public TipoFunzioneEntity TipoFunzione { get; set; }
        }

        #endregion
    }
}
