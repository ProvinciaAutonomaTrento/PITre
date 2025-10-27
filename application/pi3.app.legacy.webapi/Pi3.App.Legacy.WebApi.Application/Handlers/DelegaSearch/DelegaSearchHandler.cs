// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.Deleghe;
using DocsPaVO.ricerche;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DelegaSearchRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaSearch;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaSearch
{
    public class DelegaSearchHandler : IRequestHandler<DelegaSearchRequest, DelegaSearchResult>
    {
        #region Public Members

        public DelegaSearchHandler(ILogger<DelegaSearchHandler> logger,
            IClaimsPrincipalService claimsPrincipalService,
            IMediator mediator,
            IPi3DbContext dbContext,
            IDelegaRepository delegaRepository)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._delegaRepository = delegaRepository;

            this.InitializeMapper();
        }

        public async Task<DelegaSearchResult> Handle(DelegaSearchRequest request, CancellationToken cancellationToken)
        {
            var idTenant = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdTenant, true);
            var idPeople = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            var delegatedIdUser = this._claimsPrincipalService.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.DelegatedIdUser);

            var statoDelega = request.searchInfo.StatoDelega;
            List<InfoDelega> output = new List<InfoDelega>();

            var queryable = this._dbContext.DelegheEntities.AsNoTracking();
            if (statoDelega == "A")
                queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_DECORRENZA <= DateTime.Now && (d.DATA_SCADENZA == null || d.DATA_SCADENZA > DateTime.Now));
            if (statoDelega == "I")
                queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_DECORRENZA > DateTime.Now);
            if (statoDelega == "S")
                queryable = this._dbContext.DelegheEntities.AsNoTracking().Where(d => d.DATA_SCADENZA < DateTime.Now);

            switch (request.searchInfo.TipoDelega)
            {
                case "assegnate":
                    queryable = queryable.Where(d => d.ID_PEOPLE_DELEGANTE == idPeople);
                    if (!string.IsNullOrEmpty(request.searchInfo.IdRuoloDelegante))
                    {
                        var idRuoloDelegante = request.searchInfo.IdRuoloDelegante.AsLong();
                        queryable = queryable.Where(d => d.ID_RUOLO_DELEGANTE == idRuoloDelegante);
                    }
                    break;
                case "ricevute":
                    queryable = queryable.Where(d => d.ID_PEOPLE_DELEGATO == idPeople);
                    break;
                case "esercizio":
                    queryable = this._dbContext.DelegheEntities
                        .Where(d => d.ID_PEOPLE_DELEGATO == delegatedIdUser && d.CHA_IN_ESERCIZIO == "1" && d.DATA_DECORRENZA <= DateTime.Now && (d.DATA_SCADENZA == null || d.DATA_SCADENZA >= DateTime.Now));
                    break;
            }

            if (!string.IsNullOrEmpty(request.searchInfo.NomeDelegato))
            {
                queryable = queryable.Join(this._dbContext.PeopleEntities.AsNoTracking(), delega => delega.ID_PEOPLE_DELEGATO, people => people.SYSTEM_ID, (delega, people) => new
                {
                    delega = delega,
                    people.FULL_NAME
                })
                    .Where(p => p.FULL_NAME.ToUpper().Contains(request.searchInfo.NomeDelegato.ToUpper()))
                    .Select(p => p.delega);
            }

            if(!string.IsNullOrEmpty(request.searchInfo.NomeDelegante))
            {
                queryable = queryable.Join(this._dbContext.PeopleEntities.AsNoTracking(), delega => delega.ID_PEOPLE_DELEGANTE, people => people.SYSTEM_ID, (delega, people) => new
                {
                    delega = delega,
                    people.FULL_NAME
                })
                    .Where(p => p.FULL_NAME.ToUpper().Contains(request.searchInfo.NomeDelegante.ToUpper()))
                    .Select(p => p.delega);
            }

            request.pagingContext.RecordCount = await queryable.CountAsync();

            queryable = queryable.OrderByDescending(d => d.DATA_DECORRENZA)
                .Skip(request.pagingContext.StartRow - 1).Take(request.pagingContext.PageSize);

            var delegaEntity = await queryable.ToListAsync();

            foreach (var delega in delegaEntity)
            {
                var infoDelega = new InfoDelega
                {
                    id_delega = delega.SYSTEM_ID.ToString(),
                    id_utente_delegante = delega.ID_PEOPLE_DELEGANTE.ToString(),
                    codiceDelegante = delega.COD_PEOPLE_DELEGANTE,
                    id_ruolo_delegante = delega.ID_RUOLO_DELEGANTE.ToString(),
                    cod_ruolo_delegante = delega.COD_RUOLO_DELEGANTE,
                    id_utente_delegato = delega.ID_PEOPLE_DELEGATO.ToString(),
                    id_uo_delegato = delega.ID_UO_DELEGATO.ToString(),
                    id_ruolo_delegato = delega.ID_RUOLO_DELEGATO.ToString(),
                    cod_ruolo_delegato = delega.COD_RUOLO_DELEGATO,
                    dataDecorrenza = delega.DATA_DECORRENZA.AsDateTimeFormat(),
                    dataScadenza = delega.DATA_SCADENZA.AsDateTimeFormat(),
                    inEsercizio = delega.CHA_IN_ESERCIZIO
                };


                infoDelega.id_people_corr_globali = (await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(c => c.ID_PEOPLE == delega.ID_PEOPLE_DELEGATO)
                    .Select(c => c.SYSTEM_ID).FirstAsync()).ToString();

                if (delega.DATA_SCADENZA != null && delega.CHA_IN_ESERCIZIO == "1" && delega.DATA_SCADENZA < DateTime.Now)
                {
                    var aggregate = await _delegaRepository.Get(idTenant.ToString(), delega.SYSTEM_ID.ToString());
                    aggregate.Dismetti();

                    await this._delegaRepository.Update(aggregate);

                    infoDelega.inEsercizio = "0";
                }

                if(delega.ID_PEOPLE_DELEGATO != 0)
                {
                    var utenteEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == delega.ID_PEOPLE_DELEGATO)
                        .Select(p => new
                        {
                            p.VAR_COGNOME,
                            p.VAR_NOME,
                            p.DISABLED
                        })
                        .FirstAsync();

                    infoDelega.cod_utente_delegato = String.Format("{0} {1}", utenteEntity.VAR_COGNOME, utenteEntity.VAR_NOME);
                    infoDelega.utDelegatoDismesso = utenteEntity.DISABLED == "Y" ? "1" : "0";
                }

                if (delega.ID_PEOPLE_DELEGANTE != 0)
                {
                    var utenteEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                        .Where(p => p.SYSTEM_ID == delega.ID_PEOPLE_DELEGANTE)
                        .Select(p => new
                        {
                            p.VAR_COGNOME,
                            p.VAR_NOME,
                            p.DISABLED
                        })
                        .FirstAsync();

                    infoDelega.cod_utente_delegante = String.Format("{0} {1}", utenteEntity.VAR_COGNOME, utenteEntity.VAR_NOME);
                    infoDelega.utDeleganteDismesso = utenteEntity.DISABLED == "Y" ? "1" : "0";
                }

                output.Add(infoDelega);
            }

            return new DelegaSearchResult(output.ToArray(), request.pagingContext);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaSearchHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected readonly IDelegaRepository _delegaRepository;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DelegaEntity, InfoDelega>()
                    .ForMember(dest => dest.id_delega, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.id_utente_delegante, src => src.MapFrom(opt => opt.ID_PEOPLE_DELEGANTE))
                    .ForMember(dest => dest.codiceDelegante, src => src.MapFrom(opt => opt.COD_PEOPLE_DELEGANTE))

                    .ForMember(dest => dest.id_ruolo_delegante, src => src.MapFrom(opt => opt.ID_PEOPLE_DELEGANTE))
                    .ForMember(dest => dest.cod_ruolo_delegante, src => src.MapFrom(opt => opt.COD_RUOLO_DELEGANTE))
                    .ForMember(dest => dest.id_utente_delegato, src => src.MapFrom(opt => opt.ID_PEOPLE_DELEGATO))
                    .ForMember(dest => dest.id_uo_delegato, src => src.MapFrom(opt => opt.ID_UO_DELEGATO))
                    .ForMember(dest => dest.id_ruolo_delegato, src => src.MapFrom(opt => opt.ID_RUOLO_DELEGATO))
                    .ForMember(dest => dest.cod_ruolo_delegato, src => src.MapFrom(opt => opt.COD_RUOLO_DELEGATO))
                    .ForMember(dest => dest.dataDecorrenza, src => src.MapFrom(opt => opt.DATA_DECORRENZA))
                    .ForMember(dest => dest.dataScadenza, src => src.MapFrom(opt => opt.DATA_SCADENZA))
                    .ForMember(dest => dest.inEsercizio, src => src.MapFrom(opt => opt.CHA_IN_ESERCIZIO))
                    .ForMember(dest => dest.id_people_corr_globali, src => src.MapFrom(opt => opt.ID_PEOPLE_DELEGANTE));


            });

            _mapper = configuration.CreateMapper();
        }
        #endregion
    }
}
