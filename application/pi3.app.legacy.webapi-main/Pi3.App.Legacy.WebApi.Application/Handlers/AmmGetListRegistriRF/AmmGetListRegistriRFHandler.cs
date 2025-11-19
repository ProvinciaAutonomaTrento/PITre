// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
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
using AmmGetListRegistriRFRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetListRegistriRF;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetListRegistriRF
{
    public class AmmGetListRegistriRFHandler : IRequestHandler<AmmGetListRegistriRFRequest, AmmGetListRegistriRFResult>
    {
        #region Public Members

        public AmmGetListRegistriRFHandler(ILogger<AmmGetListRegistriRFHandler> logger,
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

        public async Task<AmmGetListRegistriRFResult> Handle(AmmGetListRegistriRFRequest request, CancellationToken cancellationToken)
        {
            OrgRegistro[] output = null;

            try
            {
                var idAmm = request.idAmm.AsLong();

                IQueryable<RegistroAssociatoEntity> queryable = queryable = this._dbContext.RegistroEntities.AsNoTracking()
                        .Where(r => r.ID_AMM == idAmm)
                        .Select(r => new RegistroAssociatoEntity()
                        {
                            Registro = r
                        }); 

                if(!string.IsNullOrEmpty(request.idRuolo))
                {
                    var idRuolo = request.idRuolo.AsLong();
                    
                    if (!string.IsNullOrEmpty(request.chaRF))
                        queryable = queryable.Where(r => r.Registro.CHA_RF == request.chaRF);

                    queryable = queryable
                        .Select(r => new RegistroAssociatoEntity()
                        {
                            Registro = r.Registro,
                            ASSOCIATO = this._dbContext.RuoloRegistroEntities.AsNoTracking()
                                        .Where(l => l.ID_REGISTRO == r.Registro.SYSTEM_ID && l.ID_RUOLO_IN_UO == idRuolo)
                                        .Select(l => l.SYSTEM_ID).FirstOrDefault()
                        });
                }
                else
                {
                    queryable = queryable
                        .Where(r => r.Registro.CHA_RF == "0")
                        .OrderBy(r => r.Registro.VAR_DESC_REGISTRO);
                }

                var registroEntities = await queryable.ToListAsync();
                output = this._mapper.Map<OrgRegistro[]>(registroEntities);

            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
            }

            return new AmmGetListRegistriRFResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetListRegistriRFHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroAssociatoEntity, OrgRegistro>()
                    .ForMember(dest => dest.IDRegistro, opt => opt.MapFrom(src => src.Registro.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, opt => opt.MapFrom(src => src.Registro.VAR_CODICE))
                    .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.Registro.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.IDAmministrazione, opt => opt.MapFrom(src => src.Registro.ID_AMM))
                    .ForMember(dest => dest.Associato, opt => opt.MapFrom(src => src.ASSOCIATO.HasValue ? src.ASSOCIATO.ToString() : null))
                    .ForMember(dest => dest.chaRF, opt => opt.MapFrom(src => src.Registro.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, opt => opt.MapFrom(src => src.Registro.CHA_DISABILITATO ?? string.Empty))
                    .ForMember(dest => dest.Sospeso, opt => opt.MapFrom(src => src.Registro.CHA_DISABILITATO == "1" || src.Registro.CHA_STATO == "S" ? true : false))
                    .ForMember(dest => dest.idAOOCollegata, opt => opt.MapFrom(src => src.Registro.ID_AOO_COLLEGATA != null ? src.Registro.ID_AOO_COLLEGATA.ToString() : string.Empty))
                    .ForMember(dest => dest.Stato, opt => opt.MapFrom(src => src.Registro.CHA_STATO));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected class RegistroAssociatoEntity
        {
            public RegistroEntity Registro { get; set; }

            public long? ASSOCIATO { get; set; }
        }
        #endregion
    }
}
