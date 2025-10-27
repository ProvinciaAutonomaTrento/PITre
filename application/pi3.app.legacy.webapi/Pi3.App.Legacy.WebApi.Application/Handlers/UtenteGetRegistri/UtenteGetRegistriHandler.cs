// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtenteGetRegistriRequest = Pi3.App.Legacy.WebApi.Application.Requests.UtenteGetRegistri;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UtenteGetRegistri
{
    public class UtenteGetRegistriHandler : IRequestHandler<UtenteGetRegistriRequest, UtenteGetRegistriResult>
    {
        #region Public Members

        public UtenteGetRegistriHandler(ILogger<UtenteGetRegistriHandler> logger,
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

        public async Task<UtenteGetRegistriResult> Handle(UtenteGetRegistriRequest request, CancellationToken cancellationToken)
        {
            Registro[] output = null;

            var instance = this._claimsPrincipalService.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.Instance, true);

            try
            {
                long idCorrGlobali = Convert.ToInt64(request.idCorrGlobali);

                var registro_entity = await _dbContext.RegistroEntities
                    .Join(
                        _dbContext.RuoloRegistroEntities,
                        registro => registro.SYSTEM_ID,
                        ruolo => ruolo.ID_REGISTRO,
                        (registro, ruolo) => new
                        {
                            registro,
                            ruolo.CHA_PREFERITO,
                            ruolo.ID_RUOLO_IN_UO
                        }
                    )
                    .Where(x => x.ID_RUOLO_IN_UO == idCorrGlobali && x.registro.CHA_RF == "0")
                    .OrderByDescending(x => x.CHA_PREFERITO == "1")
                    .ThenBy(x => x.registro.VAR_PREG == "1")
                    .ThenBy(x => x.registro.VAR_CODICE)
                    .ThenBy(x => x.registro.VAR_DESC_REGISTRO)
                    .Select(x => x.registro)
                    .ToListAsync();

                output = _mapper.Map<Registro[]>(registro_entity);

                long id_amministrazione = Convert.ToInt64(output[0].idAmministrazione);
                var amministrazioneEntity = await _distributedCache.FromCache(instance!, _dbContext.AmministraEntities, a => a.SYSTEM_ID == id_amministrazione);
                foreach (Registro registro in output)
                    registro.codAmministrazione = amministrazioneEntity[0].VAR_CODICE_AMM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new UtenteGetRegistriResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UtenteGetRegistriHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroEntity, Registro>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateFormat()))
                    .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateFormat()))
                    .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateFormat()))
                    .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP))
                    .ForMember(dest => dest.Diritto_Ruolo_AOO, src => src.MapFrom(opt => opt.DIRITTO_RUOLO_AOO))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) ? opt.CHA_DISABILITATO : "0"))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) && opt.CHA_DISABILITATO.Equals("1") ? false : true))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE != null && opt.INVIO_RICEVUTA_MANUALE.ToString() == "0" ? "0" : "1"))
                    .ForMember(dest => dest.FlagWspia, src => src.MapFrom(opt => opt.FLAG_WSPIA != null ? opt.FLAG_WSPIA : "0"))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.VAR_PREG) && opt.VAR_PREG.Equals("1") ? true : false))
                    .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.VAR_PREG) ? opt.ANNO_PREG : null));
            });
            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
