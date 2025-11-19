// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DelegaEsercitaRequest = Pi3.App.Legacy.WebApi.Application.Requests.DelegaEsercita;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DelegaEsercita
{
    public class DelegaEsercitaHandler : IRequestHandler<DelegaEsercitaRequest, DelegaEsercitaResult>
    {
        #region Public Members

        public DelegaEsercitaHandler(ILogger<DelegaEsercitaHandler> logger,
            IClaimsPrincipalService claimsPrincipalService, 
            IMediator mediator,
            IPi3DbContext dbContext,
            IWebMethodLoggerService webMethodLoggerService)
        {
            this._logger = logger;
            this._claimsPrincipalService = claimsPrincipalService;
            this._mediator = mediator;
            this._dbContext = dbContext;
            this._webMethodLoggerService = webMethodLoggerService;

            this.InitializeMapper();
        }

        public async Task<DelegaEsercitaResult> Handle(DelegaEsercitaRequest request, CancellationToken cancellationToken)
        {
            Utente output = null;
            UserLogin.LoginResult loginResult = UserLogin.LoginResult.OK;

            try
            {

                var delegheEntity = await this._dbContext.DelegheEntities
                    .Where(d => d.ID_PEOPLE_DELEGATO == request.infoUtente.idPeople.AsLong() && d.CHA_IN_ESERCIZIO == "1")
                    .Select(d => d)
                    .ToListAsync();

                this._logger.LogDebug("username {0} idAmm {1}", request.login.UserName, request.login.IdAmministrazione);

                if (delegheEntity != null)
                    delegheEntity.ForEach(d => d.CHA_IN_ESERCIZIO = "0");

                var peopleEntity = await this._dbContext.PeopleEntities.AsNoTracking()
                        .Where(p => p.USER_ID.ToUpper().Equals(request.login.UserName.ToUpper()) && p.ID_AMM == request.login.IdAmministrazione.AsLong() && p.DISABLED != "Y")
                        .FirstAsync();

                if (peopleEntity == null)
                    throw new UtenteNotFoundPi3Exception();

                output = this._mapper.Map<Utente>(peopleEntity);
                output.tipoCorrispondente = "P";
                output.dst = Guid.NewGuid().ToString().Replace("-", string.Empty);
                output.sessionID = request.webSessionId;
                output.dominio = await this._dbContext.NetworkAliasesEntities.AsNoTracking().Where(n => n.PERSONORGROUP == peopleEntity.SYSTEM_ID).Select(n => n.NETWORK_ID).FirstOrDefaultAsync();

                var loginEntity = await this._dbContext.LoginEntities.AsNoTracking().FirstOrDefaultAsync(l => l.USER_ID.ToUpper().Equals(peopleEntity.USER_ID.ToUpper()) && l.ID_AMM == peopleEntity.ID_AMM);
                if (loginEntity != null)
                    this._dbContext.LoginEntities.Remove(loginEntity);

                var ruoliQueryable = this._dbContext.PeopleGroupEntities.AsNoTracking()
                        .Join(this._dbContext.CorrGlobaliEntities, pg => pg.GROUPS_SYSTEM_ID, cg => cg.ID_GRUPPO, (pg, cg) => new { pg, cg })
                        .Join(this._dbContext.TipoRuoloEntities, j => j.cg.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new { j.pg, j.cg, t })
                        .Where(j => j.pg.DTA_FINE == null && j.cg.DTA_FINE == null && j.pg.PEOPLE_SYSTEM_ID == peopleEntity.SYSTEM_ID);

                if (request.idRuoloDelegante != "0")
                {
                    ruoliQueryable = ruoliQueryable.Where(j => j.cg.SYSTEM_ID == request.idRuoloDelegante.AsLong());
                }

                var ruoliEntity = await ruoliQueryable.Select(j => new
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

                if(ruoliEntity == null || ruoliEntity.Count == 0)
                    throw new UtenteNoRuoliPi3Exception();

                List<Ruolo> ruoliList = new List<Ruolo>();
                foreach (var r in ruoliEntity)
                {
                    var ruolo = new Ruolo()
                    {
                        systemId = r.SYSTEM_ID.ToString(),
                        descrizione = r.VAR_DESC_CORR,
                        codice = r.VAR_CODICE,
                        livello = r.NUM_LIVELLO.ToString(),
                        idGruppo = r.ID_GRUPPO.ToString(),
                        tipoRuolo = new TipoRuolo()
                        {
                            codice = r.VAR_CODICE,
                            descrizione = r.VAR_DESC_RUOLO
                        },
                        codiceRubrica = r.VAR_COD_RUBRICA,
                        idRegistro = r.ID_REGISTRO.ToString(),
                        idAmministrazione = r.ID_AMM.ToString(),
                        tipoCorrispondente = "R",
                        Responsabile = r.CHA_RESPONSABILE == "1",
                        Segretario = r.CHA_SEGRETARIO == "1",
                        selezionato = r.CHA_PREFERITO == "1"
                    };

                    var uoEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == r.ID_UO)
                        .Select(c => c)
                        .FirstAsync();
                    ruolo.uo = this._mapper.Map<UnitaOrganizzativa>(uoEntity);
                    var idParent = uoEntity.ID_PARENT;
                    while (idParent != null && idParent != 0)
                    {
                        var uoParentEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.SYSTEM_ID == idParent)
                        .Select(c => c)
                        .FirstAsync();

                        ruolo.uo.parent = this._mapper.Map<UnitaOrganizzativa>(uoParentEntity);
                        idParent = uoParentEntity.ID_PARENT;
                    }

                    List<Registro> registriList = new List<Registro>();
                    var registriEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                        .Join(this._dbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                        .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == r.SYSTEM_ID)
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
                    ruolo.registri = registriList.ToArray();

                    List<Funzione> funzioniList = new List<Funzione>();
                    var funzioniEntity = await this._dbContext.FunzioneEntities.AsNoTracking()
                        .Join(this._dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                        .Join(this._dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
                        .Where(j => j.r.ID_RUOLO_IN_UO == r.SYSTEM_ID)
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
                    ruolo.funzioni = funzioniList.ToArray();
                    ruoliList.Add(ruolo);
                }

                output.ruoli = ruoliList.ToArray();

                await this._dbContext.LoginEntities.AddAsync(new LoginEntity
                {
                    USER_ID = peopleEntity.USER_ID.ToUpper(),
                    ID_AMM = peopleEntity.ID_AMM,
                    IP_ADDRESS = request.login.IPAddress,
                    DTA_CONNESSIONE = DateTime.Now,
                    SESSION_ID = request.webSessionId,
                    DST = output.dst,
                    USER_ID_DELEGATO = request.id_delega,
                    CHA_DELEGA = "1"
                });

                var idDelega = request.id_delega.AsLong();
                var delegaEntity = await this._dbContext.DelegheEntities
                    .Where(d => d.SYSTEM_ID == idDelega)
                    .FirstAsync();

                delegaEntity.CHA_IN_ESERCIZIO = "1";

                await this._webMethodLoggerService.LogOK("ESERCITADELEGA");

            }
            catch (UtenteNotFoundPi3Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                loginResult = UserLogin.LoginResult.UNKNOWN_USER;
                await this._webMethodLoggerService.LogKO("ESERCITADELEGA");
            }
            catch (UtenteNoRuoliPi3Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                loginResult = UserLogin.LoginResult.NO_RUOLI;
                output = null;
                await this._webMethodLoggerService.LogKO("ESERCITADELEGA");
            }
            catch (Exception ex)
            {
                this._logger.LogWebMethodError(ex);
                loginResult = UserLogin.LoginResult.APPLICATION_ERROR;
                await this._webMethodLoggerService.LogKO("ESERCITADELEGA");
            }

            await ((DbContext)this._dbContext).SaveChangesAsync();

            return new DelegaEsercitaResult(output, loginResult);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<DelegaEsercitaHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;

        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PeopleEntity, Utente>()
                    .ForMember(dest => dest.idPeople, src => src.MapFrom(opt => opt.SYSTEM_ID))
                    .ForMember(dest => dest.userId, src => src.MapFrom(opt => opt.USER_ID))
                    .ForMember(dest => dest.descrizione, src => src.MapFrom(opt => string.Format("{0} {1}", opt.VAR_COGNOME, opt.VAR_NOME)))
                    .ForMember(dest => dest.telefono, src => src.MapFrom(opt => opt.VAR_TELEFONO))
                    .ForMember(dest => dest.email, src => src.MapFrom(opt => opt.EMAIL_ADDRESS))
                    .ForMember(dest => dest.notifica, src => src.MapFrom(opt => opt.CHA_NOTIFICA))
                    .ForMember(dest => dest.amministratore, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnante, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.assegnatario, src => src.MapFrom(opt => opt.CHA_AMMINISTRATORE == "1"))
                    .ForMember(dest => dest.idAmministrazione, src => src.MapFrom(opt => opt.ID_AMM))
                    .ForMember(dest => dest.notificaConAllegato, src => src.MapFrom(opt => opt.CHA_NOTIFICA_CON_ALLEGATO == "1"))
                    .ForMember(dest => dest.sede, src => src.MapFrom(opt => opt.VAR_SEDE))
                    .ForMember(dest => dest.matricola, src => src.MapFrom(opt => opt.MATRICOLA))
                    .ForMember(dest => dest.cognome, src => src.MapFrom(opt => opt.VAR_COGNOME))
                    .ForMember(dest => dest.nome, src => src.MapFrom(opt => opt.VAR_NOME));

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
