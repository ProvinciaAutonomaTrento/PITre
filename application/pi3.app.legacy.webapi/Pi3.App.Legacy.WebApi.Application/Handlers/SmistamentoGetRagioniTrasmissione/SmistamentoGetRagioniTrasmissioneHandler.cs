// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
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
using SmistamentoGetRagioniTrasmissioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.SmistamentoGetRagioniTrasmissione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.SmistamentoGetRagioniTrasmissione
{

    // Richiede libreria MediatR
    public class SmistamentoGetRagioniTrasmissioneHandler : IRequestHandler<SmistamentoGetRagioniTrasmissioneRequest, SmistamentoGetRagioniTrasmissioneResult>
    {
        #region Public Members

        public SmistamentoGetRagioniTrasmissioneHandler(ILogger<SmistamentoGetRagioniTrasmissioneHandler> logger,
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

        public async Task<SmistamentoGetRagioniTrasmissioneResult> Handle(SmistamentoGetRagioniTrasmissioneRequest request, CancellationToken cancellationToken)
        {
            RagioneTrasmissione[] output = null;

            try
            {
                var idAmmAsLong = request.idAmm.AsLong();

                var idRagioni = await this._dbContext.AmministraEntities.AsNoTracking()
                    .Where(a => a.SYSTEM_ID == idAmmAsLong)
                    .Select(a => new
                    {
                        a.ID_RAGIONE_COMPETENZA,
                        a.ID_RAGIONE_CONOSCENZA
                    })
                    .FirstOrDefaultAsync();

                if(idRagioni != null && idRagioni.ID_RAGIONE_COMPETENZA.HasValue && idRagioni.ID_RAGIONE_CONOSCENZA.HasValue)
                {
                    var ragioneTrasmissioneEntity= await this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                        .Where(r => r.SYSTEM_ID == idRagioni.ID_RAGIONE_COMPETENZA || r.SYSTEM_ID == idRagioni.ID_RAGIONE_CONOSCENZA)
                        .ToListAsync();
                    
                    output = this._mapper.Map<RagioneTrasmissione[]>(ragioneTrasmissioneEntity);

                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(exception: ex, message: ex.Message);
                output = null;
            }

            return new SmistamentoGetRagioniTrasmissioneResult(output);

        }

        #endregion

        #region Private Members

        protected readonly ILogger<SmistamentoGetRagioniTrasmissioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RagioneTrasmissioneEntity, RagioneTrasmissione>()
                     .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_RAGIONE))
                     .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => src.CHA_TIPO_RAGIONE))
                     .ForMember(dest => dest.tipoDiritti, src => src.MapFrom(opt => RagioneTrasmissione.tipoDirittoStringa.Keys.OfType<TipoDiritto>().FirstOrDefault(s => RagioneTrasmissione.tipoDirittoStringa[s].Equals(opt.CHA_TIPO_DIRITTI))))
                     .ForMember(dest => dest.tipoDestinatario, src => src.MapFrom(opt => RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<TipoGerarchia>().FirstOrDefault(s => RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(opt.CHA_TIPO_DEST))))
                     .ForMember(dest => dest.risposta, opt => opt.MapFrom(src => src.CHA_RISPOSTA))
                     .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.VAR_NOTE))
                     .ForMember(dest => dest.eredita, opt => opt.MapFrom(src => src.CHA_EREDITA))
                     .ForMember(dest => dest.tipoRisposta, opt => opt.MapFrom(src => src.CHA_TIPO_RISPOSTA))
                     .ForMember(dest => dest.prevedeCessione, opt => opt.MapFrom(src => src.CHA_CEDE_DIRITTI));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
