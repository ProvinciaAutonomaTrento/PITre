// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Roles.GetFunzioniRuolo
{
    public class GetFunzioniRuoloCommandHandler : IRequestHandler<GetFunzioniRuoloCommand, GetFunzioniRuoloCommandResponse>
    {
        public GetFunzioniRuoloCommandHandler(ILogger<GetFunzioniRuoloCommandHandler> logger,
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


        public async Task<GetFunzioniRuoloCommandResponse> Handle(GetFunzioniRuoloCommand request, CancellationToken cancellationToken)
        {
            Funzione[] output = null;

            try
            {
                var idCorrGlobaliAsLong = request.IdCorrGlobali.AsLong();

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
                                   VAR_DESC_FUNZIONE = j.f.VAR_DESC_FUNZIONE,
                                   ID_TIPO_FUNZIONE = j.f.ID_TIPO_FUNZIONE,
                               },
                               TipoFunzione = new TipoFunzioneEntity()
                               {

                                   VAR_COD_TIPO = j.t.VAR_COD_TIPO,
                                   VAR_DESC_TIPO_FUN = j.t.VAR_DESC_TIPO_FUN
                               }
                           })
                    .ToListAsync();

                if (funzioniEntities != null && funzioniEntities.Count > 0)
                {
                    output = this._mapper.Map<Funzione[]>(funzioniEntities);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new()
            {
                Funzioni = output
            };
        }


        #region Private Members

        protected readonly ILogger<GetFunzioniRuoloCommandHandler> _logger;
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
