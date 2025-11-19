// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
using DocsPaVO.DatiCert;
using DocsPaVO.utente;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AmmGetRagioneTrasmissioneRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmGetRagioneTrasmissione;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmGetRagioneTrasmissione
{

    public class AmmGetRagioneTrasmissioneHandler : IRequestHandler<AmmGetRagioneTrasmissioneRequest, AmmGetRagioneTrasmissioneResult>
    {
        #region Public Members

        public AmmGetRagioneTrasmissioneHandler(ILogger<AmmGetRagioneTrasmissioneHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IRagioneTrasmissioneRepository ragioneTrasmissioneRepository,
            IDistributedCache distributedCache,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._ragioneTrasmissioneRepository = ragioneTrasmissioneRepository;
            this._distributedCache = distributedCache;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<AmmGetRagioneTrasmissioneResult> Handle(AmmGetRagioneTrasmissioneRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true);
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            var aggregate = await _ragioneTrasmissioneRepository.Get(idTenant!, request.idRagione);

            OrgRagioneTrasmissione output = _mapper.Map<OrgRagioneTrasmissione>(aggregate);
            if(output != null)
            {
                var idRagione = output.ID.AsLong();
                var amministraEntities = await this._distributedCache.FromCache(instance!, this._dbContext.AmministraEntities);
                output.RagionePredefinitaDestinatari = amministraEntities.Any(a => a.SYSTEM_ID == idTenantAsLong && a.ID_RAGIONE_TO == idRagione);
                output.RagionePredefinitaDestinatariCC = amministraEntities.Any(a => a.SYSTEM_ID == idTenantAsLong && a.ID_RAGIONE_CC == idRagione);
            }

            return new AmmGetRagioneTrasmissioneResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmGetRagioneTrasmissioneHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IRagioneTrasmissioneRepository _ragioneTrasmissioneRepository;
        protected readonly IDistributedCache _distributedCache;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RagioneTrasmissione, OrgRagioneTrasmissione>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.Id))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.Name))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.Description))
                    .ForMember(dest => dest.Tipo, src => src.MapFrom(opt => opt.Opzioni.TipoRagione))
                    .ForMember(dest => dest.Visibilita, src => src.MapFrom(opt => opt.Opzioni.Visible))
                    .ForMember(dest => dest.TipoDiritto, src => src.MapFrom(opt => opt.Opzioni.TipoDirittoDestinatari))
                    .ForMember(dest => dest.TipoDestinatario, src => src.MapFrom(opt => opt.Opzioni.TipoDestinatario))
                    .ForMember(dest => dest.Risposta, src => src.MapFrom(opt => opt.Opzioni.Risposta))
                    .ForMember(dest => dest.Eredita, src => src.MapFrom(opt => opt.Opzioni.EstendiSuperioriGerarchici))
                    .ForMember(dest => dest.IDAmministrazione, src => src.MapFrom(opt => opt.IdTenant))
                    .ForMember(dest => dest.testoMsgNotificaDoc, src => src.MapFrom(opt => opt.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaDocumenti))
                    .ForMember(dest => dest.testoMsgNotificaFasc, src => src.MapFrom(opt => opt.Opzioni.MessaggioNotificaTrasmissione.TestoPerNotificaFascicoli))
                    .ForMember(dest => dest.MantieniLettura, src => src.MapFrom(opt => opt.Opzioni.MantieneDiritti == TipiDirittiTrasmissioneEnum.Lettura || opt.Opzioni.MantieneDiritti == TipiDirittiTrasmissioneEnum.Scrittura))
                    .ForMember(dest => dest.MantieniScrittura, src => src.MapFrom(opt => opt.Opzioni.MantieneDiritti == TipiDirittiTrasmissioneEnum.Scrittura))
                    .ForMember(dest => dest.TipoNotifica, src => src.MapFrom(opt => ParseTipoNotifica(opt.Opzioni.NotificaTramissionePerEmail)))
                    .ForMember(dest => dest.PrevedeRisposta, src => src.MapFrom(opt => opt.Opzioni.PrevedeRisposta))
                    .ForMember(dest => dest.PrevedeCessione, src => src.MapFrom(opt => opt.Opzioni.TipoCessioneDiritto))
                    .ForMember(dest => dest.DiSistema, src => src.MapFrom(opt => opt.Opzioni.RagioneDiSistema ? OrgRagioneTrasmissione.RagioneDiSistemaEnum.Si : OrgRagioneTrasmissione.RagioneDiSistemaEnum.No));
            });

            _mapper = configuration.CreateMapper();
        }

        protected static OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum ParseTipoNotifica(OpzioniNotificaTrasmissionePerEmail tipoNotifica)
        {
            OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum retValue = OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum.Nessuna;

            if (tipoNotifica.IncludiLinkSchedaDettaglio && tipoNotifica.IncludiLinkImmagineDocumento && tipoNotifica.AllegaDocumenti)
                retValue = OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum.MailAllegati;

            if (tipoNotifica.IncludiLinkSchedaDettaglio && tipoNotifica.IncludiLinkImmagineDocumento && !tipoNotifica.AllegaDocumenti)
                retValue = OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum.Mail;

            if (!tipoNotifica.IncludiLinkSchedaDettaglio && !tipoNotifica.IncludiLinkImmagineDocumento && tipoNotifica.AllegaDocumenti)
                retValue = OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum.MailSoloAllegati;

            if (!tipoNotifica.IncludiLinkSchedaDettaglio && !tipoNotifica.IncludiLinkImmagineDocumento && !tipoNotifica.AllegaDocumenti)
                retValue = OrgRagioneTrasmissione.TipiNotificaTrasmissioneEnum.NonNotificareMai;

            return retValue;
        }
        #endregion
    }
}
