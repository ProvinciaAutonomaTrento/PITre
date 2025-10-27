// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
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

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneGetRagioniATutti
{
    // Richiede libreria MediatR
    public class TrasmissioneGetRagioniATuttiHandler : IRequestHandler<Application.Requests.TrasmissioneGetRagioniATutti, TrasmissioneGetRagioniATuttiResult>
    {
        #region Public Members

        public TrasmissioneGetRagioniATuttiHandler(ILogger<TrasmissioneGetRagioniATuttiHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;

            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<TrasmissioneGetRagioniATuttiResult> Handle(Application.Requests.TrasmissioneGetRagioniATutti request, CancellationToken cancellationToken)
        {
            DocsPaVO.trasmissione.Diritti objDiritti = request.diritti;
            long idAmmAsLong = objDiritti.idAmministrazione.AsLong();
            DocsPaVO.trasmissione.RagioneTrasmissione[] result = null;
            List<string> tipoDestList = new List<string> { "T", "P" };

            try
            {
                var ragioni = this._dbContext.RagioneTrasmissioneEntities
                    .Where(x => x.CHA_VIS.Equals("1") && x.ID_AMM == idAmmAsLong && tipoDestList.Contains(x.CHA_TIPO_DEST))
                    .OrderBy(x => x.VAR_NOTE);

                result = this._mapper.Map<DocsPaVO.trasmissione.RagioneTrasmissione[]>(ragioni);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new TrasmissioneGetRagioniATuttiResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneGetRagioniATuttiHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RagioneTrasmissioneEntity, DocsPaVO.trasmissione.RagioneTrasmissione>()
                     .ForMember(dest => dest.systemId, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.descrizione, opt => opt.MapFrom(src => src.VAR_DESC_RAGIONE))
                     .ForMember(dest => dest.tipo, opt => opt.MapFrom(src => src.CHA_TIPO_RAGIONE))
                     .ForMember(dest => dest.tipoDiritti, opt => opt.MapFrom(src => src.CHA_TIPO_DIRITTI.Equals("R") ? DocsPaVO.trasmissione.TipoDiritto.READ : (src.CHA_TIPO_DIRITTI.Equals("W") ?DocsPaVO.trasmissione.TipoDiritto.WRITE : DocsPaVO.trasmissione.TipoDiritto.NONE)))
                     .ForMember(dest => dest.risposta, opt => opt.MapFrom(src => src.CHA_RISPOSTA))
                     .ForMember(dest => dest.tipoDestinatario, opt => opt.MapFrom(src => GetTipoGerarchia(src.CHA_TIPO_DEST)))
                     .ForMember(dest => dest.note, opt => opt.MapFrom(src => src.VAR_NOTE))
                     .ForMember(dest => dest.eredita, opt => opt.MapFrom(src => src.CHA_EREDITA))
                     .ForMember(dest => dest.tipoRisposta, opt => opt.MapFrom(src => src.CHA_TIPO_RISPOSTA))
                     .ForMember(dest => dest.notifica, opt => opt.MapFrom(src => src.VAR_NOTIFICA_TRASM))
                     .ForMember(dest => dest.testoMsgNotificaDoc, opt => opt.MapFrom(src => src.VAR_TESTO_MSG_NOTIFICA_DOC))
                     .ForMember(dest => dest.testoMsgNotificaFasc, opt => opt.MapFrom(src => src.VAR_TESTO_MSG_NOTIFICA_FASC))
                     .ForMember(dest => dest.prevedeCessione, opt => opt.MapFrom(src => src.CHA_CEDE_DIRITTI ?? "N"))
                     .ForMember(dest => dest.mantieniLettura, opt => opt.MapFrom(src => src.CHA_MANTIENI_LETT ?? "false"))
                     .ForMember(dest => dest.mantieniScrittura, opt => opt.MapFrom(src => src.CHA_MANTIENI_SCRITT ?? "false"));
            });

            _mapper = configuration.CreateMapper();
        }

        private TipoGerarchia GetTipoGerarchia(string tipoDest)
        {
            switch (tipoDest)
            {
                case "I":
                    return TipoGerarchia.INFERIORE;
                    break;
                case "S":
                    return TipoGerarchia.SUPERIORE;
                    break;
                case "T":
                    return TipoGerarchia.TUTTI;
                    break;
                case "P":
                    return TipoGerarchia.PARILIVELLO;
                    break;
                default:
                    return TipoGerarchia.TUTTI;
                    break;
            }
        }

        #endregion
    }

}
