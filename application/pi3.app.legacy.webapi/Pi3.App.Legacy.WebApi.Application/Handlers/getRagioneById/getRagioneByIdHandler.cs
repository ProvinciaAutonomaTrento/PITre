// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.documento;
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getRagioneByIdRequest = Pi3.App.Legacy.WebApi.Application.Requests.getRagioneById;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getRagioneById
{
    public class getRagioneByIdHandler : IRequestHandler<getRagioneByIdRequest, getRagioneByIdResult>
    {
        #region Public Members

        public getRagioneByIdHandler(ILogger<getRagioneByIdHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;
            _distributedCache = distributedCache;

            InitializeMapper();
        }

        public async Task<getRagioneByIdResult> Handle(getRagioneByIdRequest request, CancellationToken cancellationToken)
        {
            RagioneTrasmissione output = null;

            try
            {
                var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

                long idRagione = request.idRagione.AsLong();
                var ragioni = await _distributedCache.FromCache(instance!, _dbContext.RagioneTrasmissioneEntities);

                var ragione = ragioni.Where(x => x.SYSTEM_ID == idRagione).FirstOrDefault();

                if (ragione == null)
                    throw new RagioneTrasmissioneNotFoundPi3Exception(request.idRagione);

                output = _mapper.Map<RagioneTrasmissione>(ragione);
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
            }

            return new getRagioneByIdResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getRagioneByIdHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
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
                    .ForMember(dest => dest.tipoRisposta, src => src.MapFrom(opt => opt.CHA_TIPO_RISPOSTA))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.VAR_NOTIFICA_TRASM))
                    .ForMember(dest => dest.testoMsgNotificaDoc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_DOC))
                    .ForMember(dest => dest.testoMsgNotificaFasc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_FASC))
                    .ForMember(dest => dest.prevedeCessione, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI != null ? opt.CHA_CEDE_DIRITTI : "N"))
                    .ForMember(dest => dest.mantieniLettura, src => src.MapFrom(opt => opt.CHA_MANTIENI_LETT != null ? opt.CHA_MANTIENI_LETT : "false"))
                    .ForMember(dest => dest.mantieniScrittura, src => src.MapFrom(opt => opt.CHA_MANTIENI_SCRITT));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
