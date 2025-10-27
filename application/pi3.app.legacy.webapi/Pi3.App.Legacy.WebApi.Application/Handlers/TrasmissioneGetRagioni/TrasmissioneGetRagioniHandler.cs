// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.trasmissione;
using MediatR;
using Microsoft.AspNetCore.Http;
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
using TrasmissioneGetRagioniRequest = Pi3.App.Legacy.WebApi.Application.Requests.TrasmissioneGetRagioni;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.TrasmissioneGetRagioni
{
    public class TrasmissioneGetRagioniHandler : IRequestHandler<TrasmissioneGetRagioniRequest, TrasmissioneGetRagioniResult>
    {
        #region Public Members

        public TrasmissioneGetRagioniHandler(ILogger<TrasmissioneGetRagioniHandler> logger,
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

        public async Task<TrasmissioneGetRagioniResult> Handle(TrasmissioneGetRagioniRequest request, CancellationToken cancellationToken)
        {
            RagioneTrasmissione[] output = null;

            try
            {
                var queryRagioneTrasm = this._dbContext.RagioneTrasmissioneEntities.Select(r => r);

                if (!request.flgDaRicercaTrasm)
                    queryRagioneTrasm = queryRagioneTrasm.Where(r => r.CHA_VIS == "1");

                if (request.diritti != null)
                {
                    if (!string.IsNullOrEmpty(request.diritti.idAmministrazione))
                    {
                        var idAmministrazione = request.diritti.idAmministrazione.AsLong();
                        queryRagioneTrasm = queryRagioneTrasm.Where(r => r.ID_AMM == idAmministrazione);
                    }

                    if (request.diritti.accessRights != null && request.diritti.accessRights.Equals("45"))
                    {
                        queryRagioneTrasm = queryRagioneTrasm.Where(r => r.CHA_TIPO_DIRITTI == "R" || r.CHA_TIPO_DIRITTI == "C");
                    }
                }
                var ragioneEntities = await queryRagioneTrasm.OrderBy(r => r.VAR_NOTE).ToListAsync();
                output = this._mapper.Map<RagioneTrasmissione[]>(ragioneEntities);
            }
            catch (Exception ex)
            {
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new TrasmissioneGetRagioniResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<TrasmissioneGetRagioniHandler> _logger;
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
                    .ForMember(dest => dest.tipoRisposta, src => src.MapFrom(opt => opt.CHA_TIPO_RISPOSTA))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.VAR_NOTIFICA_TRASM))
                    .ForMember(dest => dest.testoMsgNotificaDoc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_DOC))
                    .ForMember(dest => dest.testoMsgNotificaFasc, src => src.MapFrom(opt => opt.VAR_TESTO_MSG_NOTIFICA_FASC))
                    .ForMember(dest => dest.prevedeCessione, src => src.MapFrom(opt => opt.CHA_CEDE_DIRITTI != null ? opt.CHA_CEDE_DIRITTI : "N"))
                    .ForMember(dest => dest.mantieniLettura, src => src.MapFrom(opt => opt.CHA_MANTIENI_LETT != null ? opt.CHA_MANTIENI_LETT : "false"))
                    .ForMember(dest => dest.mantieniScrittura, src => src.MapFrom(opt => opt.CHA_MANTIENI_SCRITT));
            });

            this._mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
