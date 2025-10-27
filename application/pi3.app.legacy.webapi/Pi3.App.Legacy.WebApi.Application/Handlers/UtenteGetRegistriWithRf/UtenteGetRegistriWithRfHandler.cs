// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.areaConservazione;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtenteGetRegistriWithRfRequest = Pi3.App.Legacy.WebApi.Application.Requests.UtenteGetRegistriWithRf;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.UtenteGetRegistriWithRf
{
    public class UtenteGetRegistriWithRfHandler : IRequestHandler<UtenteGetRegistriWithRfRequest, UtenteGetRegistriWithRfResult>
    {
        #region Public Members

        public UtenteGetRegistriWithRfHandler(ILogger<UtenteGetRegistriWithRfHandler> logger,
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

        public async Task<UtenteGetRegistriWithRfResult> Handle(UtenteGetRegistriWithRfRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.Registro[] output = null;

            try
            {
                long idCorrGlobali = request.idCorrGlobali.AsLong();

                var queryable = _dbContext.RegistroEntities
                    .Join(
                        _dbContext.RuoloRegistroEntities,
                        registro => registro.SYSTEM_ID,
                        ruolo => ruolo.ID_REGISTRO,
                        (registro, ruolo) => new
                        {
                            registro,
                            ruolo.CHA_PREFERITO,
                            ruolo.ID_RUOLO_IN_UO,
                            ruolo.CHA_PROTOCOLLO_ABILITATO
                        }
                    )
                    .Where(x => x.ID_RUOLO_IN_UO == idCorrGlobali);


                //Estraggo solo gli RF con protocolloAbilitato attivo(MEV RF creati SOLO come RF di rubrica e non di Protocollazione)
                if (request.protocolloAbilitato)
                    queryable = queryable.Where(x => x.CHA_PROTOCOLLO_ABILITATO == "1");

                var registro_entity = queryable
                    .OrderBy(x => x.registro.CHA_STATO)
                    .ThenByDescending(x => x.CHA_PREFERITO)
                    .ThenBy(x => x.registro.VAR_CODICE)
                    .Select(x => x.registro);

                if (!string.IsNullOrEmpty(request.all))
                {
                    registro_entity = registro_entity.Where(x => x.CHA_RF.Equals(request.all));
                    if (!string.IsNullOrEmpty(request.idAooColl) && request.all.Equals("1"))
                    {
                        long id_aoo_collegata = request.idAooColl.AsLong();
                        registro_entity = registro_entity.Where(x => x.ID_AOO_COLLEGATA == id_aoo_collegata);
                    }
                }

                output = _mapper.Map<DocsPaVO.utente.Registro[]>(registro_entity);

                if (output != null && output.Any())
                {
                    long id_amministrazione = output[0].idAmministrazione.AsLong();
                    var codiceAmministrazione = await this._dbContext.AmministraEntities.AsNoTracking()
                        .Where(a => a.SYSTEM_ID == id_amministrazione)
                        .Select(a => a.VAR_CODICE_AMM)
                        .FirstAsync();

                    foreach (DocsPaVO.utente.Registro registro in output)
                        registro.codAmministrazione = codiceAmministrazione;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new UtenteGetRegistriWithRfResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<UtenteGetRegistriWithRfHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDistributedCache _distributedCache;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.codRegistro, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.NUM_RIF))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_REGISTRO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL_REGISTRO))
                    .ForMember(dest => dest.stato, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.dataApertura, src => src.MapFrom(opt => opt.DTA_OPEN.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.dataChiusura, src => src.MapFrom(opt => opt.DTA_CLOSE.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.dataUltimoProtocollo, src => src.MapFrom(opt => opt.DTA_ULTIMO_PROTO.AsDateFormat() ?? string.Empty))
                    .ForMember(dest => dest.idRuoloAOO, src => src.MapFrom(opt => opt.ID_RUOLO_AOO))
                    .ForMember(dest => dest.idRuoloResp, src => src.MapFrom(opt => opt.ID_RUOLO_RESP))
                    .ForMember(dest => dest.idUtenteAOO, src => src.MapFrom(opt => opt.ID_PEOPLE_AOO))
                    .ForMember(dest => dest.autoInterop, src => src.MapFrom(opt => opt.CHA_AUTO_INTEROP ?? string.Empty))
                    .ForMember(dest => dest.chaRF, src => src.MapFrom(opt => opt.CHA_RF))
                    .ForMember(dest => dest.rfDisabled, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) ? opt.CHA_DISABILITATO : "0"))
                    .ForMember(dest => dest.Sospeso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.CHA_DISABILITATO) && opt.CHA_DISABILITATO.Equals("1") ? true : false))
                    .ForMember(dest => dest.idAOOCollegata, src => src.MapFrom(opt => opt.ID_AOO_COLLEGATA))
                    .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE))
                    .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => !string.IsNullOrEmpty(opt.VAR_PREG) && opt.VAR_PREG.Equals("1") ? true : false));
            });
            _mapper = configuration.CreateMapper();
        }
        #endregion
    }

}
