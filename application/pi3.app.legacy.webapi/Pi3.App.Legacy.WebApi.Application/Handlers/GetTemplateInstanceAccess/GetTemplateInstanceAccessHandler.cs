// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetTemplateInstanceAccess
{
    // Richiede libreria MediatR
    public class GetTemplateInstanceAccessHandler : IRequestHandler<Application.Requests.GetTemplateInstanceAccess, GetTemplateInstanceAccessResult>
    {   
        #region Public Members
        public GetTemplateInstanceAccessHandler(ILogger<GetTemplateInstanceAccessHandler> logger, IClaimsPrincipalService claimsPrincipalService, IMediator mediator, IPi3DbContext dbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;

            this.InitializeMapper();
        }

        public async Task<GetTemplateInstanceAccessResult> Handle(Application.Requests.GetTemplateInstanceAccess request, CancellationToken cancellationToken)
        {
            DocsPaVO.ProfilazioneDinamica.Templates result = null;
            var idTenantAsLong = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);

            try
            {
                var template = await _dbContext.TipoAttoEntities.Where(x => !string.IsNullOrEmpty(x.IS_TYPE_INSTANCE) && x.IS_TYPE_INSTANCE.Equals("1") && x.ID_AMM == idTenantAsLong).FirstOrDefaultAsync();

                result = this._mapper.Map<DocsPaVO.ProfilazioneDinamica.Templates>(template);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
            }

            return new GetTemplateInstanceAccessResult(result);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetTemplateInstanceAccessHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TipoAttoEntity, DocsPaVO.ProfilazioneDinamica.Templates>()
                     .ForMember(dest => dest.SYSTEM_ID, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.ID_TIPO_ATTO, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.DESCRIZIONE, opt => opt.MapFrom(src => src.VAR_DESC_ATTO))
                     .ForMember(dest => dest.ABILITATO_SI_NO, opt => opt.MapFrom(src => src.ABILITATO_SI_NO))
                     .ForMember(dest => dest.IN_ESERCIZIO, opt => opt.MapFrom(src => src.IN_ESERCIZIO))
                     .ForMember(dest => dest.PATH_MODELLO_1, opt => opt.MapFrom(src => src.PATH_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2, opt => opt.MapFrom(src => src.PATH_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_1_EXT, opt => opt.MapFrom(src => src.EXT_MOD_1))
                     .ForMember(dest => dest.PATH_MODELLO_2_EXT, opt => opt.MapFrom(src => src.EXT_MOD_2))
                     .ForMember(dest => dest.PATH_MODELLO_STAMPA_UNIONE, opt => opt.MapFrom(src => src.PATH_MOD_SU))
                     .ForMember(dest => dest.PATH_MODELLO_EXCEL, opt => opt.MapFrom(src => src.PATH_MOD_EXC))
                     .ForMember(dest => dest.PATH_XSD_ASSOCIATO, opt => opt.MapFrom(src => src.PATH_XSD_ASSOCIATO))
                     .ForMember(dest => dest.PATH_ALLEGATO_1, opt => opt.MapFrom(src => src.PATH_ALL_1))
                     .ForMember(dest => dest.SCADENZA, opt => opt.MapFrom(src => src.GG_SCADENZA))
                     .ForMember(dest => dest.PRE_SCADENZA, opt => opt.MapFrom(src => src.GG_PRE_SCADENZA))
                     .ForMember(dest => dest.PRIVATO, opt => opt.MapFrom(src => src.CHA_PRIVATO ?? "0"))
                     .ForMember(dest => dest.ID_AMMINISTRAZIONE, opt => opt.MapFrom(src => src.ID_AMM))
                     .ForMember(dest => dest.CODICE_CLASSIFICA, opt => opt.MapFrom(src => src.COD_CLASS))
                     .ForMember(dest => dest.CODICE_MODELLO_TRASM, opt => opt.MapFrom(src => src.COD_MOD_TRASM))
                     .ForMember(dest => dest.IPER_FASC_DOC, opt => opt.MapFrom(src => src.IPERDOCUMENTO == 1 ? "1" : "0"));
            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
