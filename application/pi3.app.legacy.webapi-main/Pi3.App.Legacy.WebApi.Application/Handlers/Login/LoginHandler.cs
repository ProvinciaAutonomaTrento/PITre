// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using DocsPaVO.utente;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.Legacy.WebApi.Application.Extensions;
using Pi3.App.Legacy.WebApi.Application.Requests;
using Pi3.Core.AggregateModels.AggregazioneDocumentaleAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LoginRequest = Pi3.App.Legacy.WebApi.Application.Requests.Login;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.Login
{

    public class LoginHandler : IRequestHandler<LoginRequest, LoginResult>
    {
        #region Public Members

        public LoginHandler(ILogger<LoginHandler> logger,
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

        public async Task<LoginResult> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            DocsPaVO.utente.UserLogin.LoginResult output = UserLogin.LoginResult.OK;
            Utente utente = null;
            string ipAddress = string.Empty;
            UserLogin userLogin = request.login;
            bool multiAmministrazione = false;

            try
            {
                _logger.LogInformation("Inizio Login");

                var peopleEntities = await this._dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.USER_ID.ToUpper().Equals(userLogin.UserName.ToUpper()))
                    .ToListAsync();

                if (peopleEntities == null || peopleEntities.Count == 0)
                    throw new UtenteNotFoundPi3Exception();

                _logger.LogInformation("People trovata");

                if (string.IsNullOrEmpty(userLogin.IdAmministrazione))
                {
                    var listIdAmmPeople = peopleEntities.Where(p => p.DISABLED != "Y").ToList();
                    
                    if(listIdAmmPeople == null || listIdAmmPeople.Count == 0)
                        throw new UtenteDisabilitatoPi3Exception();

                    userLogin.IdAmministrazione = listIdAmmPeople[0].ID_AMM.ToString();
                    multiAmministrazione = listIdAmmPeople.Count > 1;
                }

                if (!string.IsNullOrEmpty(userLogin.IdAmministrazione))
                {
                    _logger.LogInformation("Amministrazione trovata trovata");

                    var peopleEntity = peopleEntities.Where(p => p.ID_AMM == userLogin.IdAmministrazione.AsLong()).FirstOrDefault();

                    if (peopleEntity == null)
                        throw new UtenteNotFoundPi3Exception();

                    if (peopleEntity.DISABLED == "Y")
                        throw new UtenteDisabilitatoPi3Exception();

                    if (userLogin.SSOLogin)
                    {
                        //SSOAuthToken
                        _logger.LogInformation("SSOLogin attiva");
                        if(IsAuthToken(userLogin.Password))
                        {
                            _logger.LogInformation("IsAuthToken true");
                        }
                        else
                        {
                            
                        }
                    }
                    else
                    {
                        _logger.LogInformation("SSOLogin non attiva: autenticazione classica");

                        //Autenticazione PiTre
                        var encryptedPassword = CalcolaImpronta(System.Text.Encoding.Unicode.GetBytes(userLogin.Password));
                        if (string.Compare(encryptedPassword, peopleEntity.ENCRYPTED_PASSWORD, false) != 0)
                        {
                            throw new UtenteNotFoundPi3Exception();
                        }
                        var amministraEntity = await this._dbContext.AmministraEntities.AsNoTracking().FirstAsync(a => a.SYSTEM_ID == userLogin.IdAmministrazione.AsLong());
                        if (amministraEntity.ENABLE_PASSWORD_EXPIRATION == "1" && peopleEntity.CHA_AMMINISTRATORE == "0" && peopleEntity.PASSWORD_NEVER_EXPIRE != "1")
                        {
                            //DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                            DateTime actualDate = DateTime.Now;

                            var creationDate = peopleEntity.PASSWORD_CREATION_DATE.HasValue ? peopleEntity.PASSWORD_CREATION_DATE : DateTime.MinValue;
                            DateTime expireDate = creationDate.Value.AddDays(Convert.ToInt32(amministraEntity.PASSWORD_EXPIRATION_DAYS));

                            if (!peopleEntity.PASSWORD_CREATION_DATE.HasValue || peopleEntity.PASSWORD_CREATION_DATE > actualDate || actualDate >= expireDate)
                                throw new PasswordScadutaPi3Exception();
                        }
                    }
                    if (multiAmministrazione)
                        throw new UtenteMultiAmministrazionePi3Exception();

                    var loginEntity = await this._dbContext.LoginEntities.AsNoTracking().FirstOrDefaultAsync(l => l.USER_ID.ToUpper().Equals(peopleEntity.USER_ID.ToUpper()) && l.ID_AMM == peopleEntity.ID_AMM);
                    if (!request.forced)
                    {
                        if (loginEntity != null)
                        {
                            ipAddress = loginEntity.IP_ADDRESS;
                            throw new SessioUtenteEsistentePi3Exception();
                        }
                    }

                    if (loginEntity != null)
                        this._dbContext.LoginEntities.Remove(loginEntity);

                    var ruoliEntity = await this._dbContext.PeopleGroupEntities.AsNoTracking()
                    .Join(this._dbContext.CorrGlobaliEntities, pg => pg.GROUPS_SYSTEM_ID, cg => cg.ID_GRUPPO, (pg, cg) => new { pg, cg })
                    .Join(this._dbContext.TipoRuoloEntities, j => j.cg.ID_TIPO_RUOLO, t => t.SYSTEM_ID, (j, t) => new { j.pg, j.cg, t })
                    .Where(j => j.pg.DTA_FINE == null && j.cg.DTA_FINE == null && j.pg.PEOPLE_SYSTEM_ID == peopleEntity.SYSTEM_ID)
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
                    .ThenByDescending(c => c.CHA_PREFERITO)
                    .ThenBy(c => c.VAR_DESC_CORR)
                    .ToListAsync();

                    if (ruoliEntity == null || ruoliEntity.Count == 0)
                        throw new UtenteNoRuoliPi3Exception();

                    var corrGlobaliSystemId = await this._dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_PEOPLE == peopleEntity.SYSTEM_ID).Select(c => c.SYSTEM_ID).FirstAsync();
                    utente = this._mapper.Map<Utente>(peopleEntity);
                    utente.tipoCorrispondente = "P";
                    utente.dst = Guid.NewGuid().ToString().Replace("-", string.Empty);
                    utente.sessionID = request.webSessionId;
                    utente.dominio = await this._dbContext.NetworkAliasesEntities.AsNoTracking().Where(n => n.PERSONORGROUP == peopleEntity.SYSTEM_ID).Select(n => n.NETWORK_ID).FirstOrDefaultAsync();
                    utente.systemId = corrGlobaliSystemId.ToString();

                    utente.ruoli = new Ruolo[ruoliEntity.Count];
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

                        var uoEntity = await this._dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(c => c.SYSTEM_ID == ruoliEntity[r].ID_UO)
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

                        var registriEntity = await this._dbContext.RegistroEntities.AsNoTracking()
                            .Join(this._dbContext.RuoloRegistroEntities, r => r.SYSTEM_ID, rr => rr.ID_REGISTRO, (r, rr) => new { r, rr })
                            .Where(j => j.r.CHA_RF == "0" && j.rr.ID_RUOLO_IN_UO == ruoliEntity[r].SYSTEM_ID)
                            .Select(j => new
                            {
                                REGISTRO = j.r,
                                j.rr.CHA_PREFERITO ,
                                j.r.VAR_PREG
                            })
                            .OrderBy(j => j.REGISTRO.CHA_STATO)
                            .ThenByDescending(j => j.CHA_PREFERITO)
                            .ThenBy(j => j.REGISTRO.VAR_CODICE)
                            .ThenBy(j => j.REGISTRO.VAR_DESC_REGISTRO)
                            .ToListAsync();

                        ruolo.registri = new DocsPaVO.utente.Registro[registriEntity.Count];
                        for (int reg = 0; reg < registriEntity.Count; reg++)
                        {
                            ruolo.registri[reg] = (this._mapper.Map<DocsPaVO.utente.Registro>(registriEntity[reg].REGISTRO)); 
                        }

                        var funzioniEntity = await this._dbContext.FunzioneEntities.AsNoTracking()
                            .Join(this._dbContext.TipoFunzioneEntities, f => f.ID_TIPO_FUNZIONE, t => t.SYSTEM_ID, (f, t) => new { f, t })
                            .Join(this._dbContext.TipoFRuoloEntities, j => j.t.SYSTEM_ID, r => r.ID_TIPO_FUNZ, (j, r) => new { j.f, j.t, r })
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
                        utente.ruoli[r] = ruolo;
                    }

                    await this._dbContext.LoginEntities.AddAsync(new LoginEntity
                    {
                        USER_ID = peopleEntity.USER_ID,
                        ID_AMM = peopleEntity.ID_AMM,
                        IP_ADDRESS = userLogin.IPAddress,
                        DTA_CONNESSIONE = DateTime.Now,
                        SESSION_ID = request.webSessionId,
                        DST = utente.dst
                    });
                }

                await this._webMethodLoggerService.LogOK("LOGIN", null, null, null, null, null, userLogin.IdAmministrazione, utente.idPeople, utente.codiceRubrica, utente.ruoli[0].idGruppo);

            }
            catch (UtenteNotFoundPi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.UNKNOWN_USER;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (UtenteDisabilitatoPi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.DISABLED_USER;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (PasswordScadutaPi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.PASSWORD_EXPIRED;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (SessioUtenteEsistentePi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.USER_ALREADY_LOGGED_IN;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (UtenteMultiAmministrazionePi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.NO_AMMIN;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (UtenteNoRuoliPi3Exception ex)
            {
                this._logger.LogWarning(exception: ex, message: ex.Message);
                output = UserLogin.LoginResult.NO_RUOLI;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }
            catch (Exception ex) 
            {
                this._logger.LogWebMethodError(ex);
                output = UserLogin.LoginResult.APPLICATION_ERROR;
                utente = null;
                //await this._webMethodLoggerService.LogKO("LOGIN", null, null, null, null, null, null, "0");
            }

            return new LoginResult(output, utente, ipAddress);
        }

        #endregion

        #region Private Members

        protected readonly ILogger<LoginHandler> _logger;
        protected readonly IClaimsPrincipalService _claimsPrincipalService;
        protected readonly IMediator _mediator;
        protected readonly IPi3DbContext _dbContext;
        protected IWebMethodLoggerService _webMethodLoggerService;
        protected const string TOKEN_PREFIX = "SSO=";

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

        protected string CalcolaImpronta(byte[] stream)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] impronta = sha.ComputeHash(stream);
            return BitConverter.ToString(impronta).Replace("-", "");
        }

        protected bool IsAuthToken(string token)
        {
            return (token.IndexOf(TOKEN_PREFIX) > -1);
        }
        #endregion
    }
}
