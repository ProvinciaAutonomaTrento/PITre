// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.areaConservazione;
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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ConservazioneGetInfoByFiltroRequest = Pi3.App.Legacy.WebApi.Application.Requests.ConservazioneGetInfoByFiltro;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.ConservazioneGetInfoByFiltro
{

    // Richiede libreria MediatR
    public class ConservazioneGetInfoByFiltroHandler : IRequestHandler<ConservazioneGetInfoByFiltroRequest, ConservazioneGetInfoByFiltroResult>
    {
        #region Public Members

        public ConservazioneGetInfoByFiltroHandler(ILogger<ConservazioneGetInfoByFiltroHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<ConservazioneGetInfoByFiltroResult> Handle(ConservazioneGetInfoByFiltroRequest request, CancellationToken cancellationToken)
        {
            var idTenant = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var idPeople = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var idGroup = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);
            var surname = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.UserSurname);
            var name = _claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.UserName);

            var idRuoloInUO = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGroup).Select(c => c.SYSTEM_ID).FirstAsync();
            var descPeople = string.Format("{0} {1}", surname, name);

            var areaConservazioneEntity = await this._dbContext.AreaConservazioneEntities.AsNoTracking().
                Where(a => a.ID_AMM == idTenant && a.ID_PEOPLE == idPeople && a.ID_RUOLO_IN_UO == idRuoloInUO).OrderBy(a => a.DATA_APERTURA).ToListAsync();

            var output = _mapper.Map<InfoConservazione[]>(areaConservazioneEntity);

            foreach (var a in output)
                a.userID = descPeople;

            return new ConservazioneGetInfoByFiltroResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<ConservazioneGetInfoByFiltroHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AreaConservazioneEntity, InfoConservazione>()
                    .ForMember(dest => dest.SystemID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.IdAmm, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.IdPeople, src => src.MapFrom(opt => opt.ID_PEOPLE))
                    .ForMember(dest => dest.IdRuoloInUo, src => src.MapFrom(opt => opt.ID_RUOLO_IN_UO))
                    .ForMember(dest => dest.StatoConservazione, src => src.MapFrom(opt => opt.CHA_STATO))
                    .ForMember(dest => dest.TipoSupporto, src => src.MapFrom(opt => opt.VAR_TIPO_SUPPORTO))
                    .ForMember(dest => dest.Note, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => opt.VAR_DESCRIZIONE))
                    .ForMember(dest => dest.Data_Apertura, src => src.MapFrom(opt => opt.DATA_APERTURA.AsDateTimeFormat()))
                    .ForMember(dest => dest.Data_Invio, src => src.MapFrom(opt => opt.DATA_INVIO.AsDateTimeFormat()))
                    .ForMember(dest => dest.Data_Conservazione, src => src.MapFrom(opt => opt.DATA_CONSERVAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.MarcaTemporale, src => src.MapFrom(opt => opt.VAR_MARCA_TEMPORALE))
                    .ForMember(dest => dest.FirmaResponsabile, src => src.MapFrom(opt => opt.VAR_FIRMA_RESPONSABILE))
                    .ForMember(dest => dest.LocazioneFisica, src => src.MapFrom(opt => opt.VAR_LOCAZIONE_FISICA))
                    .ForMember(dest => dest.Data_Prox_Verifica, src => src.MapFrom(opt => opt.DATA_PROX_VERIFICA.AsDateTimeFormat()))
                    .ForMember(dest => dest.Data_Ultima_Verifica, src => src.MapFrom(opt => opt.DATA_ULTIMA_VERIFICA.AsDateTimeFormat()))
                    .ForMember(dest => dest.Data_Riversamento, src => src.MapFrom(opt => opt.DATA_RIVERSAMENTO.AsDateTimeFormat()))
                    .ForMember(dest => dest.TipoConservazione, src => src.MapFrom(opt => opt.VAR_TIPO_CONS))
                    .ForMember(dest => dest.numCopie, src => src.MapFrom(opt => opt.COPIE_SUPPORTI))
                    .ForMember(dest => dest.noteRifiuto, src => src.MapFrom(opt => opt.VAR_NOTE_RIFIUTO))                   
                    .ForMember(dest => dest.formatoDoc, src => src.MapFrom(opt => opt.VAR_FORMATO_DOC))
                    .ForMember(dest => dest.IdGruppo, src => src.MapFrom(opt => opt.ID_GRUPPO))
                    .ForMember(dest => dest.validationMask, src => src.MapFrom(opt => opt.VALIDATION_MASK))
                    .ForMember(dest => dest.decrSupporto, src => src.MapFrom(opt => opt.VAR_TIPO_SUPPORTO))
                    .ForMember(dest => dest.esitoVerifica, src => src.MapFrom(opt => opt.ESITO_VERIFICA == null ? 0 : opt.ESITO_VERIFICA))
                    .ForMember(dest => dest.automatica, src => src.MapFrom(opt => opt.ID_POLICY == null ? "A" : "M"))
                    .ForMember(dest => dest.consolida, src => src.MapFrom(opt => opt.CONSOLIDA == "1"))
                    .ForMember(dest => dest.idPolicyValidata, src => src.MapFrom(opt => opt.ID_POLICY_VALIDAZIONE))
                    .ForMember(dest => dest.predefinita, src => src.MapFrom(opt => opt.CHA_STATO == "N" && opt.IS_PREFERRED == "1"))              
                    .ForMember(dest => dest.IstanzaInPreparazione, src => src.MapFrom(opt => false));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
