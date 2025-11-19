// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
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
using GetRagioneNotificaRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetRagioneNotifica;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetRagioneNotifica
{
    public class GetRagioneNotificaHandler : IRequestHandler<GetRagioneNotificaRequest, GetRagioneNotificaResult>
    {
        #region Public Members

        public GetRagioneNotificaHandler(ILogger<GetRagioneNotificaHandler> logger,
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

        public async Task<GetRagioneNotificaResult> Handle(GetRagioneNotificaRequest request, CancellationToken cancellationToken)
        {
            RagioneTrasmissione output = null;

            try
            {
                var idAmmAsLong = request.idAmm.AsLong();
                var ragioneEntity = await this._dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                    .Where(r => r.CHA_TIPO_DIRITTI == "N" && r.VAR_DESC_RAGIONE == "NOTIFICA" && r.ID_AMM == idAmmAsLong)
                    .FirstOrDefaultAsync();

                if(ragioneEntity != null)
                    output = this._mapper.Map<RagioneTrasmissione>(ragioneEntity);

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new GetRagioneNotificaResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetRagioneNotificaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RagioneTrasmissioneEntity, RagioneTrasmissione>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_RAGIONE))
                    .ForMember(dest => dest.tipo, src => src.MapFrom(opt => opt.CHA_TIPO_RAGIONE))
                    .ForMember(dest => dest.tipoDiritti, src => src.MapFrom(opt => RagioneTrasmissione.tipoDirittoStringa.Keys.OfType<DocsPaVO.trasmissione.TipoDiritto>().FirstOrDefault(s => RagioneTrasmissione.tipoDirittoStringa[s].Equals(opt.CHA_TIPO_DIRITTI))))
                    .ForMember(dest => dest.risposta, src => src.MapFrom(opt => opt.CHA_RISPOSTA))
                    .ForMember(dest => dest.tipoDestinatario, src => src.MapFrom(opt => RagioneTrasmissione.tipoGerarchiaStringa.Keys.OfType<TipoGerarchia>().FirstOrDefault(s => RagioneTrasmissione.tipoGerarchiaStringa[s].Equals(opt.CHA_TIPO_DEST))))
                    .ForMember(dest => dest.note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.eredita, src => src.MapFrom(opt => opt.CHA_EREDITA))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.VAR_NOTIFICA_TRASM))
                    .ForMember(dest => dest.tipoRisposta, src => src.MapFrom(opt => opt.VAR_DESC_RAGIONE != null ? opt.CHA_TIPO_RISPOSTA : null))
                    .ForMember(dest => dest.prevedeCessione, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI));
            });

            this._mapper = configuration.CreateMapper();
        }

        #endregion
    }
}
