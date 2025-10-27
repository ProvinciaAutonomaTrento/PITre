// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getRuoloByIdGruppoRequest = Pi3.App.Legacy.WebApi.Application.Requests.getRuoloByIdGruppo;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.getRuoloByIdGruppo
{
    public class getRuoloByIdGruppoHandler : IRequestHandler<getRuoloByIdGruppoRequest, getRuoloByIdGruppoResult>
    {
        #region Public Members

        public getRuoloByIdGruppoHandler(ILogger<getRuoloByIdGruppoHandler> logger,
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

        public async Task<getRuoloByIdGruppoResult> Handle(getRuoloByIdGruppoRequest request, CancellationToken cancellationToken)
        {
            Ruolo output = null;

            try
            {
                var idGruppoAsLong = request.idGruppo.AsLong();

                var ruoloEntity = await (from cg in this._dbContext.CorrGlobaliEntities
                                      join tr in this._dbContext.TipoRuoloEntities on cg.ID_TIPO_RUOLO equals tr.SYSTEM_ID
                                      join pg in this._dbContext.PeopleGroupEntities on cg.ID_GRUPPO equals pg.GROUPS_SYSTEM_ID
                                      join cgu in this._dbContext.CorrGlobaliEntities on cg.ID_UO equals cgu.SYSTEM_ID
                                      where pg.DTA_FINE == null && cg.ID_GRUPPO == idGruppoAsLong
                                    select new
                                    {
                                        pg.PEOPLE_SYSTEM_ID,
                                        cg.SYSTEM_ID,
                                        cg.ID_GRUPPO,
                                        tr.NUM_LIVELLO,
                                        cg.ID_REGISTRO,
                                        tr.VAR_CODICE,
                                        tr.VAR_DESC_RUOLO,
                                        NUM_LIVELLO_UO= cgu.NUM_LIVELLO,
                                        cg.ID_UO,
                                        cg.VAR_COD_RUBRICA,
                                        cg.ID_AMM,
                                        cg.VAR_DESC_CORR,
                                        pg.CHA_PREFERITO,
                                        cg.CHA_RIFERIMENTO,
                                        cg.CHA_RESPONSABILE,
                                        cg.CHA_SEGRETARIO
                                    })
                              .FirstOrDefaultAsync();

                var idUO = ruoloEntity.ID_UO;
                var uoRuoloEntity = await (from cg in this._dbContext.CorrGlobaliEntities
                                           where cg.CHA_TIPO_IE == "I" && cg.CHA_TIPO_URP == "U" && cg.DTA_FINE == null && cg.SYSTEM_ID == idUO
                                           select cg)
                                            .FirstOrDefaultAsync();
                UnitaOrganizzativa uo = this._mapper.Map<UnitaOrganizzativa>(uoRuoloEntity);
                uo.parent = await GetUoParents(uoRuoloEntity.ID_PARENT);

                List<Registro> registriList = new List<Registro>();
                var registriEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                    .Join(this._dbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                    .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == ruoloEntity.SYSTEM_ID)
                    .Select(j => new
                    {
                        REGISTRO = j.r,
                        j.rr.CHA_PREFERITO,
                        j.r.VAR_PREG
                    })
                    .OrderBy(j => j.REGISTRO.CHA_STATO)
                    .ThenByDescending(j => j.CHA_PREFERITO)
                    .ThenBy(j => j.REGISTRO.VAR_CODICE)
                    .ThenBy(j => j.REGISTRO.VAR_DESC_REGISTRO)
                    .ToListAsync();
                foreach (var reg in registriEntity)
                {
                    registriList.Add(this._mapper.Map<DocsPaVO.utente.Registro>(reg.REGISTRO));
                }

                List<Funzione> funzioniList = new List<Funzione>();
                var funzioniEntity = await this._dbContext.FunzioneEntities.AsNoTracking()
                    .Join(this._dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                    .Join(this._dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                    .Where(j => j.r.ID_RUOLO_IN_UO == ruoloEntity.SYSTEM_ID)
                    .Select(j => new
                    {
                        j.f.SYSTEM_ID,
                        j.f.COD_FUNZIONE,
                        j.f.VAR_DESC_FUNZIONE,
                        j.f.ID_TIPO_FUNZIONE,
                        j.t.VAR_COD_TIPO,
                        j.t.VAR_DESC_TIPO_FUN
                    })
                    .ToListAsync();
                foreach (var f in funzioniEntity)
                {
                    funzioniList.Add(new Funzione()
                    {
                        systemId = f.SYSTEM_ID.ToString(),
                        descrizione = f.VAR_DESC_FUNZIONE,
                        codice = f.COD_FUNZIONE,
                        idTipoFunzione = f.ID_TIPO_FUNZIONE.ToString(),
                        codTipoFunzione = f.VAR_COD_TIPO,
                        descTipoFunzione = f.VAR_DESC_TIPO_FUN
                    });
                }

                output = new Ruolo()
                {
                    systemId = ruoloEntity.SYSTEM_ID.ToString(),
                    codiceRubrica = ruoloEntity.VAR_COD_RUBRICA,
                    descrizione = ruoloEntity.VAR_DESC_CORR,
                    codice = ruoloEntity.VAR_CODICE,
                    idRegistro = ruoloEntity.ID_REGISTRO.ToString(),
                    idAmministrazione = ruoloEntity.ID_AMM.ToString(),
                    tipoCorrispondente = "R",
                    Responsabile = ruoloEntity.CHA_RESPONSABILE == "1",
                    Segretario = ruoloEntity.CHA_SEGRETARIO == "1",
                    selezionato = ruoloEntity.CHA_PREFERITO == "1",
                    livello = ruoloEntity.NUM_LIVELLO.ToString(),
                    uo = uo,
                    tipoRuolo = new TipoRuolo
                    {
                        codice = ruoloEntity.VAR_CODICE,
                        descrizione = ruoloEntity.VAR_DESC_RUOLO
                    },
                    registri = registriList.ToArray(),
                    funzioni = funzioniList.ToArray()                    
                };
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, null, null);
                output = null;
            }

            return new getRuoloByIdGruppoResult(output);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<getRuoloByIdGruppoHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, UnitaOrganizzativa>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codiceRubrica, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.interoperante, src => src.MapFrom(opt => opt.CHA_PA == "1"))
                    .ForMember(dest => dest.codiceAOO, src => src.MapFrom(opt => opt.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL))
                    .ForMember(dest => dest.tipoIE, src => src.MapFrom(opt => opt.CHA_TIPO_IE))
                    .ForMember(dest => dest.tipoCorrispondente, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.classificaUO, src => src.MapFrom(opt => opt.CLASSIFICA_UO)).AfterMap((src, dest) =>
                    {
                        dest.serverPosta = new ServerPosta()
                        {
                            serverSMTP = src.VAR_SMTP,
                            portaSMTP = src.NUM_PORTA_SMTP.ToString()
                        };
                    });

                cfg.CreateMap<RegistroEntity, DocsPaVO.utente.Registro>()
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
                   .ForMember(dest => dest.invioRicevutaManuale, src => src.MapFrom(opt => opt.INVIO_RICEVUTA_MANUALE == 0 ? "0" : "1"))
                   .ForMember(dest => dest.FlagWspia, src => src.MapFrom(opt => opt.FLAG_WSPIA == null ? "0" : opt.FLAG_WSPIA))
                   .ForMember(dest => dest.flag_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1"))
                   .ForMember(dest => dest.anno_pregresso, src => src.MapFrom(opt => opt.VAR_PREG == "1" ? opt.ANNO_PREG : string.Empty))
                   .ForMember(dest => dest.codiceIpa, src => src.MapFrom(opt => opt.VAR_CODICE_IPA));
            });

            this._mapper = configuration.CreateMapper();
        }

        protected async Task<UnitaOrganizzativa> GetUoParents(long? idParent)
        {
            UnitaOrganizzativa parent = new UnitaOrganizzativa();

            var corrGlobaliEntity = await (from cg in this._dbContext.CorrGlobaliEntities
                                            where cg.CHA_TIPO_IE == "I" && cg.CHA_TIPO_URP == "U" && cg.DTA_FINE == null && cg.SYSTEM_ID == idParent
                                            select cg)
                                    .FirstOrDefaultAsync();

            parent = this._mapper.Map<UnitaOrganizzativa>(corrGlobaliEntity);
                
            if(corrGlobaliEntity != null && corrGlobaliEntity.ID_PARENT != null && corrGlobaliEntity.ID_PARENT != 0)
                parent.parent = await GetUoParents(corrGlobaliEntity.ID_PARENT);

            return parent;
        }

        #endregion
    }
}
