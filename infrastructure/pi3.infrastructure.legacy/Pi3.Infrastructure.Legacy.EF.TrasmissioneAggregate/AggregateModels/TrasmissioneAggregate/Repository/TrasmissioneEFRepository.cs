// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Events;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Configuration;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.WebMethodLogger;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Resources;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.Entities.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.TrasmissioneAggregate.Repository
{
    internal static class SecurityListExtensions
    {
        public static void AddIfNotExists(this List<SecurityEntity> list, SecurityEntity securityEntity)
        {
            if (!list.Any(s => s.THING == securityEntity.THING
                && s.PERSONORGROUP == securityEntity.PERSONORGROUP
                && s.ACCESSRIGHTS == securityEntity.ACCESSRIGHTS))
            {
                list.Add(securityEntity);
            }
            else
            {

            }
        }
    }

    public class TrasmissioneEFRepository : ElementRepository<Trasmissione>, ITrasmissioneRepository
    {
        #region Public Members

        public TrasmissioneEFRepository(
            ILogger<TrasmissioneEFRepository> logger, 
            IClaimsPrincipalService claimsPrincipalService, 
            IEventPublisher eventPublisher, 
            IPi3DbContext dbContext, 
            IConfigurationService configurationService)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;
            _configurationService = configurationService;
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected readonly IConfigurationService _configurationService;

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            return await _dbContext.TrasmissioneEntities.AsNoTracking().AnyAsync(t => t.SYSTEM_ID == id.AsLong());
        }

        protected override async Task<Trasmissione> HandleGet(Trasmissione newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var idGruppo = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup);

            var trasmissioneEntity = await (from t in _dbContext.TrasmissioneEntities.AsNoTracking()
                                            join cg in _dbContext.CorrGlobaliEntities.AsNoTracking() on t.ID_RUOLO_IN_UO equals cg.SYSTEM_ID
                                            where t.SYSTEM_ID == id.AsLong()
                                            select new
                                            {
                                                t.SYSTEM_ID,
                                                t.DTA_INVIO,
                                                t.ID_PEOPLE,
                                                t.ID_PEOPLE_DELEGATO,
                                                t.ID_RUOLO_IN_UO,
                                                cg.ID_GRUPPO,
                                                t.CHA_TIPO_OGGETTO,
                                                t.ID_PROFILE,
                                                t.ID_PROJECT,
                                                t.VAR_NOTE_GENERALI
                                            })
                                     .FirstOrDefaultAsync();

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(id);

            await _dbContext.AssertSecurityRights(
                (trasmissioneEntity.ID_PROFILE.HasValue ? trasmissioneEntity.ID_PROFILE : trasmissioneEntity.ID_PROJECT).ToString(),
                _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true),
                _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true),
                SecurityRightTypesEnum.Read);

            var idGruppoTrasm = trasmissioneEntity.ID_GRUPPO;
            if (!trasmissioneEntity.ID_GRUPPO.HasValue)
            {
                //Se ID_GRUPPO è null vuol dire che il ruolo mittente è stato storicizzato per cui vado a recuperare il nuovo id
                var idCorrGlobali = trasmissioneEntity.ID_RUOLO_IN_UO;
                while(!idGruppoTrasm.HasValue)
                {
                     var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(c => c.ID_OLD == idCorrGlobali)
                        .Select(c => new
                        {
                            c.ID_GRUPPO,
                            c.SYSTEM_ID
                        })
                        .FirstOrDefaultAsync();

                    if(corrGlobaliEntity != null)
                    {
                        idGruppoTrasm = corrGlobaliEntity.ID_GRUPPO;
                        idCorrGlobali = corrGlobaliEntity.SYSTEM_ID;
                    }
                }
            }

            var aggregate = new Trasmissione(
                trasmissioneEntity.SYSTEM_ID.ToString(),
                idTenant,
                trasmissioneEntity.DTA_INVIO.HasValue ? trasmissioneEntity.DTA_INVIO.Value : DateTime.Now,
                trasmissioneEntity.CHA_TIPO_OGGETTO == "D" ? trasmissioneEntity.ID_PROFILE.ToString() : trasmissioneEntity.ID_PROJECT.ToString(),
                trasmissioneEntity.CHA_TIPO_OGGETTO == "D" ? TipiOggettiTrasmessiEnum.DocumentoAmministrativo : TipiOggettiTrasmessiEnum.Fascicolo,
                new Autore()
                {
                    IdUtente = trasmissioneEntity.ID_PEOPLE.ToString(),
                    IdUtenteDelegato = trasmissioneEntity.ID_PEOPLE_DELEGATO.HasValue ? trasmissioneEntity.ID_PEOPLE.ToString() : null,
                    IdGruppo = idGruppoTrasm.ToString()
                },
                !string.IsNullOrWhiteSpace(trasmissioneEntity.VAR_NOTE_GENERALI) ? new TextValue(trasmissioneEntity.VAR_NOTE_GENERALI) : null);

            var trasmissioniSingoleEntities = await (from ts in _dbContext.TrasmSingolaEntities.AsNoTracking()
                                                     join rg in _dbContext.RagioneTrasmissioneEntities.AsNoTracking() on ts.ID_RAGIONE equals rg.SYSTEM_ID
                                                     join cg in _dbContext.CorrGlobaliEntities.AsNoTracking() on ts.ID_CORR_GLOBALE equals cg.SYSTEM_ID
                                                     where ts.ID_TRASMISSIONE == trasmissioneEntity.SYSTEM_ID
                                                     select new
                                                     {
                                                         TS_SYSTEM_ID = ts.SYSTEM_ID,
                                                         TS_CHA_TIPO_TRASM = ts.CHA_TIPO_TRASM,
                                                         TS_CHA_TIPO_DEST = ts.CHA_TIPO_DEST,
                                                         TS_DTA_SCADENZA = ts.DTA_SCADENZA,
                                                         TS_HIDE_DOC_VERSIONS = ts.HIDE_DOC_VERSIONS,
                                                         TS_VAR_NOTE_SING = ts.VAR_NOTE_SING,
                                                         RG_SYSTEM_ID = rg.SYSTEM_ID,
                                                         RG_VAR_DESC_RAGIONE = rg.VAR_DESC_RAGIONE,
                                                         RG_CHA_TIPO_RAGIONE = rg.CHA_TIPO_RAGIONE,
                                                         CG_SYSTEM_ID = cg.SYSTEM_ID,
                                                         CG_ID_GRUPPO = cg.ID_GRUPPO,
                                                         CG_ID_PEOPLE = cg.ID_PEOPLE,
                                                         CG_VAR_CODICE = cg.VAR_CODICE,
                                                         CG_VAR_DESC_CORR = cg.VAR_DESC_CORR,
                                                         CG_VAR_COGNOME = cg.VAR_COGNOME,
                                                         CG_VAR_NOME = cg.VAR_NOME
                                                     })
                                .ToListAsync();

            foreach (var tsEntity in trasmissioniSingoleEntities)
            {
                if (tsEntity.TS_CHA_TIPO_DEST == "R")
                {
                    aggregate.AddTrasmissioneSingolaGruppo(
                        tsEntity.TS_SYSTEM_ID.ToString(),
                        tsEntity.RG_SYSTEM_ID.ToString(),
                        tsEntity.RG_VAR_DESC_RAGIONE,
                        tsEntity.RG_CHA_TIPO_RAGIONE == "W",
                        tsEntity.TS_CHA_TIPO_TRASM == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                        tsEntity.CG_ID_GRUPPO.ToString(),
                        tsEntity.CG_VAR_CODICE,
                        new TextValue(tsEntity.CG_VAR_DESC_CORR),
                        !string.IsNullOrWhiteSpace(tsEntity.TS_VAR_NOTE_SING) ? new TextValue(tsEntity.TS_VAR_NOTE_SING) : null,
                        tsEntity.TS_DTA_SCADENZA,
                        tsEntity.TS_HIDE_DOC_VERSIONS == "1");
                }
                else
                {
                    aggregate.AddTrasmissioneSingolaUtente(
                        tsEntity.TS_SYSTEM_ID.ToString(),
                        tsEntity.RG_SYSTEM_ID.ToString(),
                        tsEntity.RG_VAR_DESC_RAGIONE,
                        tsEntity.RG_CHA_TIPO_RAGIONE == "W",
                        tsEntity.CG_ID_PEOPLE.ToString(),
                        tsEntity.CG_VAR_CODICE,
                        tsEntity.CG_VAR_COGNOME,
                        tsEntity.CG_VAR_NOME,
                        !string.IsNullOrWhiteSpace(tsEntity.TS_VAR_NOTE_SING) ? new TextValue(tsEntity.TS_VAR_NOTE_SING) : null,
                        tsEntity.TS_DTA_SCADENZA,
                        tsEntity.TS_HIDE_DOC_VERSIONS == "1");
                }

                var trasmissioniUtenteEntities =
                    await (from tu in _dbContext.TrasmUtenteEntities.AsNoTracking()
                           join p in _dbContext.PeopleEntities.AsNoTracking() on tu.ID_PEOPLE equals p.SYSTEM_ID
                           join p2 in _dbContext.PeopleEntities.AsNoTracking() on tu.ID_PEOPLE_DELEGATO equals p2.SYSTEM_ID into pDelGruping
                           from pDel in pDelGruping.DefaultIfEmpty()
                           where tu.ID_TRASM_SINGOLA == tsEntity.TS_SYSTEM_ID
                           select new
                           {
                               TU_SYSTEM_ID = tu.SYSTEM_ID,
                               TU_ID_PEOPLE = tu.ID_PEOPLE,
                               TU_DTA_RIMOZIONE_TODOLIST = tu.DTA_RIMOZIONE_TODOLIST,
                               TU_DTA_VISTA = tu.DTA_VISTA,
                               TU_DTA_ACCETTATA = tu.DTA_ACCETTATA,
                               TU_DTA_RIFIUTATA = tu.DTA_RIFIUTATA,
                               TU_VAR_NOTE_ACC = tu.VAR_NOTE_ACC,
                               TU_VAR_NOTE_RIF = tu.VAR_NOTE_RIF,
                               TU_ID_PEOPLE_DELEGATO = tu.ID_PEOPLE_DELEGATO,
                               P_USER_ID = p.USER_ID,
                               P_VAR_COGNOME = p.VAR_COGNOME,
                               P_VAR_NOME = p.VAR_NOME,
                               PDEL_USER_ID = pDel.USER_ID,
                               PDEL_VAR_COGNOME = pDel.VAR_COGNOME,
                               PDEL_VAR_NOME = pDel.VAR_NOME
                           })
                    .ToListAsync();

                foreach (var tuEntity in trasmissioniUtenteEntities)
                {
                    aggregate.AddTrasmissioneUtente(
                        tsEntity.TS_SYSTEM_ID.ToString(),
                        tuEntity.TU_SYSTEM_ID.ToString(),
                        tuEntity.TU_ID_PEOPLE.ToString(),
                        tuEntity.P_USER_ID,
                        tuEntity.P_VAR_COGNOME,
                        tuEntity.P_VAR_NOME,
                        tuEntity.TU_DTA_RIMOZIONE_TODOLIST);

                    long idGruppoDest = 0;
                    if (tsEntity.TS_CHA_TIPO_DEST == "R" && tsEntity.CG_ID_GRUPPO.HasValue)
                        idGruppoDest = tsEntity.CG_ID_GRUPPO.Value;

                    if (tuEntity.TU_DTA_VISTA.HasValue)
                    {
                        aggregate.Visto(idGruppoDest.ToString(), tuEntity.TU_ID_PEOPLE.ToString(),
                            new Visto()
                            {
                                Data = tuEntity.TU_DTA_VISTA
                            });
                    }

                    if (tuEntity.TU_DTA_ACCETTATA.HasValue)
                    {
                        aggregate.Accetta(idGruppoDest.ToString(), tuEntity.TU_ID_PEOPLE.ToString(),
                            new Accetta()
                            {
                                Data = tuEntity.TU_DTA_ACCETTATA,
                                Note = !string.IsNullOrWhiteSpace(tuEntity.TU_VAR_NOTE_ACC) ? new TextValue(tuEntity.TU_VAR_NOTE_ACC) : null,
                                IdDelegato = tuEntity.TU_ID_PEOPLE_DELEGATO.HasValue ? tuEntity.TU_ID_PEOPLE_DELEGATO.ToString() : null,
                                UserIdDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_USER_ID) ? tuEntity.PDEL_USER_ID : null,
                                CognomeDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_VAR_COGNOME) ? tuEntity.PDEL_VAR_COGNOME : null,
                                NomeDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_VAR_NOME) ? tuEntity.PDEL_VAR_NOME : null
                            });
                    }
                    else if (tuEntity.TU_DTA_RIFIUTATA.HasValue)
                    {
                        if (tsEntity.RG_CHA_TIPO_RAGIONE == "W")
                        {
                            aggregate.RifiutaTrasmissioneUtente(tuEntity.TU_SYSTEM_ID.ToString(),
                                new Rifiuta()
                                {
                                    Data = tuEntity.TU_DTA_RIFIUTATA,
                                    Note = !string.IsNullOrWhiteSpace(tuEntity.TU_VAR_NOTE_RIF) ? new TextValue(tuEntity.TU_VAR_NOTE_RIF) : null,
                                    IdDelegato = tuEntity.TU_ID_PEOPLE_DELEGATO.HasValue ? tuEntity.TU_ID_PEOPLE_DELEGATO.ToString() : null,
                                    UserIdDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_USER_ID) ? tuEntity.PDEL_USER_ID : null,
                                    CognomeDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_VAR_COGNOME) ? tuEntity.PDEL_VAR_COGNOME : null,
                                    NomeDelegato = !string.IsNullOrWhiteSpace(tuEntity.PDEL_VAR_NOME) ? tuEntity.PDEL_VAR_NOME : null
                                });
                        }
                    }
                }
            }

            if (trasmissioneEntity.DTA_INVIO.HasValue)
                aggregate.Invia(trasmissioneEntity.DTA_INVIO.Value);

            aggregate.MarkChangesAsCommitted();

            return aggregate;
        }

        protected override async Task HandleAdd(Trasmissione aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override async Task HandleDelete(Trasmissione aggregate)
        {
            if (aggregate.DataInvio.HasValue)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            var trasmissioneEntity = await this._dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            if (trasmissioneEntity.DTA_INVIO.HasValue)
                throw new TrasmissionePi3Exception(ErrorDescriptions.TrasmissioneGiaInviata, ErrorDescriptions.ResourceManager, aggregate.Id);

            var trasmissioniSingoleEntities = await this._dbContext.TrasmSingolaEntities
                   .Where(ts => ts.ID_TRASMISSIONE == trasmissioneEntity.SYSTEM_ID)
                   .Select(ts => ts)
                   .ToListAsync();

            if (trasmissioniSingoleEntities.Count > 0)
            {
                this._dbContext.TrasmSingolaEntities.RemoveRange(trasmissioniSingoleEntities);
            }

            foreach (var trasmisisoneSingolaEntity in trasmissioniSingoleEntities)
            {
                var trasmissioniUtenteEntities = await this._dbContext.TrasmUtenteEntities
                    .Where(tu => tu.ID_TRASM_SINGOLA == trasmisisoneSingolaEntity.SYSTEM_ID)
                    .Select(tu => tu)
                    .ToListAsync();

                if (trasmissioniUtenteEntities.Count > 0)
                {
                    this._dbContext.TrasmUtenteEntities.RemoveRange(trasmissioniUtenteEntities);
                }
            }


            this._dbContext.TrasmissioneEntities.Remove(trasmissioneEntity);


            await ((DbContext)this._dbContext).SaveChangesAsync();

        }

        protected override async Task HandleUpdate(Trasmissione aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected virtual async Task HandleChanges(Trasmissione aggregate)
        {
            var uncommitted = new List<dynamic>(aggregate.GetUncommittedChanges());

            foreach (var @event in uncommitted)
            {
                var handleMethod = GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => m.Name == "Handle"
                        && m.GetParameters().Any(p => p.ParameterType == @event.GetType()));

                if (handleMethod != null)
                    await this.Handle(@event, aggregate);
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected virtual void AssertTrasmissioneInviata(TrasmissioneEntity trasmissioneEntity)
        {
            if (trasmissioneEntity.SYSTEM_ID > 0 && trasmissioneEntity.DTA_INVIO.HasValue)
                throw new TrasmissionePi3Exception(ErrorDescriptions.TrasmissioneGiaInviata, ErrorDescriptions.ResourceManager, trasmissioneEntity.SYSTEM_ID.ToString());
        }

        protected virtual void AssertTrasmissioneNonInviata(TrasmissioneEntity trasmissioneEntity)
        {
            if (!trasmissioneEntity.DTA_INVIO.HasValue)
                throw new TrasmissionePi3Exception(ErrorDescriptions.TrasmissioneNonInviata, ErrorDescriptions.ResourceManager, trasmissioneEntity.SYSTEM_ID.ToString());
        }

        protected virtual async Task Handle(TrasmissioneCreataEvent @event, Trasmissione aggregate)
        {
            string idUser = @event.Autore != null ? @event.Autore.IdUtente : _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdUser, true);
            string idGroup = @event.Autore != null ? @event.Autore.IdGruppo : _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdGroup, true);
            string idDelegatedUser = @event.Autore != null ? @event.Autore.IdUtenteDelegato : _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.DelegatedIdUser);
            var isUtenteAutomatico = await _dbContext.PeopleEntities.AsNoTracking().AnyAsync(p => p.SYSTEM_ID == idUser.AsLong() && p.CHA_AUTOMATICO == "1");

            //L'utente automatico non sta in nessun ruolo, quindi lui può comunque trasmettere
            if (!isUtenteAutomatico
                && !await _dbContext.PeopleGroupEntities.AsNoTracking()
                .AnyAsync(pg => pg.PEOPLE_SYSTEM_ID == idUser.AsLong()
                        && pg.GROUPS_SYSTEM_ID == idGroup.AsLong()
                        && pg.DTA_FINE == null))
            {
                throw new AutoreTrasmissioneNonValidoPi3Exception(idUser, idGroup);
            }

            var trasmissioneEntity = new TrasmissioneEntity();

            await _dbContext.TrasmissioneEntities.AddAsync(trasmissioneEntity);

            this.LoadAggregateFromHistory(aggregate,
                    new ElementIdAssignedEvent()
                    {
                        Id = trasmissioneEntity.SYSTEM_ID.ToString()
                    });

            trasmissioneEntity.ID_PEOPLE = idUser.AsLong();
            trasmissioneEntity.ID_RUOLO_IN_UO = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                                                .Where(cg => cg.ID_GRUPPO == idGroup.AsLong())
                                                .Select(cg => cg.SYSTEM_ID)
                                                .FirstAsync();

            if (!string.IsNullOrWhiteSpace(idDelegatedUser))
                trasmissioneEntity.ID_PEOPLE_DELEGATO = idDelegatedUser.AsLong();

            if (@event.TipoOggettoTrasmesso == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
            {
                trasmissioneEntity.CHA_TIPO_OGGETTO = "D";
                trasmissioneEntity.ID_PROFILE = @event.IdOggettoTrasmesso.AsLong();
            }
            else if (@event.TipoOggettoTrasmesso == TipiOggettiTrasmessiEnum.Fascicolo)
            {
                trasmissioneEntity.CHA_TIPO_OGGETTO = "F";
                trasmissioneEntity.ID_PROJECT = @event.IdOggettoTrasmesso.AsLong();
            }

            trasmissioneEntity.CHA_SALVATA_CON_CESSIONE = 0.ToString();
            trasmissioneEntity.VAR_NOTE_GENERALI = @event.NoteGenerali != null ? @event.NoteGenerali.ToString() : null;
        }

        protected virtual async Task Handle(NoteGeneraliChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            trasmissioneEntity.VAR_NOTE_GENERALI = @event.NewNoteGenerali != null ? @event.NewNoteGenerali.ToString() : null;
        }

        protected virtual async Task Handle(CediDirittiChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            trasmissioneEntity.CHA_SALVATA_CON_CESSIONE = @event.NewCediDiritti ? "1" : "0";
        }

        protected virtual async Task Handle(TrasmissioneInviataEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());
            var idTenantAsLong = aggregate.IdTenant.AsLong();
            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var newSecurityEntities = new List<SecurityEntity>();
            var securityEntitiesToRemove = new List<SecurityEntity>();

            var currentDateTime = await _dbContext.GetSystemDateTime();

            trasmissioneEntity.DTA_INVIO = currentDateTime;

            //gestione cessione dirittti
            var trasmissioneSingolaConCessione = aggregate.TrasmissioniSingole.Where(s => s.RagioneTrasmissione.CessioneDirittiRagione != null).Select(s => s).FirstOrDefault();

            if (trasmissioneSingolaConCessione != null
                && (aggregate.TrasmissioniSingole.Any(s => !s.RagioneTrasmissione.CessioneDirittiRagione.ConSceltaUtente)
                    || aggregate.CediDiritti.HasValue && (bool)aggregate.CediDiritti))
            {
                var cessioneDiritti = trasmissioneSingolaConCessione.RagioneTrasmissione.CessioneDirittiRagione;

                var tipoDiritto = string.Empty;

                long idOggettoTrasmesso = aggregate.OggettoTrasmesso.Id.AsLong();
                long? idOggettoTrasmessoFolder = null;
                long idGruppoOld = aggregate.Autore != null ? aggregate.Autore.IdGruppo.AsLong() : _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                long idUtenteOld = aggregate.Autore != null ? aggregate.Autore.IdUtente.AsLong() : _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                string userIdAutore = aggregate.Autore != null ? aggregate.Autore.UserId : _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.UserId, true);
                long? idGruppoNew = null;
                long? idUtenteNew = null;

                var personale = false;
                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
                {
                    var profileEntity = await _dbContext.ProfileEntities.AsNoTracking()
                        .Where(p => p.DOCNUMBER == idOggettoTrasmesso)
                        .Select(c => new
                        {
                            c.CHA_PERSONALE
                        })
                        .FirstAsync();

                    personale = profileEntity.CHA_PERSONALE == "1";
                }
                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                {
                    // Per il fascicolo, le stesse ACL devo essere inserite anche per il record C
                    idOggettoTrasmessoFolder = await _dbContext.ProjectEntities.AsNoTracking()
                        .Where(p => p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong() && p.CHA_TIPO_PROJ == "C")
                        .Select(p => p.SYSTEM_ID)
                        .FirstAsync();
                }

                if (trasmissioneSingolaConCessione.GetType() == typeof(TrasmissioneSingolaGruppo))
                {
                    var tsRuolo = (TrasmissioneSingolaGruppo)trasmissioneSingolaConCessione;
                    idGruppoNew = tsRuolo.GruppoDestinatario.Id.AsLong();
                    idUtenteNew = tsRuolo.TrasmissioniUtente[0].UtenteDestinatario.Id.AsLong();
                }

                if (trasmissioneSingolaConCessione.GetType() == typeof(TrasmissioneSingolaUtente))
                {
                    var tsUtente = (TrasmissioneSingolaUtente)trasmissioneSingolaConCessione;
                    idUtenteNew = tsUtente.UtenteDestinatario.Id.AsLong();
                }

                //Estraggo i diritti del ruolo/utente mittente
                var utenteOldSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), idUtenteOld.ToString(), null);
                var ruoloOldSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), 0.ToString(), idGruppoOld.ToString());

                var keyCediDirittiRuolo = (await this._configurationService.GetValue<string>(idTenantAsLong.ToString(), "FE_CEDI_DIRITTI_IN_RUOLO", false, "0")) == "1";

                var ruoloProprietario = ruoloOldSecurityEntity != null && ruoloOldSecurityEntity.ACCESSRIGHTS == 255;
                var utenteProprietario = utenteOldSecurityEntity != null && utenteOldSecurityEntity.ACCESSRIGHTS == 0 || ruoloProprietario && keyCediDirittiRuolo;

                if ((utenteProprietario || personale) && ruoloProprietario)
                {
                    tipoDiritto = Descriptions.TipoDirittoProprieta;

                    if(utenteOldSecurityEntity == null)
                    {
                        //recupero l'utente proprietario per la rimozione dei diritti
                        utenteOldSecurityEntity = await _dbContext.SecurityEntities.Where(s => s.THING == idOggettoTrasmesso && s.ACCESSRIGHTS == 0).FirstOrDefaultAsync();
                    }

                    if (utenteOldSecurityEntity != null)
                    {
                        _dbContext.SecurityEntities.Remove(utenteOldSecurityEntity);

                        var utenteOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                            .Where(d => d.PERSONORGROUP == idUtenteOld && d.ACCESSRIGHTS == 0 && d.THING == idOggettoTrasmesso)
                            .ToListAsync();
                        _dbContext.DeletedSecurityEntities.RemoveRange(utenteOldDeletedSecurityEntity);

                        DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                        await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                        {
                            ACCESSRIGHTS = 0,
                            PERSONORGROUP = idUtenteOld,
                            THING = idOggettoTrasmesso,
                            CHA_TIPO_DIRITTO = "P",
                            ID_GRUPPO_TRASM = null,
                            NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                            DTA_REVOCA = dataRevoca,
                            ID_UTENTE_REV = idUtenteNew,
                            ID_RUOLO_REV = idGruppoNew,
                            CHA_COPIA_VISIBILITA = null
                        });

                        var newUtenteSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), idUtenteNew.ToString());
                        if (newUtenteSecurityEntity != null)
                            _dbContext.SecurityEntities.RemoveRange(newUtenteSecurityEntity);

                        newSecurityEntities.AddIfNotExists(new SecurityEntity
                        {
                            PERSONORGROUP = idUtenteNew,
                            ACCESSRIGHTS = 0,
                            THING = idOggettoTrasmesso,
                            CHA_TIPO_DIRITTO = "P",
                            ID_GRUPPO_TRASM = null
                        });

                        if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                        {
                            var utenteOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idUtenteOld && s.ACCESSRIGHTS == 0)
                                .FirstOrDefaultAsync();

                            if (utenteOldFolderSecurityEntity != null)
                                _dbContext.SecurityEntities.Remove(utenteOldFolderSecurityEntity);

                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idUtenteNew,
                                ACCESSRIGHTS = 0,
                                THING = idOggettoTrasmessoFolder,
                                CHA_TIPO_DIRITTO = "P",
                                ID_GRUPPO_TRASM = null
                            });

                        }
                    }
                    if (ruoloOldSecurityEntity != null && idGruppoOld != idGruppoNew)
                    {
                        _dbContext.SecurityEntities.Remove(ruoloOldSecurityEntity);

                        var ruoloOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                            .Where(d => d.PERSONORGROUP == idGruppoOld && d.ACCESSRIGHTS == 255 && d.THING == idOggettoTrasmesso)
                            .ToListAsync();
                        _dbContext.DeletedSecurityEntities.RemoveRange(ruoloOldDeletedSecurityEntity);

                        DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                        await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                        {
                            ACCESSRIGHTS = 255,
                            PERSONORGROUP = idGruppoOld,
                            THING = idOggettoTrasmesso,
                            CHA_TIPO_DIRITTO = "P",
                            ID_GRUPPO_TRASM = null,
                            NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                            DTA_REVOCA = dataRevoca,
                            ID_UTENTE_REV = idUtenteNew,
                            ID_RUOLO_REV = idGruppoNew,
                            CHA_COPIA_VISIBILITA = null
                        });

                        var newGruppoSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), 0.ToString(), idGruppoNew.ToString());
                        if (newGruppoSecurityEntity != null)
                            _dbContext.SecurityEntities.RemoveRange(newGruppoSecurityEntity);

                        newSecurityEntities.AddIfNotExists(new SecurityEntity
                        {
                            PERSONORGROUP = idGruppoNew,
                            ACCESSRIGHTS = 255,
                            THING = idOggettoTrasmesso,
                            CHA_TIPO_DIRITTO = "P",
                            ID_GRUPPO_TRASM = null
                        });

                        if (cessioneDiritti.MantieniScrittura || cessioneDiritti.MantieniLettura)
                        {
                            var accessright = cessioneDiritti.MantieniScrittura ? 63 : 45;
                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idGruppoOld,
                                ACCESSRIGHTS = accessright,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = "A",
                                ID_GRUPPO_TRASM = null
                            });
                        }

                        if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                        {
                            var ruoloOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idGruppoOld && s.ACCESSRIGHTS == 255)
                                .FirstOrDefaultAsync();

                            if (ruoloOldFolderSecurityEntity != null)
                                _dbContext.SecurityEntities.Remove(ruoloOldFolderSecurityEntity);

                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idGruppoNew,
                                ACCESSRIGHTS = 255,
                                THING = idOggettoTrasmessoFolder,
                                CHA_TIPO_DIRITTO = "P",
                                ID_GRUPPO_TRASM = null
                            });

                            if (cessioneDiritti.MantieniScrittura || cessioneDiritti.MantieniLettura)
                            {
                                var accessright = cessioneDiritti.MantieniScrittura ? 63 : 45;
                                newSecurityEntities.AddIfNotExists(new SecurityEntity
                                {
                                    PERSONORGROUP = idGruppoOld,
                                    ACCESSRIGHTS = accessright,
                                    THING = idOggettoTrasmessoFolder,
                                    CHA_TIPO_DIRITTO = "A",
                                    ID_GRUPPO_TRASM = null
                                });
                            }
                        }
                    }
                }
                else
                {
                    //Cedo i diritti acquisiti

                    // CASI POSSIBILI:
                    // 1. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di Scrittura e si possiedono solo quelli
                    //    la cessione del nuovo ruolo nn viene eseguita.
                    // 2. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono solo quelli 
                    //    la cessione del nuovo ruolo nn viene eseguita
                    // 3. Se Cedo i Diritti ed è attiva l'opzione di mantenere i diritti di lettura e si possiedono quelli di Scrittura
                    //    deve avvenire la cessione del nuovo ruolo
                    tipoDiritto = Descriptions.TipoDirittoAcquisito;
                    if (utenteOldSecurityEntity != null && !utenteProprietario && idUtenteNew != null)
                    {
                        bool cediDirittiUtente = false;
                        if (!cessioneDiritti.MantieniLettura && !cessioneDiritti.MantieniScrittura)
                        {
                            cediDirittiUtente = true;
                            _dbContext.SecurityEntities.Remove(utenteOldSecurityEntity);

                            var utenteOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                                       .Where(d => d.PERSONORGROUP == idUtenteOld && d.ACCESSRIGHTS == (long)utenteOldSecurityEntity.ACCESSRIGHTS && d.THING == idOggettoTrasmesso)
                                       .ToListAsync();
                            _dbContext.DeletedSecurityEntities.RemoveRange(utenteOldDeletedSecurityEntity);

                            DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                            await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                            {
                                ACCESSRIGHTS = (long)utenteOldSecurityEntity.ACCESSRIGHTS,
                                PERSONORGROUP = idUtenteOld,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = utenteOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null,
                                NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                                DTA_REVOCA = dataRevoca,
                                ID_UTENTE_REV = idUtenteNew,
                                ID_RUOLO_REV = idGruppoNew,
                                CHA_COPIA_VISIBILITA = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var utenteOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idUtenteOld && s.ACCESSRIGHTS == 0)
                                    .FirstOrDefaultAsync();

                                if (utenteOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(utenteOldFolderSecurityEntity);
                            }

                        }
                        else if (cessioneDiritti.MantieniScrittura && utenteOldSecurityEntity.ACCESSRIGHTS != 63)
                        {
                            _dbContext.SecurityEntities.Remove(utenteOldSecurityEntity);

                            var utenteOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                                                                   .Where(d => d.PERSONORGROUP == idUtenteOld && d.ACCESSRIGHTS == (long)utenteOldSecurityEntity.ACCESSRIGHTS && d.THING == idOggettoTrasmesso)
                                                                   .ToListAsync();
                            _dbContext.DeletedSecurityEntities.RemoveRange(utenteOldDeletedSecurityEntity);

                            DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                            await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                            {
                                ACCESSRIGHTS = (long)utenteOldSecurityEntity.ACCESSRIGHTS,
                                PERSONORGROUP = idUtenteOld,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = utenteOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null,
                                NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                                DTA_REVOCA = dataRevoca,
                                ID_UTENTE_REV = idUtenteNew,
                                ID_RUOLO_REV = idGruppoNew,
                                CHA_COPIA_VISIBILITA = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var utenteOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idUtenteOld && s.ACCESSRIGHTS == 0)
                                    .FirstOrDefaultAsync();

                                if (utenteOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(utenteOldFolderSecurityEntity);
                            }
                        }
                        else if (cessioneDiritti.MantieniLettura && !cessioneDiritti.MantieniScrittura && utenteOldSecurityEntity.ACCESSRIGHTS == 63)
                        {
                            cediDirittiUtente = true;
                            _dbContext.SecurityEntities.Remove(utenteOldSecurityEntity);
                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idUtenteOld,
                                ACCESSRIGHTS = 45,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = "A",
                                ID_GRUPPO_TRASM = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var utenteOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idUtenteOld && s.ACCESSRIGHTS == 0)
                                    .FirstOrDefaultAsync();

                                if (utenteOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(utenteOldFolderSecurityEntity);

                                newSecurityEntities.AddIfNotExists(new SecurityEntity
                                {
                                    PERSONORGROUP = idUtenteOld,
                                    ACCESSRIGHTS = 45,
                                    THING = idOggettoTrasmessoFolder,
                                    CHA_TIPO_DIRITTO = "A",
                                    ID_GRUPPO_TRASM = null
                                });

                            }
                        }

                        if (cediDirittiUtente)
                        {
                            var newUtenteSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), idUtenteNew.ToString());
                            if (newUtenteSecurityEntity != null)
                                _dbContext.SecurityEntities.RemoveRange(newUtenteSecurityEntity);

                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idUtenteNew,
                                ACCESSRIGHTS = (long)utenteOldSecurityEntity.ACCESSRIGHTS,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = utenteOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                newSecurityEntities.AddIfNotExists(new SecurityEntity
                                {
                                    PERSONORGROUP = idUtenteNew,
                                    ACCESSRIGHTS = (long)utenteOldSecurityEntity.ACCESSRIGHTS,
                                    THING = idOggettoTrasmessoFolder,
                                    CHA_TIPO_DIRITTO = utenteOldSecurityEntity.CHA_TIPO_DIRITTO,
                                    ID_GRUPPO_TRASM = null
                                });
                            }
                        }
                    }

                    if (ruoloOldSecurityEntity != null && !ruoloProprietario && idGruppoNew != null && idGruppoOld != idGruppoNew)
                    {
                        bool cediDirittiRuolo = false;
                        if (!cessioneDiritti.MantieniLettura && !cessioneDiritti.MantieniScrittura)
                        {
                            cediDirittiRuolo = true;
                            _dbContext.SecurityEntities.Remove(ruoloOldSecurityEntity);

                            var ruoloOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                                    .Where(d => d.PERSONORGROUP == idGruppoOld && d.ACCESSRIGHTS == (long)ruoloOldSecurityEntity.ACCESSRIGHTS && d.THING == idOggettoTrasmesso)
                                    .ToListAsync();
                            _dbContext.DeletedSecurityEntities.RemoveRange(ruoloOldDeletedSecurityEntity);

                            DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                            await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                            {
                                ACCESSRIGHTS = (long)ruoloOldSecurityEntity.ACCESSRIGHTS,
                                PERSONORGROUP = idGruppoOld,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = ruoloOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null,
                                NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                                DTA_REVOCA = dataRevoca,
                                ID_UTENTE_REV = idUtenteNew,
                                ID_RUOLO_REV = idGruppoNew,
                                CHA_COPIA_VISIBILITA = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var ruoloOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idGruppoOld && s.ACCESSRIGHTS == 255)
                                    .FirstOrDefaultAsync();

                                if (ruoloOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(ruoloOldFolderSecurityEntity);
                            }
                        }
                        else if (cessioneDiritti.MantieniScrittura && ruoloOldSecurityEntity.ACCESSRIGHTS != 63)
                        {
                            _dbContext.SecurityEntities.Remove(ruoloOldSecurityEntity);
                            var ruoloOldDeletedSecurityEntity = await _dbContext.DeletedSecurityEntities.AsNoTracking()
                                                                .Where(d => d.PERSONORGROUP == idGruppoOld && d.ACCESSRIGHTS == (long)ruoloOldSecurityEntity.ACCESSRIGHTS && d.THING == idOggettoTrasmesso)
                                                                .ToListAsync();
                            _dbContext.DeletedSecurityEntities.RemoveRange(ruoloOldDeletedSecurityEntity);

                            DateTime dataRevoca = await _dbContext.GetSystemDateTime();
                            await _dbContext.DeletedSecurityEntities.AddAsync(new DeletedSecurityEntity()
                            {
                                ACCESSRIGHTS = (long)ruoloOldSecurityEntity.ACCESSRIGHTS,
                                PERSONORGROUP = idUtenteOld,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = ruoloOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null,
                                NOTE = string.Format(Descriptions.DirittoCedudoDa, userIdAutore),
                                DTA_REVOCA = dataRevoca,
                                ID_UTENTE_REV = idUtenteNew,
                                ID_RUOLO_REV = idGruppoNew,
                                CHA_COPIA_VISIBILITA = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var ruoloOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idGruppoOld && s.ACCESSRIGHTS == 255)
                                    .FirstOrDefaultAsync();

                                if (ruoloOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(ruoloOldFolderSecurityEntity);
                            }
                        }
                        else if (cessioneDiritti.MantieniLettura && !cessioneDiritti.MantieniScrittura && ruoloOldSecurityEntity.ACCESSRIGHTS == 63)
                        {
                            cediDirittiRuolo = true;
                            _dbContext.SecurityEntities.Remove(ruoloOldSecurityEntity);
                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idUtenteOld,
                                ACCESSRIGHTS = 45,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = "A",
                                ID_GRUPPO_TRASM = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                var ruoloOldFolderSecurityEntity = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == idOggettoTrasmessoFolder && s.PERSONORGROUP == idGruppoOld && s.ACCESSRIGHTS == 255)
                                    .FirstOrDefaultAsync();

                                if (ruoloOldFolderSecurityEntity != null)
                                    _dbContext.SecurityEntities.Remove(ruoloOldFolderSecurityEntity);

                                newSecurityEntities.AddIfNotExists(new SecurityEntity
                                {
                                    PERSONORGROUP = idGruppoOld,
                                    ACCESSRIGHTS = 45,
                                    THING = idOggettoTrasmessoFolder,
                                    CHA_TIPO_DIRITTO = "A",
                                    ID_GRUPPO_TRASM = null
                                });
                            }
                        }

                        if (cediDirittiRuolo)
                        {
                            var newGruppoSecurityEntity = await _dbContext.GetSecurity(idOggettoTrasmesso.ToString(), 0.ToString(), idGruppoNew.ToString());
                            if (newGruppoSecurityEntity != null)
                                _dbContext.SecurityEntities.RemoveRange(newGruppoSecurityEntity);

                            newSecurityEntities.AddIfNotExists(new SecurityEntity
                            {
                                PERSONORGROUP = idGruppoNew,
                                ACCESSRIGHTS = ruoloOldSecurityEntity.ACCESSRIGHTS,
                                THING = idOggettoTrasmesso,
                                CHA_TIPO_DIRITTO = ruoloOldSecurityEntity.CHA_TIPO_DIRITTO,
                                ID_GRUPPO_TRASM = null
                            });

                            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo && idOggettoTrasmessoFolder != null)
                            {
                                newSecurityEntities.AddIfNotExists(new SecurityEntity
                                {
                                    PERSONORGROUP = idGruppoNew,
                                    ACCESSRIGHTS = ruoloOldSecurityEntity.ACCESSRIGHTS,
                                    THING = idOggettoTrasmessoFolder,
                                    CHA_TIPO_DIRITTO = ruoloOldSecurityEntity.CHA_TIPO_DIRITTO,
                                    ID_GRUPPO_TRASM = null
                                });
                            }
                        }
                    }
                }
            }

            var securityEntities = await _dbContext.SecurityEntities.AsNoTracking()
                                    .Where(s => s.THING == aggregate.OggettoTrasmesso.Id.AsLong())
                                    .Select(s => s)
                                    .ToListAsync();

            var idGruppoTrasm = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            List<long>? idProjectC = null;
            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
            {
                // Per il fascicolo, le stesse ACL devo essere inserite anche per il record C
                idProjectC = await _dbContext.ProjectEntities.AsNoTracking()
                    .Where(p => p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong())
                    .Select(p => p.SYSTEM_ID)
                    .ToListAsync();
            }

            foreach (var trasmissioneSingola in aggregate.TrasmissioniSingole)
            {
                var ragioneTrasmissioneEntity = await _dbContext.RagioneTrasmissioneEntities.FindAsync(trasmissioneSingola.RagioneTrasmissione.Id.AsLong());
                if (ragioneTrasmissioneEntity == null)
                    throw new RagioneTrasmissioneNotFoundPi3Exception(trasmissioneSingola.RagioneTrasmissione.Id);

                long personOrGroupDestinatario = 0;
                bool trasmissioneGruppo = false;

                if (trasmissioneSingola.GetType() == typeof(TrasmissioneSingolaGruppo))
                {
                    var tsRuolo = (TrasmissioneSingolaGruppo)trasmissioneSingola;
                    personOrGroupDestinatario = tsRuolo.GruppoDestinatario.Id.AsLong();
                    trasmissioneGruppo = true;
                }
                else if (trasmissioneSingola.GetType() == typeof(TrasmissioneSingolaUtente))
                {
                    var tsUtente = (TrasmissioneSingolaUtente)trasmissioneSingola;
                    personOrGroupDestinatario = tsUtente.UtenteDestinatario.Id.AsLong();
                }

                var accessRights = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45;
                if (ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE == "W")
                {
                    // il diritto a 20 va comunque inserito per trasmissioni a utente di cui l'utente è già proprietario(accessrights 0)
                    if (!securityEntities.Any(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS >= 20)) // per le trasmissioni ad utente di cui l'utente è propritario(accessright 0)
                    {
                        // Se il destinatario non ha ancora i diritti sull'oggetto

                        // Se la trasmissione prevede workflow, inserisce il diritto in attesa di accettazione
                        newSecurityEntities.AddIfNotExists(new SecurityEntity()
                        {
                            THING = aggregate.OggettoTrasmesso.Id.AsLong(),
                            PERSONORGROUP = personOrGroupDestinatario,
                            ACCESSRIGHTS = 20,
                            ID_GRUPPO_TRASM = idGruppoTrasm,
                            CHA_TIPO_DIRITTO = "A",
                            HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                        });

                        if (idProjectC != null && idProjectC.Any())
                        {
                            // Se l'oggetto trasmesso è un fascicolo, è necessario aggiungere l'ACL anche per il record C
                            idProjectC.ForEach(id =>
                            {
                                newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                {
                                    THING = id,
                                    PERSONORGROUP = personOrGroupDestinatario,
                                    ACCESSRIGHTS = 20,
                                    ID_GRUPPO_TRASM = idGruppoTrasm,
                                    CHA_TIPO_DIRITTO = "F",
                                    HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                                });
                            });
                        }
                    }
                }
                else
                {
                    var dirittiDestinatario = securityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS != 0 && s.ACCESSRIGHTS < accessRights).ToList();
                    if (dirittiDestinatario.Any())
                    {
                        securityEntitiesToRemove.AddRange(dirittiDestinatario);
                        if (idProjectC != null && idProjectC.Any())
                        {
                            securityEntitiesToRemove.AddRange(await _dbContext.SecurityEntities.AsNoTracking()
                                   .Where(s => s.PERSONORGROUP == personOrGroupDestinatario && idProjectC.Contains(s.THING.Value) && s.ACCESSRIGHTS != 0 && s.ACCESSRIGHTS < accessRights)
                                   .Select(s => s)
                                   .ToListAsync());
                        }
                    }

                    //Controllo se al ruolo destinatario sono stati ceduti diritti di proprietà perchè la ragione prevdeva cessione, in caso non inserisco nuovamente
                    var existsOwnerNewSecurity = newSecurityEntities.Any(s => s.ACCESSRIGHTS == 255 && s.PERSONORGROUP == personOrGroupDestinatario);

                    if (!securityEntities.Any(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS >= accessRights) && !existsOwnerNewSecurity)
                    {
                        // Se il destinatario non ha ancora i diritti sull'oggetto

                        // Se la trasmissione prevede workflow, inserisce il diritto in attesa di accettazione
                        newSecurityEntities.AddIfNotExists(new SecurityEntity()
                        {
                            THING = aggregate.OggettoTrasmesso.Id.AsLong(),
                            PERSONORGROUP = personOrGroupDestinatario,
                            ACCESSRIGHTS = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45,
                            ID_GRUPPO_TRASM = idGruppoTrasm,
                            CHA_TIPO_DIRITTO = "T",
                            HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                        });

                        if (idProjectC != null && idProjectC.Any())
                        {
                            // Se l'oggetto trasmesso è un fascicolo, è necessario aggiungere l'ACL anche per il record C
                            idProjectC.ForEach(id =>
                            {
                                newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                {
                                    THING = id,
                                    PERSONORGROUP = personOrGroupDestinatario,
                                    ACCESSRIGHTS = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45,
                                    ID_GRUPPO_TRASM = idGruppoTrasm,
                                    CHA_TIPO_DIRITTO = "F",
                                    HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                                });
                            });
                        }
                    }

                    // Se la ragione trasmissione prevede workflow, inserisce il diritto con risalita gerarchica
                    if (ragioneTrasmissioneEntity.CHA_EREDITA == "1" &&  trasmissioneGruppo)
                    {
                        //MEV 2020:Se il documento è privato o ad utente non estendo la visibilita ai superiori
                        var isPrivato = false;
                        if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                        {
                            isPrivato = await _dbContext.ProjectEntities.AsNoTracking()
                                .AnyAsync(p => p.SYSTEM_ID == aggregate.OggettoTrasmesso.Id.AsLong() && p.CHA_PRIVATO == "1");
                        }
                        else
                        {
                            isPrivato = await _dbContext.ProfileEntities.AsNoTracking()
                                .AnyAsync(p => p.SYSTEM_ID == aggregate.OggettoTrasmesso.Id.AsLong() && (p.CHA_PRIVATO == "1" || p.CHA_PERSONALE == "1"));
                        }

                        if (!isPrivato)
                        {                    
                            var hierarcy = await _dbContext.GetHierarcy(personOrGroupDestinatario.ToString());

                            foreach (var h in hierarcy)
                            {
                                var dirittiHierarcy = securityEntities.Where(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS != 0 && s.ACCESSRIGHTS < accessRights).ToList();
                                if (dirittiHierarcy.Any())
                                {
                                    securityEntitiesToRemove.AddRange(dirittiHierarcy);
                                    if (idProjectC != null && idProjectC.Any())
                                    {
                                        securityEntitiesToRemove.AddRange(await _dbContext.SecurityEntities.AsNoTracking()
                                               .Where(s => s.PERSONORGROUP == h.ID_GRUPPO && idProjectC.Contains(s.THING.Value) && s.ACCESSRIGHTS != 0 && s.ACCESSRIGHTS < accessRights)
                                               .Select(s => s)
                                               .ToListAsync());
                                    }
                                }

                                if (!securityEntities.Any(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS >= accessRights))
                                {
                                    // Se il destinatario non ha già i diritti sull'oggetto
                                    newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                    {
                                        THING = aggregate.OggettoTrasmesso.Id.AsLong(),
                                        PERSONORGROUP = h.ID_GRUPPO,
                                        ACCESSRIGHTS = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45,
                                        ID_GRUPPO_TRASM = idGruppoTrasm,
                                        CHA_TIPO_DIRITTO = "A",
                                        HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                                    });

                                    if (idProjectC != null && idProjectC.Any())
                                    {
                                        // Se l'oggetto trasmesso è un fascicolo, è necessario aggiungere l'ACL anche per il record C
                                        idProjectC.ForEach(id =>
                                        {
                                            newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                            {
                                                THING = id,
                                                PERSONORGROUP = h.ID_GRUPPO,
                                                ACCESSRIGHTS = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45,
                                                ID_GRUPPO_TRASM = idGruppoTrasm,
                                                CHA_TIPO_DIRITTO = "F",
                                                HIDE_DOC_VERSIONS = trasmissioneSingola.NascondiVersioniPrecedenti.HasValue ? trasmissioneSingola.NascondiVersioniPrecedenti.Value ? "1" : "0" : null
                                            });
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
            {
                // Reperimento delle ACL di tutti i documenti contenuti nel fascicolo e sottofascicoli
                var documentsSecurityEntities =
                        await (from p in _dbContext.ProjectEntities.AsNoTracking()
                               join pc in _dbContext.ProjectComponentEntities.AsNoTracking() on p.SYSTEM_ID equals pc.PROJECT_ID
                               join s in _dbContext.SecurityEntities.AsNoTracking() on pc.LINK equals s.THING
                               where p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong()
                               select s)
                        .ToListAsync();

                foreach (var idDocument in documentsSecurityEntities.Select(s => s.THING).Distinct())
                {
                    // Analisi delle ACL di ogni documento contenuto nel fascicolo
                    var securityEntitiesPerIdDocument = documentsSecurityEntities.Where(s => s.THING == idDocument).ToList();

                    // Reperimento delle ACL da aggiungere per ogni documento
                    newSecurityEntities
                        .Except(securityEntitiesPerIdDocument, new SecurityEntityEqualitComparer())
                        .DistinctBy(s => s.PERSONORGROUP)
                        .ToList()
                        .ForEach(ase =>
                        {
                            if (!securityEntitiesPerIdDocument.Any(s => s.THING == idDocument && s.PERSONORGROUP == ase.PERSONORGROUP && s.ACCESSRIGHTS == ase.ACCESSRIGHTS))
                            {
                                // Aggiunta ACL mancante al documento
                                newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                {
                                    THING = idDocument,
                                    PERSONORGROUP = ase.PERSONORGROUP,
                                    ACCESSRIGHTS = ase.ACCESSRIGHTS,
                                    CHA_TIPO_DIRITTO = "F",
                                    CHA_COPIA_VISIBILITA = ase.CHA_COPIA_VISIBILITA,
                                    HIDE_DOC_VERSIONS = ase.HIDE_DOC_VERSIONS,
                                    ID_GRUPPO_TRASM = ase.ID_GRUPPO_TRASM,
                                    VAR_NOTE_SEC = ase.VAR_NOTE_SEC
                                });
                            }
                        });
                }
            }

            if (securityEntitiesToRemove.Any())
            {
                securityEntitiesToRemove.ForEach(e =>
                {
                    var securityEntry = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                            .Where(a => a.State != EntityState.Detached 
                                    && a.Entity.THING == e.THING
                                    && a.Entity.ACCESSRIGHTS == e.ACCESSRIGHTS
                                    && a.Entity.PERSONORGROUP == e.PERSONORGROUP)
                            .FirstOrDefault();
                    if (securityEntry == null)
                        securityEntry = ((DbContext)_dbContext).Entry<SecurityEntity>(e);

                    securityEntry.State = EntityState.Deleted;
                });
                //_dbContext.SecurityEntities.RemoveRange(securityEntitiesToRemove);
            }

            if (newSecurityEntities.Any())
            {
                await _dbContext.SecurityEntities.AddRangeAsync(newSecurityEntities);
            }
        }

        private class SecurityEntityEqualitComparer : IEqualityComparer<SecurityEntity>
        {
            public bool Equals(SecurityEntity? x, SecurityEntity? y)
            {
                return x.PERSONORGROUP == y.PERSONORGROUP;
            }

            public int GetHashCode([DisallowNull] SecurityEntity obj)
            {
                return obj.GetHashCode();
            }
        }

        protected virtual async Task Handle(TrasmissioneSingolaGruppoPreparedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var ragioneTrasmissioneEntity = await _dbContext.RagioneTrasmissioneEntities.FindAsync(@event.DatiTrasmissioneSingola.IdRagioneTrasmissione.AsLong());

            if (ragioneTrasmissioneEntity == null || ragioneTrasmissioneEntity != null && ragioneTrasmissioneEntity.ID_AMM != aggregate.IdTenant.AsLong())
                throw new TrasmissioneNotFoundPi3Exception(@event.DatiTrasmissioneSingola.IdRagioneTrasmissione);

            // Reperimento di tutti gli utenti attivi presenti nel gruppo
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(cg => cg.ID_GRUPPO == @event.DatiTrasmissioneSingola.IdGruppoDestinatario.AsLong()
                        && cg.ID_AMM == aggregate.IdTenant.AsLong())
                .Select(cg => new
                {
                    cg.SYSTEM_ID,
                    cg.ID_GRUPPO
                })
                .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new GruppoDestinatarioNotFoundPi3Exception(@event.DatiTrasmissioneSingola.IdGruppoDestinatario);

            var events = new List<IEvent>();

            var trasmissioneSingolaEntity = new TrasmSingolaEntity();

            await _dbContext.TrasmSingolaEntities.AddAsync(trasmissioneSingolaEntity);

            trasmissioneSingolaEntity.ID_TRASMISSIONE = trasmissioneEntity.SYSTEM_ID;
            trasmissioneSingolaEntity.ID_RAGIONE = @event.DatiTrasmissioneSingola.IdRagioneTrasmissione.AsLong();
            trasmissioneSingolaEntity.ID_CORR_GLOBALE = corrGlobaliEntity.SYSTEM_ID;
            trasmissioneSingolaEntity.VAR_NOTE_SING = @event.DatiTrasmissioneSingola.Note != null ? @event.DatiTrasmissioneSingola.Note.ToString() : null;
            trasmissioneSingolaEntity.HIDE_DOC_VERSIONS = @event.DatiTrasmissioneSingola.NascondiVersioniPrecedenti ? "1" : "0";
            trasmissioneSingolaEntity.CHA_TIPO_TRASM = @event.DatiTrasmissioneSingola.Tipo == TipiTrasmissioneSingolaEnum.Tutti ? "T" : "S";
            trasmissioneSingolaEntity.CHA_TIPO_DEST = "R";
            trasmissioneSingolaEntity.CHA_SET_EREDITA = ragioneTrasmissioneEntity.CHA_EREDITA;

            events.Add(new TrasmissioneSingolaGruppoAddedEvent()
            {
                Id = trasmissioneEntity.SYSTEM_ID.ToString(),
                IdTrasmissioneSingola = trasmissioneSingolaEntity.SYSTEM_ID.ToString(),
                IdRagioneTrasmissione = @event.DatiTrasmissioneSingola.IdRagioneTrasmissione,
                NomeRagioneTrasmissione = @event.DatiTrasmissioneSingola.NomeRagioneTrasmissione,
                RagioneConWorkflow = @event.DatiTrasmissioneSingola.RagioneConWorkflow,
                Tipo = @event.DatiTrasmissioneSingola.Tipo,
                IdGruppoDestinatario = @event.DatiTrasmissioneSingola.IdGruppoDestinatario,
                CodiceGruppoDestinatario = @event.DatiTrasmissioneSingola.CodiceGruppoDestinatario,
                DescrizioneGruppoDestinatario = @event.DatiTrasmissioneSingola.DescrizioneGruppoDestinatario,
                Note = @event.DatiTrasmissioneSingola.Note,
                DataScadenza = @event.DatiTrasmissioneSingola.DataScadenza,
                NascondiVersioniPrecedenti = @event.DatiTrasmissioneSingola.NascondiVersioniPrecedenti,
                CessioneDirittiRagione = @event.DatiTrasmissioneSingola.CessioneDirittiRagione
            });

            var utentiNotificati = @event.DatiTrasmissioneSingola.UtentiNotificati ?? new List<DatiUtenteNotificatoTrasmissioneSingolaGruppo>();

            if (utentiNotificati.Any())
            {
                // Verifica presenza utente
                foreach (var utenteNotificato in utentiNotificati)
                    if (!await _dbContext.PeopleGroupEntities.AsNoTracking().AnyAsync(pg => pg.PEOPLE_SYSTEM_ID == utenteNotificato.IdUtente.AsLong() && pg.GROUPS_SYSTEM_ID == corrGlobaliEntity.ID_GRUPPO && pg.DTA_FINE == null))
                        throw new UtenteDestinatarioNotFoundPi3Exception(utenteNotificato.IdUtente);
            }
            else
            {
                utentiNotificati = await _dbContext.PeopleGroupEntities.AsNoTracking()
                    .Where(pg => pg.GROUPS_SYSTEM_ID == corrGlobaliEntity.ID_GRUPPO && pg.DTA_FINE == null)
                    .Select(pg => new DatiUtenteNotificatoTrasmissioneSingolaGruppo()
                    {
                        IdUtente = pg.PEOPLE_SYSTEM_ID.ToString()
                    })
                    .ToListAsync();
            }

            foreach (var utenteNotificato in utentiNotificati)
            {
                var trasmissioneUtenteEntity = new TrasmUtenteEntity();

                await _dbContext.TrasmUtenteEntities.AddAsync(trasmissioneUtenteEntity);

                trasmissioneUtenteEntity.ID_TRASM_SINGOLA = trasmissioneSingolaEntity.SYSTEM_ID;
                trasmissioneUtenteEntity.ID_PEOPLE = utenteNotificato.IdUtente.AsLong();
                trasmissioneUtenteEntity.CHA_VALIDA = 1.ToString();
                trasmissioneUtenteEntity.CHA_IN_TODOLIST = 1.ToString();
                trasmissioneUtenteEntity.CHA_VISTA = 0.ToString();
                trasmissioneUtenteEntity.CHA_ACCETTATA = 0.ToString();
                trasmissioneUtenteEntity.CHA_RIFIUTATA = 0.ToString();
                trasmissioneUtenteEntity.CHA_VISTA_DELEGATO = 0.ToString();
                trasmissioneUtenteEntity.CHA_ACCETTATA_DELEGATO = 0.ToString();
                trasmissioneUtenteEntity.CHA_RIFIUTATA_DELEGATO = 0.ToString();

                events.Add(new TrasmissioneUtenteAddedEvent()
                {
                    Id = trasmissioneEntity.SYSTEM_ID.ToString(),
                    IdTrasmissioneSingola = trasmissioneSingolaEntity.SYSTEM_ID.ToString(),
                    IdTrasmissioneUtente = trasmissioneUtenteEntity.SYSTEM_ID.ToString(),
                    IdUtente = utenteNotificato.IdUtente,
                    UserId = utenteNotificato.UserId,
                    Cognome = utenteNotificato.Cognome,
                    Nome = utenteNotificato.Nome,
                    DataRimozioneCentroNotifiche = trasmissioneSingolaEntity.DTA_SCADENZA
                });
            }

            this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task Handle(TrasmissioneSingolaUtentePreparedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var ragioneTrasmissioneEntity = await _dbContext.RagioneTrasmissioneEntities.FindAsync(@event.DatiTrasmissioneSingola.IdRagioneTrasmissione.AsLong());

            if (ragioneTrasmissioneEntity == null || ragioneTrasmissioneEntity != null && ragioneTrasmissioneEntity.ID_AMM != aggregate.IdTenant.AsLong())
                throw new TrasmissioneNotFoundPi3Exception(@event.DatiTrasmissioneSingola.IdRagioneTrasmissione);

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(cg => cg.ID_PEOPLE == @event.DatiTrasmissioneSingola.IdUtente.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                .Select(cg => new
                {
                    cg.SYSTEM_ID,
                    cg.ID_PEOPLE
                })
                .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new UtenteDestinatarioNotFoundPi3Exception(@event.DatiTrasmissioneSingola.IdUtente);

            var events = new List<IEvent>();

            var trasmissioneSingolaEntity = new TrasmSingolaEntity();

            await _dbContext.TrasmSingolaEntities.AddAsync(trasmissioneSingolaEntity);

            trasmissioneSingolaEntity.ID_TRASMISSIONE = trasmissioneEntity.SYSTEM_ID;
            trasmissioneSingolaEntity.ID_RAGIONE = @event.DatiTrasmissioneSingola.IdRagioneTrasmissione.AsLong();
            trasmissioneSingolaEntity.ID_CORR_GLOBALE = corrGlobaliEntity.SYSTEM_ID;
            trasmissioneSingolaEntity.VAR_NOTE_SING = @event.DatiTrasmissioneSingola.Note != null ? @event.DatiTrasmissioneSingola.Note.ToString() : null;
            trasmissioneSingolaEntity.HIDE_DOC_VERSIONS = @event.DatiTrasmissioneSingola.NascondiVersioniPrecedenti ? "1" : "0";
            trasmissioneSingolaEntity.CHA_TIPO_TRASM = "S";
            trasmissioneSingolaEntity.CHA_TIPO_DEST = "U";
            trasmissioneSingolaEntity.CHA_SET_EREDITA = ragioneTrasmissioneEntity.CHA_EREDITA;

            events.Add(new TrasmissioneSingolaUtenteAddedEvent()
            {
                Id = trasmissioneEntity.SYSTEM_ID.ToString(),
                IdTrasmissioneSingola = trasmissioneSingolaEntity.SYSTEM_ID.ToString(),
                IdRagioneTrasmissione = @event.DatiTrasmissioneSingola.IdRagioneTrasmissione,
                NomeRagioneTrasmissione = @event.DatiTrasmissioneSingola.NomeRagioneTrasmissione,
                RagioneConWorkflow = @event.DatiTrasmissioneSingola.RagioneConWorkflow,
                IdUtenteDestinatario = @event.DatiTrasmissioneSingola.IdUtente,
                UserIdDestinatario = @event.DatiTrasmissioneSingola.UserId,
                CognomeDestinatario = @event.DatiTrasmissioneSingola.Cognome,
                NomeDestinatario = @event.DatiTrasmissioneSingola.Nome,
                Note = @event.DatiTrasmissioneSingola.Note,
                DataScadenza = @event.DatiTrasmissioneSingola.DataScadenza,
                NascondiVersioniPrecedenti = @event.DatiTrasmissioneSingola.NascondiVersioniPrecedenti,
                CessioneDirittiRagione = @event.DatiTrasmissioneSingola.CessioneDirittiRagione
            });

            var trasmissioneUtenteEntity = new TrasmUtenteEntity();

            await _dbContext.TrasmUtenteEntities.AddAsync(trasmissioneUtenteEntity);

            trasmissioneUtenteEntity.ID_TRASM_SINGOLA = trasmissioneSingolaEntity.SYSTEM_ID;
            trasmissioneUtenteEntity.ID_PEOPLE = corrGlobaliEntity.ID_PEOPLE;
            trasmissioneUtenteEntity.CHA_VALIDA = 1.ToString();
            trasmissioneUtenteEntity.CHA_IN_TODOLIST = 1.ToString();
            trasmissioneUtenteEntity.CHA_VISTA = 0.ToString();
            trasmissioneUtenteEntity.CHA_ACCETTATA = 0.ToString();
            trasmissioneUtenteEntity.CHA_RIFIUTATA = 0.ToString();
            trasmissioneUtenteEntity.CHA_VISTA_DELEGATO = 0.ToString();
            trasmissioneUtenteEntity.CHA_ACCETTATA_DELEGATO = 0.ToString();
            trasmissioneUtenteEntity.CHA_RIFIUTATA_DELEGATO = 0.ToString();

            events.Add(new TrasmissioneUtenteAddedEvent()
            {
                Id = trasmissioneEntity.SYSTEM_ID.ToString(),
                IdTrasmissioneSingola = trasmissioneSingolaEntity.SYSTEM_ID.ToString(),
                IdTrasmissioneUtente = trasmissioneUtenteEntity.SYSTEM_ID.ToString(),
                IdUtente = @event.DatiTrasmissioneSingola.IdUtente,
                UserId = @event.DatiTrasmissioneSingola.UserId,
                Cognome = @event.DatiTrasmissioneSingola.Cognome,
                Nome = @event.DatiTrasmissioneSingola.Nome,
                DataRimozioneCentroNotifiche = trasmissioneSingolaEntity.DTA_SCADENZA
            });

            this.LoadAggregateFromHistory(aggregate, events.ToArray());
        }

        protected virtual async Task Handle(TrasmissioneUtenteAccettataEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneNonInviata(trasmissioneEntity);

            var trasmissioneUtenteEntity = await _dbContext.TrasmUtenteEntities.FindAsync(@event.IdTrasmissioneUtente.AsLong());

            if (trasmissioneUtenteEntity == null)
                throw new TrasmissioneUtenteNotFoundPi3Exception(@event.IdTrasmissioneUtente);

            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
            {
                var idProfileAsLong = aggregate.OggettoTrasmesso.Id.AsLong();
                if (await _dbContext.ProfileEntities.AsNoTracking().AnyAsync(p => p.DOCNUMBER == idProfileAsLong && (p.CHA_IN_CESTINO ?? "0") == "1"))
                    throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.OggettoTrasmessoInCestino, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);
            }

            if (trasmissioneUtenteEntity.CHA_ACCETTATA == "1" || trasmissioneUtenteEntity.CHA_RIFIUTATA == "1")
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettataORifiutata, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);

            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser);
            if (trasmissioneUtenteEntity.ID_PEOPLE != idUser)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.UtenteNonDestinatarioDirettoTrasmissione, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);

            bool applySecurity = true;

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(trasmissioneUtenteEntity.ID_TRASM_SINGOLA);

            if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "S" && trasmissioneSingolaEntity.CHA_TIPO_DEST == "R")
            {
                // Se la trasmissione è UNO ed a ruolo, verifica se la trasmissione non sia già stata accettata / rifiutata da altri destinatari
                if (await _dbContext.TrasmUtenteEntities.AsNoTracking()
                    .AnyAsync(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID
                            && (tu.CHA_ACCETTATA == "1" || tu.CHA_RIFIUTATA == "1")))
                {
                    throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteNonAccettabileORifiutabile, ErrorDescriptions.ResourceManager, trasmissioneUtenteEntity.SYSTEM_ID.ToString());
                }
            }
            else if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "T" && trasmissioneSingolaEntity.CHA_TIPO_DEST == "R")
            {
                // Verifica se è il primo utente che accetta la trasmissione:
                // in questo caso il metodo restituisce un valore True e si procederà ad impostare la visibilità gerarchica sull'oggetto,
                // altrimenti non succedera nulla.
                var allTs = await _dbContext.TrasmUtenteEntities.AsNoTracking()
                                .Where(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID)
                                .Select(tu => new
                                {
                                    tu.CHA_ACCETTATA,
                                    tu.CHA_RIFIUTATA
                                })
                                .ToListAsync();

                var remaining = allTs.Count(ts => ts.CHA_ACCETTATA == "1");

                if (remaining > 0)
                {
                    // Se il numero di accettazioni è superiore ad 1 allora l'acl è stata già applicata
                    applySecurity = false;
                }
            }

            var ragioneTrasmissioneEntity = await _dbContext.RagioneTrasmissioneEntities.FindAsync(trasmissioneSingolaEntity.ID_RAGIONE);

            if (ragioneTrasmissioneEntity == null || ragioneTrasmissioneEntity != null && ragioneTrasmissioneEntity.ID_AMM != aggregate.IdTenant.AsLong())
                throw new TrasmissioneNotFoundPi3Exception(trasmissioneSingolaEntity.ID_RAGIONE.ToString());

            if (ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE != "W")
                throw new RagioneTrasmissioneNoWorkflowPi3Exception(ragioneTrasmissioneEntity.SYSTEM_ID.ToString());

            var currentDateTime = await _dbContext.GetSystemDateTime();

            trasmissioneUtenteEntity.CHA_ACCETTATA = "1";
            trasmissioneUtenteEntity.DTA_ACCETTATA = currentDateTime;
            trasmissioneUtenteEntity.VAR_NOTE_ACC = @event.Accetta.Note != null ? @event.Accetta.Note.ToString() : null;
            trasmissioneUtenteEntity.CHA_VALIDA = "0";
            trasmissioneUtenteEntity.ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(@event.Accetta.IdDelegato) ? @event.Accetta.IdDelegato.AsLong() : null;
            trasmissioneUtenteEntity.CHA_ACCETTATA_DELEGATO = !string.IsNullOrWhiteSpace(@event.Accetta.IdDelegato) ? "1" : "0";
            trasmissioneUtenteEntity.CHA_IN_TODOLIST = trasmissioneUtenteEntity.DTA_VISTA.HasValue ? "0" : "1";

            //Rimozione ToDoList di altri utenti e centro notifiche
            List<NotifyEntity> notifyEntity = null;
            if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "S")
            {
                var trasmissioneUtenteAtriUtentiEntity = await _dbContext.TrasmUtenteEntities
                        .Where(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID && tu.SYSTEM_ID != trasmissioneUtenteEntity.SYSTEM_ID)
                        .ToListAsync();

                trasmissioneUtenteAtriUtentiEntity.ForEach(tu =>
                {
                    tu.CHA_VALIDA = "0";
                    tu.CHA_IN_TODOLIST = "0";
                });

                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID && n.ID_PEOPLE_RECEIVER != (trasmissioneUtenteEntity.DTA_VISTA.HasValue ? null : trasmissioneUtenteEntity.ID_PEOPLE))
                .ToListAsync();
            }
            else if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "T" && trasmissioneUtenteEntity.DTA_VISTA.HasValue)
            {
                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                    .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID && n.ID_PEOPLE_RECEIVER == trasmissioneUtenteEntity.ID_PEOPLE)
                    .ToListAsync();
            }
            if (notifyEntity != null && notifyEntity.Any())
            {
                List<NotifyHistoryEntity> notifyHistoryEntity = new List<NotifyHistoryEntity>();
                foreach (NotifyEntity n in notifyEntity)
                {
                    var notiyHistoryEntity = new NotifyHistoryEntity()
                    {
                        ID_NOTIFY = n.SYSTEM_ID,
                        ID_EVENT = n.ID_EVENT,
                        DESC_PRODUCER = n.DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER = n.ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER = n.ID_GROUP_RECEIVER,
                        TYPE_NOTIFY = n.TYPE_NOTIFY,
                        DTA_NOTIFY = n.DTA_NOTIFY,
                        FIELD_1 = n.FIELD_1,
                        FIELD_2 = n.FIELD_2,
                        FIELD_3 = n.FIELD_3,
                        FIELD_4 = n.FIELD_4,
                        MULTIPLICITY = n.MULTIPLICITY,
                        SPECIALIZED_FIELD = n.SPECIALIZED_FIELD,
                        TYPE_EVENT = n.TYPE_EVENT,
                        DOMAINOBJECT = n.DOMAINOBJECT,
                        ID_OBJECT = n.ID_OBJECT,
                        ID_SPECIALIZED_OBJECT = n.ID_SPECIALIZED_OBJECT,
                        DTA_EVENT = n.DTA_EVENT,
                        READ_NOTIFICATION = n.READ_NOTIFICATION,
                        NOTES = n.NOTES
                    };
                    notifyHistoryEntity.Add(notiyHistoryEntity);
                }

                await _dbContext.NotifyHistoryEntities.AddRangeAsync(notifyHistoryEntity);

                _dbContext.NotifyEntities.RemoveRange(notifyEntity);
            }

            if (applySecurity)
            {
                var securityEntities = await _dbContext.SecurityEntities
                                   .Where(s => s.THING == (trasmissioneEntity.ID_PROFILE.HasValue ? trasmissioneEntity.ID_PROFILE : trasmissioneEntity.ID_PROJECT))
                                   .Select(s => s)
                                   .ToListAsync();

                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                {
                    // Per il fascicolo, le stesse ACL devo essere inserite anche per il record C
                    //var idProjectC = await _dbContext.ProjectEntities.AsNoTracking()
                    //    .Where(p => p.ID_PARENT == aggregate.OggettoTrasmesso.Id.AsLong())
                    //    .Select(p => p.SYSTEM_ID)
                    //    .FirstAsync();

                    securityEntities.AddRange(await _dbContext.SecurityEntities.AsNoTracking()
                                    .Join(_dbContext.ProjectEntities.AsNoTracking(),
                                        s => s.THING,
                                        p => p.SYSTEM_ID,
                                        (s, p) => new { s, p})
                                   .Where(j => j.p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong())
                                   .Select(j => j.s)
                                   .ToListAsync());
                }

                var corrGlobaleEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(cg => cg.SYSTEM_ID == trasmissioneSingolaEntity.ID_CORR_GLOBALE)
                        .Select(cg => new
                        {
                            cg.ID_GRUPPO,
                            cg.ID_PEOPLE
                        })
                        .FirstAsync();

                var personOrGroupDestinatario = corrGlobaleEntity.ID_GRUPPO.HasValue ? corrGlobaleEntity.ID_GRUPPO : corrGlobaleEntity.ID_PEOPLE;
                var accessRight = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45;

                // Reperimento del diritto al destinatario diretto, con accessright = 20 (in attesa di accettazione / rifiuto)
                //var dirittiDestinatario = securityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && (s.ACCESSRIGHTS == 20 || s.ACCESSRIGHTS == 45)).ToList();
                var dirittiDestinatario = securityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario).ToList();
                var dirittiDestinatarioToReplace = securityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS != 0 && s.ACCESSRIGHTS < accessRight).ToList();

                var securityEntitiesToRemove = new List<SecurityEntity>();
                var newSecurityEntities = new List<SecurityEntity>();

                if (dirittiDestinatarioToReplace.Any())
                {
                    // Se il diritto è esistente, deve essere rimosso (perché chiave) e poi aggiunto nuovamente ma modificato negli attributi
                    securityEntitiesToRemove.AddRange(dirittiDestinatarioToReplace);

                    dirittiDestinatarioToReplace
                        .ForEach(d =>
                        {
                            if (!securityEntities.Any(s => s.THING == d.THING && s.PERSONORGROUP == d.PERSONORGROUP && s.ACCESSRIGHTS == accessRight))
                            {
                                newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                {
                                    PERSONORGROUP = d.PERSONORGROUP,
                                    ACCESSRIGHTS = accessRight,
                                    THING = d.THING,
                                    CHA_TIPO_DIRITTO = "T",
                                    HIDE_DOC_VERSIONS = d.HIDE_DOC_VERSIONS,
                                    ID_GRUPPO_TRASM = d.ID_GRUPPO_TRASM
                                }); 
                            }
                        });
                }

                // Se la ragione trasmissione prevede workflow, inserisce il diritto con risalita gerarchica
                if (ragioneTrasmissioneEntity.CHA_EREDITA == "1" && trasmissioneSingolaEntity.CHA_TIPO_DEST == "R")
                {
                    var hierarcy = await _dbContext.GetHierarcy(personOrGroupDestinatario.ToString());

                    foreach (var h in hierarcy)
                    {
                        var dirittiRuoloHierarcy = securityEntities.Where(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS < accessRight).ToList();
                        if(dirittiRuoloHierarcy.Any())
                            securityEntitiesToRemove.AddRange(dirittiRuoloHierarcy);

                        if (!securityEntities.Any(s => s.PERSONORGROUP == h.ID_GRUPPO && s.ACCESSRIGHTS == accessRight))
                        {
                            dirittiDestinatario
                                .ForEach(d =>
                                {
                                    // Se il destinatario non ha già i diritti sull'oggetto
                                    newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                    {
                                        THING = d.THING,
                                        PERSONORGROUP = h.ID_GRUPPO,
                                        ACCESSRIGHTS = ragioneTrasmissioneEntity.CHA_TIPO_DIRITTI == "W" ? 63 : 45,
                                        ID_GRUPPO_TRASM = d.ID_GRUPPO_TRASM,
                                        CHA_TIPO_DIRITTO = "A",
                                        HIDE_DOC_VERSIONS = trasmissioneSingolaEntity.HIDE_DOC_VERSIONS
                                    });
                                });
                        }
                    }
                }

                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                {
                    // Reperimento delle ACL di tutti i documenti contenuti nel fascicolo e sottofascicoli
                    var documentsSecurityEntities =
                            await (from p in _dbContext.ProjectEntities.AsNoTracking()
                                   join pc in _dbContext.ProjectComponentEntities.AsNoTracking() on p.SYSTEM_ID equals pc.PROJECT_ID
                                   join s in _dbContext.SecurityEntities.AsNoTracking() on pc.LINK equals s.THING
                                   where p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong()
                                   select s)
                            .ToListAsync();

                    foreach (var idDocument in documentsSecurityEntities.Select(s => s.THING).Distinct())
                    {
                        // Analisi delle ACL di ogni documento contenuto nel fascicolo
                        var securityEntitiesPerIdDocument = documentsSecurityEntities.Where(s => s.THING == idDocument).ToList();

                        // Se il diritto è esistente, deve essere rimosso (perché chiave) e poi aggiunto nuovamente ma modificato negli attributi
                        var toRemove = securityEntitiesPerIdDocument.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS == 20);

                        securityEntitiesToRemove.AddRange(toRemove);
                        securityEntitiesPerIdDocument.RemoveAll(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS == 20);

                        // Reperimento delle ACL da aggiungere per ogni documento
                        newSecurityEntities
                            .Except(securityEntitiesPerIdDocument, new SecurityEntityEqualitComparer())
                            .DistinctBy(s => s.PERSONORGROUP)
                            .ToList()
                            .ForEach(ase =>
                            {
                                if (!securityEntitiesPerIdDocument.Any(s => s.THING == idDocument && s.PERSONORGROUP == ase.PERSONORGROUP && s.ACCESSRIGHTS == ase.ACCESSRIGHTS))
                                {
                                    // Aggiunta ACL mancante al documento
                                    newSecurityEntities.AddIfNotExists(new SecurityEntity()
                                    {
                                        THING = idDocument,
                                        PERSONORGROUP = ase.PERSONORGROUP,
                                        ACCESSRIGHTS = ase.ACCESSRIGHTS,
                                        CHA_TIPO_DIRITTO = "F",
                                        CHA_COPIA_VISIBILITA = ase.CHA_COPIA_VISIBILITA,
                                        HIDE_DOC_VERSIONS = ase.HIDE_DOC_VERSIONS,
                                        ID_GRUPPO_TRASM = ase.ID_GRUPPO_TRASM,
                                        VAR_NOTE_SEC = ase.VAR_NOTE_SEC
                                    });
                                }
                            });
                    }
                }

                if (securityEntitiesToRemove.Any())
                {
                    securityEntitiesToRemove.ForEach(e =>
                    {
                        var securityEntry = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                                .Where(a => a.State != EntityState.Detached
                                        && a.Entity.THING == e.THING
                                        && a.Entity.ACCESSRIGHTS == e.ACCESSRIGHTS
                                        && a.Entity.PERSONORGROUP == e.PERSONORGROUP)
                                .FirstOrDefault();
                        if (securityEntry == null)
                            securityEntry = ((DbContext)_dbContext).Entry<SecurityEntity>(e);

                        securityEntry.State = EntityState.Deleted;
                    });
                    //_dbContext.SecurityEntities.RemoveRange(securityEntitiesToRemove);
                }

                if (newSecurityEntities.Any())
                {
                    await _dbContext.SecurityEntities.AddRangeAsync(newSecurityEntities);
                }
            }
        }

        protected virtual async Task Handle(TrasmissioneUtenteRifiutataEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneNonInviata(trasmissioneEntity);

            var trasmissioneUtenteEntity = await _dbContext.TrasmUtenteEntities.FindAsync(@event.IdTrasmissioneUtente.AsLong());

            if (trasmissioneUtenteEntity == null)
                throw new TrasmissioneUtenteNotFoundPi3Exception(@event.IdTrasmissioneUtente);

            if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.DocumentoAmministrativo)
            {
                var idProfileAsLong = aggregate.OggettoTrasmesso.Id.AsLong();
                if (await _dbContext.ProfileEntities.AsNoTracking().AnyAsync(p => p.DOCNUMBER == idProfileAsLong && (p.CHA_IN_CESTINO ?? "0") == "1"))
                    throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.OggettoTrasmessoInCestino, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);
            }

            if (trasmissioneUtenteEntity.CHA_ACCETTATA == "1" || trasmissioneUtenteEntity.CHA_RIFIUTATA == "1")
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettataORifiutata, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);

            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            if (trasmissioneUtenteEntity.ID_PEOPLE != idUser)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.UtenteNonDestinatarioDirettoTrasmissione, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(trasmissioneUtenteEntity.ID_TRASM_SINGOLA);

            bool applySecurity = true;

            if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "S" && trasmissioneSingolaEntity.CHA_TIPO_DEST == "R")
            {
                // Se la trasmissione è UNO ed a ruolo, verifica se la trasmissione non sia già stata accettata / rifiutata da altri destinatari
                if (await _dbContext.TrasmUtenteEntities.AsNoTracking()
                    .AnyAsync(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID
                            && (tu.CHA_ACCETTATA == "1" || tu.CHA_RIFIUTATA == "1")))
                {
                    throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteNonAccettabileORifiutabile, ErrorDescriptions.ResourceManager, trasmissioneUtenteEntity.SYSTEM_ID.ToString());
                }
            }
            else if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "T" && trasmissioneSingolaEntity.CHA_TIPO_DEST == "R")
            {
                // Verifica se le trasmissioni utenti sono state tutte rifiutate:
                // in questo caso il metodo restituisce un valore True e si procederà all'eliminazione della visibilità sull'oggetto,
                // altrimenti non succedera nulla.
                var allTs = await _dbContext.TrasmUtenteEntities.AsNoTracking()
                                .Where(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID)
                                .Select(tu => new
                                {
                                    tu.CHA_ACCETTATA,
                                    tu.CHA_RIFIUTATA
                                })
                                .ToListAsync();

                var remaining = allTs.Count - allTs.Count(ts => ts.CHA_RIFIUTATA == "1");

                if (remaining > 1)
                {
                    // se esiste anche solo una trasmissione utente non ancora rifiutata, non deve succedere nulla
                    applySecurity = false;
                }
            }

            var ragioneTrasmissioneEntity = await _dbContext.RagioneTrasmissioneEntities.FindAsync(trasmissioneSingolaEntity.ID_RAGIONE);

            if (ragioneTrasmissioneEntity == null || ragioneTrasmissioneEntity != null && ragioneTrasmissioneEntity.ID_AMM != aggregate.IdTenant.AsLong())
                throw new TrasmissioneNotFoundPi3Exception(trasmissioneSingolaEntity.ID_RAGIONE.ToString());

            if (ragioneTrasmissioneEntity.CHA_TIPO_RAGIONE != "W")
                throw new RagioneTrasmissioneNoWorkflowPi3Exception(ragioneTrasmissioneEntity.SYSTEM_ID.ToString());

            var currentDateTime = await _dbContext.GetSystemDateTime();

            if (trasmissioneUtenteEntity.CHA_VISTA == "0")
            {
                trasmissioneUtenteEntity.CHA_VISTA = "1";
                trasmissioneUtenteEntity.DTA_VISTA = currentDateTime;
            }

            trasmissioneUtenteEntity.CHA_RIFIUTATA = "1";
            trasmissioneUtenteEntity.DTA_RIFIUTATA = currentDateTime;
            trasmissioneUtenteEntity.VAR_NOTE_RIF = @event.Rifiuta.Note.ToString();
            trasmissioneUtenteEntity.ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(@event.Rifiuta.IdDelegato) ? @event.Rifiuta.IdDelegato.AsLong() : null;
            trasmissioneUtenteEntity.CHA_RIFIUTATA_DELEGATO = !string.IsNullOrWhiteSpace(@event.Rifiuta.IdDelegato) ? "1" : "0";
            trasmissioneUtenteEntity.CHA_VALIDA = "0";
            trasmissioneUtenteEntity.CHA_IN_TODOLIST = "0";

            //Rimozione ToDoList e Centro notifiche
            List<NotifyEntity> notifyEntity = null;
            if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "S")
            {
                var trasmissioneUtenteAtriUtentiEntity = await _dbContext.TrasmUtenteEntities
                        .Where(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID && tu.SYSTEM_ID != trasmissioneUtenteEntity.SYSTEM_ID)
                        .ToListAsync();

                trasmissioneUtenteAtriUtentiEntity.ForEach(tu =>
                {
                    tu.CHA_VALIDA = "0";
                    tu.CHA_IN_TODOLIST = "0";
                });

                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                    .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID)
                    .ToListAsync();
            }
            else if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "T")
            {
                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                    .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID && n.ID_PEOPLE_RECEIVER == trasmissioneUtenteEntity.ID_PEOPLE)
                    .ToListAsync();
            }
            if (notifyEntity != null && notifyEntity.Any())
            {
                List<NotifyHistoryEntity> notifyHistoryEntity = new List<NotifyHistoryEntity>();
                foreach (NotifyEntity n in notifyEntity)
                {
                    var notiyHistoryEntity = new NotifyHistoryEntity
                    {
                        ID_NOTIFY = n.SYSTEM_ID,
                        ID_EVENT = n.ID_EVENT,
                        DESC_PRODUCER = n.DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER = n.ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER = n.ID_GROUP_RECEIVER,
                        TYPE_NOTIFY = n.TYPE_NOTIFY,
                        DTA_NOTIFY = n.DTA_NOTIFY,
                        FIELD_1 = n.FIELD_1,
                        FIELD_2 = n.FIELD_2,
                        FIELD_3 = n.FIELD_3,
                        FIELD_4 = n.FIELD_4,
                        MULTIPLICITY = n.MULTIPLICITY,
                        SPECIALIZED_FIELD = n.SPECIALIZED_FIELD,
                        TYPE_EVENT = n.TYPE_EVENT,
                        DOMAINOBJECT = n.DOMAINOBJECT,
                        ID_OBJECT = n.ID_OBJECT,
                        ID_SPECIALIZED_OBJECT = n.ID_SPECIALIZED_OBJECT,
                        DTA_EVENT = n.DTA_EVENT,
                        READ_NOTIFICATION = n.READ_NOTIFICATION,
                        NOTES = n.NOTES
                    };
                    notifyHistoryEntity.Add(notiyHistoryEntity);
                }

                await _dbContext.NotifyHistoryEntities.AddRangeAsync(notifyHistoryEntity);

                _dbContext.NotifyEntities.RemoveRange(notifyEntity);
            }

            if (applySecurity)
            {
                var securityEntities = await _dbContext.SecurityEntities
                                   .Where(s => s.THING == (trasmissioneEntity.ID_PROFILE.HasValue ? trasmissioneEntity.ID_PROFILE : trasmissioneEntity.ID_PROJECT))
                                   .Select(s => s)
                                   .ToListAsync();

                if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                {
                    // Per il fascicolo, le stesse ACL devo essere inserite anche per il record C
                    securityEntities.AddRange(await _dbContext.SecurityEntities
                                .Join( _dbContext.ProjectEntities.AsNoTracking(),
                                      s => s.THING,
                                      p => p.SYSTEM_ID,
                                      (s, p) => new {s, p})
                                .Where(j => j.s.THING == j.p.SYSTEM_ID && j.p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong())
                                .Select(j => j.s)
                                .ToListAsync());
                }

                var corrGlobaleEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .Where(cg => cg.SYSTEM_ID == trasmissioneSingolaEntity.ID_CORR_GLOBALE)
                        .Select(cg => new
                        {
                            cg.ID_GRUPPO,
                            cg.ID_PEOPLE
                        })
                        .FirstAsync();

                var personOrGroupDestinatario = corrGlobaleEntity.ID_GRUPPO.HasValue ? corrGlobaleEntity.ID_GRUPPO : corrGlobaleEntity.ID_PEOPLE;

                // Verifica se esiste più di una trasmissione in Centro Notifiche che devono essere accettate dal ruolo, in caso non rimuovo la viibilità
                var existsTrasmDaAccettareRifiutareRuolo = corrGlobaleEntity.ID_GRUPPO.HasValue && await _dbContext.NotifyEntities
                    .Join(_dbContext.TrasmSingolaEntities,
                            n => n.ID_SPECIALIZED_OBJECT,
                            s => s.SYSTEM_ID,
                            (n, s) => new { n, s })
                    .Join(_dbContext.RagioneTrasmissioneEntities,
                            j => j.s.ID_RAGIONE,
                            r => r.SYSTEM_ID,
                            (j, r) => new { j.n, j.s, r })
                    .AsNoTracking()
                    .AnyAsync(j => j.n.ID_OBJECT == (trasmissioneEntity.ID_PROFILE.HasValue ? trasmissioneEntity.ID_PROFILE : trasmissioneEntity.ID_PROJECT)
                                   && j.n.ID_GROUP_RECEIVER == personOrGroupDestinatario
                                   && j.r.CHA_TIPO_RAGIONE == "W" 
                                   && j.s.SYSTEM_ID != trasmissioneSingolaEntity.SYSTEM_ID);

                if (!existsTrasmDaAccettareRifiutareRuolo)
                {
                    // Reperimento del diritto al destinatario diretto, con accessright = 20 (in attesa di accettazione / rifiuto)
                    var dirittiDestinatario = securityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS == 20).ToList();

                    var securityEntitiesToRemove = new List<SecurityEntity>();

                    if (dirittiDestinatario.Any())
                    {
                        // Se il diritto è esistente, deve essere rimosso (perché chiave) e poi aggiunto nuovamente ma modificato negli attributi
                        securityEntitiesToRemove.AddRange(dirittiDestinatario);
                    }

                    if (aggregate.OggettoTrasmesso.TipoOggetto == TipiOggettiTrasmessiEnum.Fascicolo)
                    {
                        // Reperimento delle ACL di tutti i documenti contenuti nel fascicolo e sottofascicoli
                        var documentsSecurityEntities =
                                await (from p in _dbContext.ProjectEntities.AsNoTracking()
                                       join pc in _dbContext.ProjectComponentEntities.AsNoTracking() on p.SYSTEM_ID equals pc.PROJECT_ID
                                       join s in _dbContext.SecurityEntities.AsNoTracking() on pc.LINK equals s.THING
                                       where p.ID_FASCICOLO == aggregate.OggettoTrasmesso.Id.AsLong()
                                       select s)
                                .ToListAsync();

                        var toRemove = documentsSecurityEntities.Where(s => s.PERSONORGROUP == personOrGroupDestinatario && s.ACCESSRIGHTS == 20);

                        securityEntitiesToRemove.AddRange(toRemove);
                    }

                    if (securityEntitiesToRemove.Any())
                    {
                        securityEntitiesToRemove.ForEach(e =>
                        {
                            var securityEntry = ((DbContext)_dbContext).ChangeTracker.Entries<SecurityEntity>()
                                    .Where(a => a.State != EntityState.Detached
                                            && a.Entity.THING == e.THING
                                            && a.Entity.ACCESSRIGHTS == e.ACCESSRIGHTS
                                            && a.Entity.PERSONORGROUP == e.PERSONORGROUP)
                                    .FirstOrDefault();
                            if (securityEntry == null)
                                securityEntry = ((DbContext)_dbContext).Entry<SecurityEntity>(e);

                            securityEntry.State = EntityState.Deleted;
                        });
                        //_dbContext.SecurityEntities.RemoveRange(securityEntitiesToRemove);
                    }
                }
            }
        }

        protected virtual async Task Handle(TrasmissioneUtenteVistaEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(@event.Id);

            AssertTrasmissioneNonInviata(trasmissioneEntity);

            var trasmissioneUtenteEntity = await _dbContext.TrasmUtenteEntities.FindAsync(@event.IdTrasmissioneUtente.AsLong());

            if (trasmissioneUtenteEntity == null)
                throw new TrasmissioneUtenteNotFoundPi3Exception(@event.IdTrasmissioneUtente);

            var idUser = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            if (trasmissioneUtenteEntity.ID_PEOPLE != idUser)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.UtenteNonDestinatarioDirettoTrasmissione, ErrorDescriptions.ResourceManager, @event.IdTrasmissioneUtente);

            trasmissioneUtenteEntity.CHA_IN_TODOLIST = "0";
            trasmissioneUtenteEntity.ID_PEOPLE_DELEGATO = !string.IsNullOrWhiteSpace(@event.Visto.IdDelegato) ? @event.Visto.IdDelegato.AsLong() : null;
            trasmissioneUtenteEntity.CHA_VISTA_DELEGATO = !string.IsNullOrWhiteSpace(@event.Visto.IdDelegato) ? "1" : "0";

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(trasmissioneUtenteEntity.ID_TRASM_SINGOLA);

            //Rimozione dalla ToDoList e Centro Notifiche
            List<NotifyEntity> notifyEntity = null;
            if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "S")
            {
                var trasmissioneUtenteAtriUtentiEntity = await _dbContext.TrasmUtenteEntities
                        .Where(tu => tu.ID_TRASM_SINGOLA == trasmissioneSingolaEntity.SYSTEM_ID && tu.SYSTEM_ID != trasmissioneUtenteEntity.SYSTEM_ID)
                        .ToListAsync();

                trasmissioneUtenteAtriUtentiEntity.ForEach(tu =>
                {
                    tu.CHA_IN_TODOLIST = "0";
                });

                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID)
                .ToListAsync();
            }
            else if (trasmissioneSingolaEntity.CHA_TIPO_TRASM == "T")
            {
                notifyEntity = await _dbContext.NotifyEntities.AsNoTracking()
                    .Where(n => n.ID_SPECIALIZED_OBJECT == trasmissioneSingolaEntity.SYSTEM_ID && n.ID_PEOPLE_RECEIVER == trasmissioneUtenteEntity.ID_PEOPLE)
                    .ToListAsync();
            }
            if (notifyEntity != null && notifyEntity.Any())
            {
                List<NotifyHistoryEntity> notifyHistoryEntity = new List<NotifyHistoryEntity>();
                foreach (NotifyEntity n in notifyEntity)
                {
                    var notiyHistoryEntity = new NotifyHistoryEntity()
                    {
                        ID_NOTIFY = n.SYSTEM_ID,
                        ID_EVENT = n.ID_EVENT,
                        DESC_PRODUCER = n.DESC_PRODUCER,
                        ID_PEOPLE_RECEIVER = n.ID_PEOPLE_RECEIVER,
                        ID_GROUP_RECEIVER = n.ID_GROUP_RECEIVER,
                        TYPE_NOTIFY = n.TYPE_NOTIFY,
                        DTA_NOTIFY = n.DTA_NOTIFY,
                        FIELD_1 = n.FIELD_1,
                        FIELD_2 = n.FIELD_2,
                        FIELD_3 = n.FIELD_3,
                        FIELD_4 = n.FIELD_4,
                        MULTIPLICITY = n.MULTIPLICITY,
                        SPECIALIZED_FIELD = n.SPECIALIZED_FIELD,
                        TYPE_EVENT = n.TYPE_EVENT,
                        DOMAINOBJECT = n.DOMAINOBJECT,
                        ID_OBJECT = n.ID_OBJECT,
                        ID_SPECIALIZED_OBJECT = n.ID_SPECIALIZED_OBJECT,
                        DTA_EVENT = n.DTA_EVENT,
                        READ_NOTIFICATION = n.READ_NOTIFICATION,
                        NOTES = n.NOTES
                    };
                    notifyHistoryEntity.Add(notiyHistoryEntity);
                }

                await _dbContext.NotifyHistoryEntities.AddRangeAsync(notifyHistoryEntity);

                _dbContext.NotifyEntities.RemoveRange(notifyEntity);
            }
        }

        protected virtual async Task Handle(TrasmissioneSingolaRemovedEvent @event, Trasmissione aggregate)
        {

        }

        protected virtual async Task Handle(TipoTrasmissioneSingolaChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(@event.IdTrasmissioneSingola.AsLong());

            if (trasmissioneSingolaEntity == null)
                throw new TrasmissioneSingolaNotFoundPi3Exception(@event.IdTrasmissioneSingola);

            trasmissioneSingolaEntity.CHA_TIPO_TRASM = @event.NewTipo == TipiTrasmissioneSingolaEnum.Tutti ? "T" : "S";
        }

        protected virtual async Task Handle(NoteTrasmissioneSingolaChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(@event.IdTrasmissioneSingola.AsLong());

            if (trasmissioneSingolaEntity == null)
                throw new TrasmissioneSingolaNotFoundPi3Exception(@event.IdTrasmissioneSingola);

            trasmissioneSingolaEntity.VAR_NOTE_SING = @event.NewNote != null ? @event.NewNote.ToString() : null;
        }

        protected virtual async Task Handle(DataScadenzaTrasmissioneSingolaChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(@event.IdTrasmissioneSingola.AsLong());

            if (trasmissioneSingolaEntity == null)
                throw new TrasmissioneSingolaNotFoundPi3Exception(@event.IdTrasmissioneSingola);

            trasmissioneSingolaEntity.DTA_SCADENZA = @event.NewDataScadenza.HasValue ? @event.NewDataScadenza : null;
        }

        protected virtual async Task Handle(TrasmissioneSingolaNascondiVersioniPrecedentiChangedEvent @event, Trasmissione aggregate)
        {
            var trasmissioneEntity = await _dbContext.TrasmissioneEntities.FindAsync(aggregate.Id.AsLong());

            if (trasmissioneEntity == null)
                throw new TrasmissioneNotFoundPi3Exception(aggregate.Id);

            AssertTrasmissioneInviata(trasmissioneEntity);

            var trasmissioneSingolaEntity = await _dbContext.TrasmSingolaEntities.FindAsync(@event.IdTrasmissioneSingola.AsLong());

            if (trasmissioneSingolaEntity == null)
                throw new TrasmissioneSingolaNotFoundPi3Exception(@event.IdTrasmissioneSingola);

            trasmissioneSingolaEntity.HIDE_DOC_VERSIONS = @event.NewNascondiVersioniPrecedenti.HasValue ? @event.NewNascondiVersioniPrecedenti.Value ? "1" : "0" : null;
        }

        #endregion
    }

}
