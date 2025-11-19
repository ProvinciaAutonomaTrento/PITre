// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.amministrazione;
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
using AmmRicercaInOrgRequest = Pi3.App.Legacy.WebApi.Application.Requests.AmmRicercaInOrg;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.AmmRicercaInOrg
{
    public class AmmRicercaInOrgHandler : IRequestHandler<AmmRicercaInOrgRequest, AmmRicercaInOrgResult>
    {
        #region Public Members

        public AmmRicercaInOrgHandler(ILogger<AmmRicercaInOrgHandler> logger, 
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

        public async Task<AmmRicercaInOrgResult> Handle(AmmRicercaInOrgRequest request, CancellationToken cancellationToken)
        {
            List<OrgRisultatoRicerca> output = new List<OrgRisultatoRicerca>();
            var tipoURP = string.Empty;

            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant);
            var descrizione = !string.IsNullOrEmpty(request.descrizione) ? request.descrizione.ToUpper() : null;
            var codice = !string.IsNullOrEmpty(request.codice) ? request.codice.ToUpper() : null;

            switch (request.tipo)
            {
                case "U":
                    tipoURP = "U";
                    break;
                case "R":
                    tipoURP = "R";
                    break;
                case "PN":
                case "PC":
                    tipoURP = "P";
                    break;                
            }

            if(tipoURP == "P")
            {
               var query = this._dbContext.CorrGlobaliEntities.AsNoTracking()
               .Join(this._dbContext.PeopleEntities.AsNoTracking(), corr => corr.ID_PEOPLE, people => people.SYSTEM_ID, (corr, people) => new
               {
                   corr.SYSTEM_ID,
                   corr.VAR_COD_RUBRICA,
                   DESCRIZIONE = corr.VAR_COGNOME + " " + corr.VAR_NOME,
                   corr.ID_PEOPLE,
                   corr.CHA_TIPO_URP,
                   corr.CHA_TIPO_IE,
                   corr.ID_AMM,
                   corr.VAR_NOME,
                   corr.VAR_COGNOME,
                   people.MATRICOLA
               })
               .Join(this._dbContext.PeopleGroupEntities.AsNoTracking(), corr => corr.ID_PEOPLE, peopleGroups => peopleGroups.PEOPLE_SYSTEM_ID, (corr, peopleGroups) => new
               {
                   corr.SYSTEM_ID,
                   corr.VAR_COD_RUBRICA,
                   corr.DESCRIZIONE,
                   corr.ID_PEOPLE,
                   corr.CHA_TIPO_URP,
                   corr.CHA_TIPO_IE,
                   corr.ID_AMM,
                   corr.VAR_NOME,
                   corr.VAR_COGNOME,
                   corr.MATRICOLA,
                   GROUP_SYSTEM_ID = peopleGroups.GROUPS_SYSTEM_ID,
                   PEOPLE_GROUPS_DTA_FINE = peopleGroups.DTA_FINE
               })
               .Join(this._dbContext.GroupEntities.AsNoTracking(), corr => corr.GROUP_SYSTEM_ID, groups => groups.SYSTEM_ID, (corr, groups) => new
               {
                   corr,
                   groups.GROUP_NAME,
                   ID_GRUPPO = groups.SYSTEM_ID
               })
               .Where(c => c.corr.PEOPLE_GROUPS_DTA_FINE == null && c.corr.CHA_TIPO_URP == "P" && c.corr.CHA_TIPO_IE == "I" && c.corr.ID_AMM == idTenant);

                if (codice != null)
                    query = request.searchByCodeExact ? query.Where(c => c.corr.VAR_COD_RUBRICA.ToUpper() == codice) : query.Where(c => c.corr.VAR_COD_RUBRICA.ToUpper().Contains(codice));

                if (descrizione != null)
                {
                    query = request.tipo == "PN" ? query.Where(c => c.corr.VAR_NOME.ToUpper().Contains(descrizione)) : query.Where(c => c.corr.VAR_COGNOME.ToUpper().Contains(descrizione));
                }

                var persona = await query.Select(c => new
                {
                    CORR_GLOBALI = new CorrGlobaliEntity { SYSTEM_ID = c.corr.SYSTEM_ID, VAR_COD_RUBRICA = c.corr.VAR_COD_RUBRICA, VAR_DESC_CORR = c.corr.DESCRIZIONE, ID_PEOPLE = c.corr.ID_PEOPLE },                  
                    c.GROUP_NAME,
                    c.corr.MATRICOLA,
                    c.ID_GRUPPO
                })
                .ToListAsync();

                foreach (var c in persona)
                {
                    var org = this._mapper.Map<OrgRisultatoRicerca>(c.CORR_GLOBALI);
                    org.Tipo = request.tipo;
                    org.Matricola = c.MATRICOLA;
                    org.DescParent = c.GROUP_NAME;

                    var idCorrGlobaliGruppo = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(g => g.ID_GRUPPO == c.ID_GRUPPO).Select(g => g.SYSTEM_ID).FirstOrDefaultAsync();
                    if(idCorrGlobaliGruppo != null)
                        org.IDParent = idCorrGlobaliGruppo.ToString();

                    output.Add(org);
                }
            }

            if (tipoURP == "U")
            {
                var query = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.DTA_FINE == null && c.CHA_TIPO_URP == "U" && c.CHA_TIPO_IE == "I" && c.ID_AMM == idTenant);

                if (codice != null)
                    query = request.searchByCodeExact ? query.Where(c => c.VAR_COD_RUBRICA.ToUpper() == codice) : query.Where(c => c.VAR_COD_RUBRICA.ToUpper().Contains(codice));

                if (descrizione != null)
                    query = query.Where(c => c.VAR_DESC_CORR.ToUpper().Contains(descrizione));

                var uo = await query.Select(c => new CorrGlobaliEntity
                 {
                    SYSTEM_ID = c.SYSTEM_ID,
                    VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                    VAR_DESC_CORR = c.VAR_DESC_CORR,
                    ID_PARENT =c.ID_PARENT
                })
                .ToListAsync();

                foreach (var c in uo)
                {
                    var org = this._mapper.Map<OrgRisultatoRicerca>(c);
                    org.Tipo = request.tipo;
                    org.IDParent = c.ID_PARENT.ToString();
                    org.DescParent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(g => g.SYSTEM_ID == c.ID_PARENT).Select(g => g.VAR_DESC_CORR).FirstOrDefaultAsync();

                    output.Add(org);
                }
            }

            if(tipoURP == "R")
            {
                var query = this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.DTA_FINE == null && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I" && c.ID_AMM == idTenant && c.CHA_SYSTEM_ROLE != "1");

                if(!request.searchHistoricized)
                    query = query.Where(c => c.DTA_FINE == null);

                if (codice != null)
                    query = request.searchByCodeExact ? query.Where(c => c.VAR_COD_RUBRICA.ToUpper() == codice) : query.Where(c => c.VAR_COD_RUBRICA.ToUpper().Contains(codice));

                if (descrizione != null)
                    query = query.Where(c => c.VAR_DESC_CORR.ToUpper().Contains(descrizione));

                var ruolo = await query.Select(c => new CorrGlobaliEntity
                {
                    SYSTEM_ID = c.ORIGINAL_ID ?? 0,
                    VAR_COD_RUBRICA = c.VAR_COD_RUBRICA,
                    DTA_INIZIO = c.DTA_INIZIO,
                    VAR_DESC_CORR = c.VAR_DESC_CORR,
                    ID_UO = c.ID_UO,
                    ID_GRUPPO = c.ID_GRUPPO
                })
                .OrderBy(c => c.DTA_INIZIO)  
                .ToListAsync();

                foreach (var c in ruolo)
                {
                    var org = this._mapper.Map<OrgRisultatoRicerca>(c);
                    org.Tipo = request.tipo;
                    org.IDParent = c.ID_UO.ToString();
                    org.DescParent = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(g => g.SYSTEM_ID == c.ID_UO).Select(g => g.VAR_DESC_CORR).FirstOrDefaultAsync();
                    if (c.DTA_FINE != null)
                        org.Codice = String.Format("<div style=\"text-decoration:line-through;\"><strong>{0}</strong></div>", org.Codice.Remove(org.Codice.LastIndexOf('_')));

                    output.Add(org);
                }
            }

            return new AmmRicercaInOrgResult(output.ToArray());
        }

        #endregion

        #region Private Members

        protected readonly ILogger<AmmRicercaInOrgHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, OrgRisultatoRicerca>()
                     .ForMember(dest => dest.IDCorrGlob, opt => opt.MapFrom(src => src.SYSTEM_ID))
                     .ForMember(dest => dest.IDGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                     .ForMember(dest => dest.Codice, opt => opt.MapFrom(src => src.VAR_COD_RUBRICA))
                     .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => src.VAR_DESC_CORR))
                     .ForMember(dest => dest.IDGruppo, opt => opt.MapFrom(src => src.ID_GRUPPO))
                     .ForMember(dest => dest.IDPeople, opt => opt.MapFrom(src => src.ID_PEOPLE));
            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
