// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using AutoMapper.Execution;
using DocsPaVO.amministrazione;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getTitolariUtilizzabiliRequest = Pi3.App.Legacy.WebApi.Application.Requests.getTitolariUtilizzabili;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getTitolariUtilizzabili
{

    public class getTitolariUtilizzabiliHandler : IRequestHandler<getTitolariUtilizzabiliRequest, getTitolariUtilizzabiliResult>
    {
        #region Public Members

        public getTitolariUtilizzabiliHandler(
            ILogger<getTitolariUtilizzabiliHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext)
        {
            _logger = logger;
            _claimsPrincipalService = claimsPrincipalService;
            _mediator = mediator;
            _dbContext = dbContext;

            InitializeMapper();
        }

        public async Task<getTitolariUtilizzabiliResult> Handle(getTitolariUtilizzabiliRequest request, CancellationToken cancellationToken)
        {
            OrgTitolario[] output = null;

            try
            {
                long id_amministrazione = Convert.ToInt64(request.idAmministrazione);
                var entities = from p in _dbContext.ProjectEntities
                                join a in _dbContext.AmministraEntities
                                on p.ID_AMM equals a.SYSTEM_ID
                                where p.ID_AMM == id_amministrazione && p.ID_PARENT == 0 && p.CHA_STATO != "D" && p.ID_TITOLARIO == 0
                                orderby p.CHA_STATO, p.DTA_CESSAZIONE descending
                                select new { p, a.VAR_CODICE_AMM }
                ;

                var codice_amministrazione = entities.Select(x => x.VAR_CODICE_AMM).FirstOrDefault();
                output = _mapper.Map<OrgTitolario[]>(entities.Select(x => x.p));
                foreach (OrgTitolario o in output)
                    o.CodiceAmministrazione = codice_amministrazione;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
            }

            return new getTitolariUtilizzabiliResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getTitolariUtilizzabiliHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ProjectEntity, OrgTitolario>()
                    .ForMember(dest => dest.ID, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.Codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.Commento, src => src.MapFrom(opt => opt.VAR_NOTE))
                    .ForMember(dest => dest.DataAttivazione, src => src.MapFrom(opt => opt.DTA_ATTIVAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.DataCessazione, src => src.MapFrom(opt =>  opt.DTA_CESSAZIONE.AsDateTimeFormat()))
                    .ForMember(dest => dest.MaxLivTitolario, src => src.MapFrom(opt => opt.MAX_LIV_TIT))
                    .ForMember(dest => dest.EtichettaTit, src => src.MapFrom(opt => opt.ET_TITOLARIO))
                    .ForMember(dest => dest.EtichettaLiv1, src => src.MapFrom(opt => opt.ET_LIVELLO1))
                    .ForMember(dest => dest.EtichettaLiv2, src => src.MapFrom(opt => opt.ET_LIVELLO2))
                    .ForMember(dest => dest.EtichettaLiv3, src => src.MapFrom(opt => opt.ET_LIVELLO3))
                    .ForMember(dest => dest.EtichettaLiv4, src => src.MapFrom(opt => opt.ET_LIVELLO4))
                    .ForMember(dest => dest.EtichettaLiv5, src => src.MapFrom(opt => opt.ET_LIVELLO5))
                    .ForMember(dest => dest.EtichettaLiv6, src => src.MapFrom(opt => opt.ET_LIVELLO6))
                    .ForMember(dest => dest.Descrizione, src => src.MapFrom(opt => GetDescriptionTitolario(opt)))
                    .ForMember(dest => dest.Stato, src => src.MapFrom(opt => GetStateTitolario(opt)));
            });

            _mapper = configuration.CreateMapper();
        }

        private string GetDescriptionTitolario(ProjectEntity entity)
        {
            switch (entity.CHA_STATO)
            {
                case "D":
                    return entity.DESCRIPTION + Resources.TitolarioInDefinizione;
                case "A":
                    return entity.DESCRIPTION + Resources.TitolarioAttivo;
                case "C":
                    return entity.DESCRIPTION + string.Format(Resources.TitolarioInVigore, entity.DTA_ATTIVAZIONE.AsDateFormat(), entity.DTA_CESSAZIONE.AsDateFormat());
                default:
                    return "";
            }
        }
        private OrgStatiTitolarioEnum GetStateTitolario(ProjectEntity entity)
        {
            switch (entity.CHA_STATO)
            {
                case "D":
                    return OrgStatiTitolarioEnum.InDefinizione;
                case "A":
                    return OrgStatiTitolarioEnum.Attivo;
                case "C":
                    return OrgStatiTitolarioEnum.Chiuso;
                default:
                    return OrgStatiTitolarioEnum.InDefinizione;
            }
        }

        #endregion
    }

}
