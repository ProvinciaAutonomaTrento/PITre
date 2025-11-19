// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetListaRuoliUtenteRequest = Pi3.App.Legacy.WebApi.Application.Requests.GetListaRuoliUtente;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.GetListaRuoliUtente
{
    public class GetListaRuoliUtenteHandler : IRequestHandler<GetListaRuoliUtenteRequest, GetListaRuoliUtenteResult>
    {
        #region Public Members

        public GetListaRuoliUtenteHandler(
            ILogger<GetListaRuoliUtenteHandler> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext pi3DbContext)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._pi3DbContext = pi3DbContext;
            this.InitializeMapper();
        }

        public async Task<GetListaRuoliUtenteResult> Handle(GetListaRuoliUtenteRequest request, CancellationToken cancellationToken)
        {
            var ruoli = new List<Ruolo>();

            try
            {
                var ruoliEntity = await this._pi3DbContext.PeopleGroupEntities.AsNoTracking()
                                    .Join(this._pi3DbContext.CorrGlobaliEntities, pg => pg.GROUPS_SYSTEM_ID, cg => cg.ID_GRUPPO, (pg, cg) => new { pg, cg })
                                    .Join(this._pi3DbContext.TipoRuoloEntities, j => j.cg.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new { j.pg, j.cg, t })
                                    .Where(j => j.pg.DTA_FINE == null && j.cg.DTA_FINE == null && j.pg.PEOPLE_SYSTEM_ID == request.idPeople.AsLong())
                                    .Select(j => new
                                    {
                                        j.pg.PEOPLE_SYSTEM_ID,
                                        j.pg.CHA_PREFERITO,
                                        j.cg.SYSTEM_ID,
                                        j.cg.ID_GRUPPO,
                                        j.cg.ID_UO,
                                        j.cg.VAR_COD_RUBRICA,
                                        j.cg.ID_REGISTRO,
                                        j.cg.ID_AMM,
                                        j.cg.VAR_DESC_CORR,
                                        j.cg.CHA_RIFERIMENTO,
                                        j.cg.CHA_RESPONSABILE,
                                        j.cg.CHA_SEGRETARIO,
                                        j.t.NUM_LIVELLO,
                                        j.t.VAR_CODICE,
                                        j.t.VAR_DESC_RUOLO,
                                    })
                                    .OrderByDescending(c => c.CHA_PREFERITO != null)
                                    .ToListAsync();

                var corrGlobaliSystemId = await this._pi3DbContext
                    .CorrGlobaliEntities
                    .AsNoTracking()
                    .Where(c => c.ID_PEOPLE == request.idPeople.AsLong())
                    .Select(c => c.SYSTEM_ID)
                    .FirstAsync();

                for (int r = 0; r < ruoliEntity.Count; r++)
                {
                    var ruolo = new Ruolo()
                    {
                        systemId = ruoliEntity[r].SYSTEM_ID.ToString(),
                        descrizione = ruoliEntity[r].VAR_DESC_CORR,
                        codice = ruoliEntity[r].VAR_CODICE,
                        livello = ruoliEntity[r].NUM_LIVELLO.ToString(),
                        idGruppo = ruoliEntity[r].ID_GRUPPO.ToString(),
                        tipoRuolo = new TipoRuolo()
                        {
                            codice = ruoliEntity[r].VAR_CODICE,
                            descrizione = ruoliEntity[r].VAR_DESC_RUOLO
                        },
                        codiceRubrica = ruoliEntity[r].VAR_COD_RUBRICA,
                        idRegistro = ruoliEntity[r].ID_REGISTRO.ToString(),
                        idAmministrazione = ruoliEntity[r].ID_AMM.ToString(),
                        tipoCorrispondente = "R",
                        Responsabile = ruoliEntity[r].CHA_RESPONSABILE == "1",
                        Segretario = ruoliEntity[r].CHA_SEGRETARIO == "1",
                        selezionato = ruoliEntity[r].CHA_PREFERITO == "1"
                    };

                    var uoEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == ruoliEntity[r].ID_UO)
                        .Select(c => c)
                        .FirstAsync();

                    ruolo.uo = this._mapper.Map<UnitaOrganizzativa>(uoEntity);

                    var idParent = uoEntity.ID_PARENT;

                    while (idParent != null && idParent != 0)
                    {
                        var uoParentEntity = await this._pi3DbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idParent)
                        .Select(c => c)
                        .FirstAsync();

                        ruolo.uo.parent = this._mapper.Map<UnitaOrganizzativa>(uoParentEntity);

                        idParent = uoParentEntity.ID_PARENT;
                    }

                    var registriEntity = await this._pi3DbContext.RegistroEntities.AsNoTracking()
                        .Join(this._pi3DbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                        .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == ruoliEntity[r].SYSTEM_ID)
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

                    ruolo.registri = new Registro[registriEntity.Count];

                    for (int reg = 0; reg < registriEntity.Count; reg++)
                    {
                        ruolo.registri[reg] = (
                            this._mapper.Map<DocsPaVO.utente.Registro>(registriEntity[reg].REGISTRO));
                    }

                    var funzioniEntity = await this._pi3DbContext.FunzioneEntities.AsNoTracking()
                        .Join(this._pi3DbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                        .Join(this._pi3DbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                        .Where(j => j.r.ID_RUOLO_IN_UO == ruoliEntity[r].SYSTEM_ID)
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

                    ruolo.funzioni = new Funzione[funzioniEntity.Count()];

                    for (int i = 0; i < funzioniEntity.Count(); i++)
                    {
                        ruolo.funzioni[i] = new Funzione()
                        {
                            systemId = funzioniEntity[i].SYSTEM_ID.ToString(),
                            descrizione = funzioniEntity[i].VAR_DESC_FUNZIONE,
                            codice = funzioniEntity[i].COD_FUNZIONE,
                            idTipoFunzione = funzioniEntity[i].ID_TIPO_FUNZIONE.ToString(),
                            codTipoFunzione = funzioniEntity[i].VAR_COD_TIPO,
                            descTipoFunzione = funzioniEntity[i].VAR_DESC_TIPO_FUN
                        };
                    }

                    ruoli.Add(ruolo);
                }
            }
            catch (Pi3Exception pi3Ex)
            {
                ruoli = null;
                this._logger.LogError(exception: pi3Ex, message: pi3Ex.Message);
            }
            catch (Exception ex)
            {
                ruoli = null;
                this._logger.LogCritical(exception: ex, message: ex.Message);
            }

            return new GetListaRuoliUtenteResult(ruoli != null ? ruoli.ToArray() : null);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<GetListaRuoliUtenteHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _pi3DbContext;

        protected IMapper _mapper = null;

        protected void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CorrGlobaliEntity, UnitaOrganizzativa>()
                    .ForMember(dest => dest.systemId, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => opt.VAR_DESC_CORR))
                    .ForMember(dest => dest.codice, src => src.MapFrom(opt => opt.VAR_CODICE))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.livello, src => src.MapFrom(opt => opt.NUM_LIVELLO))
                    .ForMember(dest => dest.codiceRubrica, src => src.MapFrom(opt => opt.VAR_COD_RUBRICA))
                    .ForMember(dest => dest.idRegistro, src => src.MapFrom(opt => opt.ID_REGISTRO))
                    .ForMember(dest => dest.interoperante, src => src.MapFrom(opt => opt.CHA_PA == "1"))
                    .ForMember(dest => dest.codiceAOO, src => src.MapFrom(opt => opt.VAR_CODICE_AOO))
                    .ForMember(dest => dest.codiceAmm, src => src.MapFrom(opt => opt.VAR_CODICE_AMM))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.VAR_EMAIL))
                    .ForMember(dest => dest.tipoIE, src => src.MapFrom(opt => opt.CHA_TIPO_IE))
                    .ForMember(dest => dest.tipoCorrispondente, src => src.MapFrom(opt => opt.CHA_TIPO_URP))
                    .ForMember(dest => dest.classificaUO, src => src.MapFrom(opt => opt.CLASSIFICA_UO));

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

        #endregion
    }
}