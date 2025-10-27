// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Events;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Resources;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ModelloTrasmissioneAggregate.Repository
{

    public class ModelloTrasmissioneEFRepository : ElementRepository<ModelloTrasmissione>, IModelloTrasmissioneRepository
    {
        #region Public Members

        public ModelloTrasmissioneEFRepository(ILogger<ModelloTrasmissioneEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;

            InitializeMapper();
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
            });

            _mapper = configuration.CreateMapper();
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var exists = false;
            var modelloTrasmissioneEntity = await _dbContext.ModelloTrasmEntities.AsNoTracking()
                    .Where(mt => mt.SYSTEM_ID == id.AsLong() && mt.ID_AMM == idTenant.AsLong())
                    .Select(mt => new
                    {
                        mt.SYSTEM_ID,
                        mt.ID_PEOPLE
                    })
                    .FirstOrDefaultAsync();

            if (modelloTrasmissioneEntity == null)
                return false;

            if (modelloTrasmissioneEntity.ID_PEOPLE.HasValue)
                exists = modelloTrasmissioneEntity.ID_PEOPLE == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            else
            {
                var idGruppo = _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                var idCorrGlobale = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(c => c.ID_GRUPPO == idGruppo).Select(c => c.SYSTEM_ID).FirstAsync();

                exists = await _dbContext.ModelloMittDestEntities.AsNoTracking()
                    .AnyAsync(d => d.ID_MODELLO == modelloTrasmissioneEntity.SYSTEM_ID && d.CHA_TIPO_MITT_DEST == "M" && d.ID_CORR_GLOBALI == idCorrGlobale);
            }

            return exists;
        }

        protected override async Task<ModelloTrasmissione> HandleGet(ModelloTrasmissione newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            if (!await HandleExists(idTenant, id))
                throw new ModelloTrasmissioneNotFoundPi3Exception(id);

            var modelloTrasmissioneEntity = await _dbContext.ModelloTrasmEntities.FindAsync(id.AsLong());

            var events = new List<IEvent>();

            events.Add(new ModelloTrasmissioneCreatoEvent()
            {
                Id = modelloTrasmissioneEntity.SYSTEM_ID.ToString(),
                IdTenant = idTenant,
                CreationDate = DateTime.Now,
                Name = new TextValue(modelloTrasmissioneEntity.NOME),
                Description = new TextValue(modelloTrasmissioneEntity.VAR_NOTE_GENERALI),
                IdRegistro = modelloTrasmissioneEntity.ID_REGISTRO.ToString(),
                TipoOggettoTrasmesso = modelloTrasmissioneEntity.CHA_TIPO_OGGETTO == "D" ? TipiOggettiTrasmessiEnum.DocumentoAmministrativo : TipiOggettiTrasmessiEnum.Fascicolo
            });

            if (modelloTrasmissioneEntity.ID_PEOPLE.HasValue)
            {
                var peopleEntity = await _dbContext.PeopleEntities.AsNoTracking()
                    .Where(p => p.SYSTEM_ID == modelloTrasmissioneEntity.ID_PEOPLE)
                    .Select(p => new
                    {
                        p.SYSTEM_ID,
                        p.USER_ID,
                        p.VAR_COGNOME,
                        p.VAR_NOME
                    })
                    .FirstOrDefaultAsync();

                if (peopleEntity != null)
                {
                    events.Add(new AutorePersonaAssignedEvent()
                    {
                        IdUser = peopleEntity.SYSTEM_ID.ToString(),
                        UserId = peopleEntity.USER_ID,
                        Cognome = peopleEntity.VAR_COGNOME,
                        Nome = peopleEntity.VAR_NOME
                    });
                }
            }

            var mittDestEntities = await (from md in _dbContext.ModelloMittDestEntities.AsNoTracking()
                                          join rt1 in _dbContext.RagioneTrasmissioneEntities on md.ID_RAGIONE equals rt1.SYSTEM_ID into grouping1
                                          from rt in grouping1.DefaultIfEmpty()
                                          join cg1 in _dbContext.CorrGlobaliEntities.AsNoTracking() on md.ID_CORR_GLOBALI equals cg1.SYSTEM_ID into grouping2
                                          from cg in grouping2.DefaultIfEmpty()
                                          where md.ID_MODELLO == modelloTrasmissioneEntity.SYSTEM_ID
                                          select new
                                          {
                                              MD_SYSTEM_ID = md.SYSTEM_ID,
                                              MD_CHA_TIPO_MITT_DEST = md.CHA_TIPO_MITT_DEST,
                                              MD_ID_CORR_GLOBALI = md.ID_CORR_GLOBALI,
                                              MD_ID_RAGIONE = md.ID_RAGIONE,
                                              MD_CHA_TIPO_TRASM = md.CHA_TIPO_TRASM,
                                              MD_VAR_NOTE_SING = md.VAR_NOTE_SING,
                                              MD_CHA_TIPO_URP = md.CHA_TIPO_URP,
                                              MD_SCADENZA = md.SCADENZA,
                                              MD_HIDE_DOC_VERSIONS = md.HIDE_DOC_VERSIONS,
                                              CG_VAR_CODICE = cg.VAR_CODICE,
                                              CG_VAR_DESC_CORR = cg.VAR_DESC_CORR,
                                              CG_ID_GRUPPO = cg.ID_GRUPPO,
                                              CG_ID_PEOPLE = cg.ID_PEOPLE,
                                              CG_VAR_COGNOME = cg.VAR_COGNOME,
                                              CG_VAR_NOME = cg.VAR_NOME,
                                              CG_CHA_TIPO_URP = cg.CHA_TIPO_URP,
                                              RG_VAR_NOME = rt.VAR_DESC_RAGIONE,
                                              RG_CHA_MANTIENI_LETT = rt.CHA_MANTIENI_LETT,
                                              RG_CHA_MANTIENI_SCRITT = rt.CHA_MANTIENI_SCRITT
                                          })
                                          .ToListAsync();

            foreach (var md in mittDestEntities)
            {
                if (md.MD_CHA_TIPO_MITT_DEST == "M")
                {
                    if (md.MD_ID_CORR_GLOBALI > 0)
                    {
                        var corrGlobaleEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                                .Where(cg => cg.SYSTEM_ID == md.MD_ID_CORR_GLOBALI)
                                .Select(cg => new
                                {
                                    cg.ID_GRUPPO,
                                    cg.VAR_CODICE,
                                    cg.VAR_DESC_CORR
                                })
                                .FirstOrDefaultAsync();

                        if (corrGlobaleEntity != null)
                        {
                            events.Add(new AutoreGruppoAssignedEvent()
                            {
                                IdGruppo = corrGlobaleEntity.ID_GRUPPO.ToString(),
                                Codice = corrGlobaleEntity.VAR_CODICE,
                                Descrizione = new TextValue(corrGlobaleEntity.VAR_DESC_CORR)
                            });
                        }
                    }
                }
                else if (md.MD_CHA_TIPO_MITT_DEST == "D")
                {
                    var cessioneDirittiRagione = md.RG_CHA_MANTIENI_LETT == "1" || md.RG_CHA_MANTIENI_SCRITT == "1" ?
                        new CessioneDirittiRagioneTrasmissione()
                        {
                            MantieniLettura = md.RG_CHA_MANTIENI_LETT == "1",
                            MantieniScrittura = md.RG_CHA_MANTIENI_SCRITT == "1"
                        } : null;

                    switch (md.MD_CHA_TIPO_URP)
                    {
                        case "R":
                            events.Add(new GruppoDestinatarioAddedEvent()
                            {
                                IdGruppo = md.CG_ID_GRUPPO.ToString(),
                                CodiceGruppo = md.CG_VAR_CODICE,
                                DescrizioneGruppo = new TextValue(md.CG_VAR_DESC_CORR),
                                IdRagioneTrasmissione = md.MD_ID_RAGIONE.ToString(),
                                NomeRagioneTrasmissione = md.RG_VAR_NOME,
                                CessioneDirittiRagioneTrasmissione = cessioneDirittiRagione,
                                TipoTrasmissioneSingola = md.MD_CHA_TIPO_TRASM == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                                UtentiNotificati = await _dbContext.ModelloDestConNotificaEntities.AsNoTracking()
                                    .Join(_dbContext.PeopleEntities.AsNoTracking(),
                                            t => t.ID_PEOPLE,
                                            p => p.SYSTEM_ID,
                                            (t, p) => new
                                            {
                                                t.ID_MODELLO_MITT_DEST,
                                                t.ID_PEOPLE,
                                                p.USER_ID,
                                                p.VAR_COGNOME,
                                                p.VAR_NOME
                                            })
                                            .Where(mdn => mdn.ID_MODELLO_MITT_DEST == md.MD_SYSTEM_ID)
                                            .Select(mdn => new DatiUtenteNotificato()
                                            {
                                                IdUtente = mdn.ID_PEOPLE.ToString(),
                                                UserId = mdn.USER_ID,
                                                Cognome = mdn.VAR_COGNOME,
                                                Nome = mdn.VAR_NOME
                                            })
                                            .ToListAsync(),
                                NoteTrasmissioneSingola = !string.IsNullOrWhiteSpace(md.MD_VAR_NOTE_SING) ? new TextValue(md.MD_VAR_NOTE_SING) : null,
                                GiorniScadenza = md.MD_SCADENZA.HasValue ? (int)md.MD_SCADENZA.Value : null,
                                NascondiVersioniPrecedenti = !string.IsNullOrWhiteSpace(md.MD_HIDE_DOC_VERSIONS) ? md.MD_HIDE_DOC_VERSIONS == "1" : null
                            });

                            break;
                        case "P":
                            events.Add(new UtenteDestinatarioAddedEvent()
                            {
                                IdUtente = md.CG_ID_PEOPLE.ToString(),
                                UserId = md.CG_VAR_CODICE,
                                Cognome = md.CG_VAR_COGNOME,
                                Nome = md.CG_VAR_NOME,
                                IdRagioneTrasmissione = md.MD_ID_RAGIONE.ToString(),
                                NomeRagioneTrasmissione = md.RG_VAR_NOME,
                                CessioneDirittiRagioneTrasmissione = cessioneDirittiRagione,
                                NoteTrasmissioneSingola = !string.IsNullOrWhiteSpace(md.MD_VAR_NOTE_SING) ? new TextValue(md.MD_VAR_NOTE_SING) : null,
                                GiorniScadenza = md.MD_SCADENZA.HasValue ? (int)md.MD_SCADENZA.Value : null,
                                NascondiVersioniPrecedenti = !string.IsNullOrWhiteSpace(md.MD_HIDE_DOC_VERSIONS) ? md.MD_HIDE_DOC_VERSIONS == "1" : null
                            });

                            break;
                        case "U":
                            events.Add(new UfficioDestinatarioAdded()
                            {
                                IdUfficio =md.MD_ID_CORR_GLOBALI.ToString(),
                                CodiceUfficio = md.CG_VAR_CODICE,
                                DescrizioneUfficio = new TextValue(md.CG_VAR_DESC_CORR),
                                IdRagioneTrasmissione = md.MD_ID_RAGIONE.ToString(),
                                NomeRagioneTrasmissione = md.RG_VAR_NOME,
                                TipoTrasmissioneSingola = md.MD_CHA_TIPO_TRASM == "T" ? TipiTrasmissioneSingolaEnum.Tutti : TipiTrasmissioneSingolaEnum.Uno,
                                NoteTrasmissioneSingola = !string.IsNullOrWhiteSpace(md.MD_VAR_NOTE_SING) ? new TextValue(md.MD_VAR_NOTE_SING) : null,
                                GiorniScadenza = md.MD_SCADENZA.HasValue ? (int)md.MD_SCADENZA.Value : null,
                                NascondiVersioniPrecedenti = !string.IsNullOrWhiteSpace(md.MD_HIDE_DOC_VERSIONS) ? md.MD_HIDE_DOC_VERSIONS == "1" : null
                            });

                            break;
                        default:
                            break;
                    }
                 }
             }

            if (modelloTrasmissioneEntity.CHA_CEDE_DIRITTI == "1")
            {
                events.Add(new CessioneDirittiEffettuataEvent()
                {
                     CessioneDiritti = new CessioneDiritti()
                     {
                         IdDestinatario = modelloTrasmissioneEntity.ID_GROUP_NEW_OWNER.HasValue ? modelloTrasmissioneEntity.ID_GROUP_NEW_OWNER.ToString() : null,
                         IdUtente = modelloTrasmissioneEntity.ID_PEOPLE_NEW_OWNER.HasValue ? modelloTrasmissioneEntity.ID_PEOPLE_NEW_OWNER.ToString() : null
                     }
                });
            }

            this.LoadAggregateFromHistory(newAggregate, events.ToArray());

            return newAggregate;
        }

        protected override async Task HandleAdd(ModelloTrasmissione aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override async Task HandleDelete(ModelloTrasmissione aggregate)
        {
            var modelloTrasmissioneEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmissioneEntity == null || modelloTrasmissioneEntity != null && modelloTrasmissioneEntity.ID_AMM != aggregate.IdTenant.AsLong())
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            var idModello = aggregate.Id.AsLong();

            var modelliMittDestEntity = await _dbContext.ModelloMittDestEntities.Where(m => m.ID_MODELLO == idModello).ToListAsync();

            var modelliDestNotificaEntity = await _dbContext.ModelloDestConNotificaEntities.Where(m => m.ID_MODELLO == idModello).ToListAsync();

            _dbContext.ModelloTrasmEntities.Remove(modelloTrasmissioneEntity);

            modelliMittDestEntity.ForEach(m => _dbContext.ModelloMittDestEntities.Remove(m));
            modelliDestNotificaEntity.ForEach(m => _dbContext.ModelloDestConNotificaEntities.Remove(m));

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleUpdate(ModelloTrasmissione aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected virtual async Task HandleChanges(ModelloTrasmissione aggregate)
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

        protected virtual async Task Handle(ModelloTrasmissioneCreatoEvent @event, ModelloTrasmissione aggregate)
        {
            if (await _dbContext.ModelloTrasmEntities.AsNoTracking()
                .AnyAsync(m => m.NOME == @event.Name.ToString() && m.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new ModelloTrasmissioneAlreadyExistsPi3Exception(@event.Name);
            }

            var modelloTrasmEntity = new ModelloTrasmEntity();

            await _dbContext.ModelloTrasmEntities.AddAsync(modelloTrasmEntity);

            modelloTrasmEntity.CODICE = $"MT_{modelloTrasmEntity.SYSTEM_ID}";
            modelloTrasmEntity.NOME = @event.Name.ToString();
            modelloTrasmEntity.ID_AMM = aggregate.IdTenant.AsLong();
            modelloTrasmEntity.CHA_TIPO_OGGETTO = @event.TipoOggettoTrasmesso == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "D" : "F";
            modelloTrasmEntity.SINGLE = "0";
            modelloTrasmEntity.CHA_CEDE_DIRITTI = "0";
            modelloTrasmEntity.CHA_MANTIENI_LETTURA = "0";
            modelloTrasmEntity.CHA_MANTIENI_SCRITTURA = "0";
            modelloTrasmEntity.VAR_NOTE_GENERALI = @event.NoteGenerali != null ? @event.NoteGenerali.ToString() : null;

            if (!await _dbContext.RegistroEntities.AsNoTracking()
                .AnyAsync(r => r.SYSTEM_ID == @event.IdRegistro.AsLong() && r.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new RegistroNotFoundPi3Exception(@event.IdRegistro);
            }

            modelloTrasmEntity.ID_REGISTRO = @event.IdRegistro.AsLong();

            var mittDestEntity = new ModelloMittDestEntity();

            await _dbContext.ModelloMittDestEntities.AddAsync(mittDestEntity);

            mittDestEntity.ID_MODELLO = modelloTrasmEntity.SYSTEM_ID;
            mittDestEntity.CHA_TIPO_MITT_DEST = "M";
            mittDestEntity.ID_CORR_GLOBALI = 0;
            mittDestEntity.ID_RAGIONE = 0;
            mittDestEntity.CHA_TIPO_URP = "R";
            mittDestEntity.SCADENZA = 0;

            this.LoadAggregateFromHistory(aggregate, new ElementIdAssignedEvent()
            {
                Id = modelloTrasmEntity.SYSTEM_ID.ToString()
            });
        }

        protected virtual async Task Handle(ElementNameChangedEvent @event, ModelloTrasmissione aggregate)
        {
            var modelloTrasmissioneEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmissioneEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            modelloTrasmissioneEntity.NOME = @event.NewName.ToString();

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected virtual async Task Handle(AutorePersonaAssignedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(cg => cg.ID_PEOPLE == @event.IdUser.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                            .Select(cg => new
                            {
                                cg.SYSTEM_ID,
                                cg.ID_PEOPLE
                            })
                            .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new UtenteNotFoundPi3Exception(@event.IdUser);

            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            modelloTrasmEntity.ID_PEOPLE = @event.IdUser.AsLong();

            var mittDestEntity = ((DbContext)_dbContext).ChangeTracker
                .Entries<ModelloMittDestEntity>()
                .Where(e => e.Entity.ID_MODELLO == aggregate.Id.AsLong()
                            && e.Entity.CHA_TIPO_MITT_DEST == "M")
                .Select(e => e.Entity)
                .FirstOrDefault();

            if (mittDestEntity == null)
                mittDestEntity = await _dbContext.ModelloMittDestEntities.FirstAsync(md =>
                                                        md.ID_MODELLO == aggregate.Id.AsLong()
                                                            && md.CHA_TIPO_MITT_DEST == "M");

            mittDestEntity.ID_CORR_GLOBALI = 0;
        }

        protected virtual async Task Handle(AutoreGruppoAssignedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                            .Where(cg => cg.ID_GRUPPO == @event.IdGruppo.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                            .Select(cg => new
                            {
                                cg.SYSTEM_ID,
                                cg.ID_GRUPPO
                            })
                            .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new GruppoNotFoundPi3Exception(@event.IdGruppo);

            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            modelloTrasmEntity.ID_PEOPLE = null;

            var mittDestEntity = ((DbContext)_dbContext).ChangeTracker
                .Entries<ModelloMittDestEntity>()
                .Where(e => e.Entity.ID_MODELLO == aggregate.Id.AsLong()
                            && e.Entity.CHA_TIPO_MITT_DEST == "M")
                .Select(e => e.Entity)
                .FirstOrDefault();

            if (mittDestEntity == null)
                mittDestEntity = await _dbContext.ModelloMittDestEntities.FirstAsync(md =>
                                                        md.ID_MODELLO == aggregate.Id.AsLong()
                                                            && md.CHA_TIPO_MITT_DEST == "M");

            mittDestEntity.ID_CORR_GLOBALI = corrGlobaliEntity.SYSTEM_ID;
        }

        protected virtual async Task Handle(NoteGeneraliChangedEvent @event, ModelloTrasmissione aggregate)
        {
            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            modelloTrasmEntity.VAR_NOTE_GENERALI = @event.NewNoteGenerali.ToString();
        }

        protected virtual async Task Handle(RegistroChangedEvent @event, ModelloTrasmissione aggregate)
        {
            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            if (!await _dbContext.RegistroEntities.AsNoTracking()
               .AnyAsync(r => r.SYSTEM_ID == @event.IdRegistro.AsLong() && r.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new RegistroNotFoundPi3Exception(@event.IdRegistro);
            }

            modelloTrasmEntity.ID_REGISTRO = @event.IdRegistro.AsLong();
        }

        protected virtual async Task Handle(TipoOggettoTrasmessoChangedEvent @event, ModelloTrasmissione aggregate)
        {
            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            modelloTrasmEntity.CHA_TIPO_OGGETTO = @event.NewTipoOggettoTrasmesso == TipiOggettiTrasmessiEnum.DocumentoAmministrativo ? "D" : "F";
        }

        protected virtual async Task Handle(GruppoDestinatarioAddedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                           .Where(cg => cg.ID_GRUPPO == @event.IdGruppo.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                           .Select(cg => new
                           {
                               cg.SYSTEM_ID,
                               cg.ID_GRUPPO
                           })
                           .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new GruppoNotFoundPi3Exception(@event.IdGruppo);

            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            if (!await _dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                .AnyAsync(r => r.SYSTEM_ID == @event.IdRagioneTrasmissione.AsLong() && r.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new RagioneTrasmissioneNotFoundPi3Exception(@event.IdRagioneTrasmissione);
            }

            var mittDestEntity = new ModelloMittDestEntity();

            mittDestEntity.ID_MODELLO = modelloTrasmEntity.SYSTEM_ID;
            mittDestEntity.CHA_TIPO_MITT_DEST = "D";
            mittDestEntity.ID_CORR_GLOBALI = corrGlobaliEntity.SYSTEM_ID;
            mittDestEntity.ID_RAGIONE = @event.IdRagioneTrasmissione.AsLong();
            mittDestEntity.CHA_TIPO_TRASM = @event.TipoTrasmissioneSingola == TipiTrasmissioneSingolaEnum.Uno ? "S" : "T";
            mittDestEntity.VAR_NOTE_SING = @event.NoteTrasmissioneSingola != null ? @event.NoteTrasmissioneSingola.ToString() : null;
            mittDestEntity.CHA_TIPO_URP = "R";
            mittDestEntity.SCADENZA = @event.GiorniScadenza.HasValue ? @event.GiorniScadenza : 0;
            mittDestEntity.HIDE_DOC_VERSIONS = @event.NascondiVersioniPrecedenti.HasValue ? @event.NascondiVersioniPrecedenti.Value ? "1" : "0" : null;

            await _dbContext.ModelloMittDestEntities.AddAsync(mittDestEntity);

            foreach (var u in @event.UtentiNotificati)
            {
                if (!await _dbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == u.IdUtente.AsLong() && p.ID_AMM == aggregate.IdTenant.AsLong()))
                    throw new UtenteNotFoundPi3Exception(u.IdUtente);

                await _dbContext.ModelloDestConNotificaEntities.AddAsync(new ModelloDestConNotificaEntity()
                {
                    ID_PEOPLE = u.IdUtente.AsLong(),
                    ID_MODELLO_MITT_DEST = mittDestEntity.SYSTEM_ID,
                    ID_MODELLO = aggregate.Id.AsLong()
                });
            }
        }

        protected virtual async Task Handle(GruppoDestinatarioRemovedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                         .Where(cg => cg.ID_GRUPPO == @event.IdGruppo.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                         .Select(cg => new
                         {
                             cg.SYSTEM_ID,
                             cg.ID_GRUPPO
                         })
                         .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new GruppoNotFoundPi3Exception(@event.IdGruppo);

            var mittDestEntity = await _dbContext.ModelloMittDestEntities
                .FirstOrDefaultAsync(md => md.ID_MODELLO == aggregate.Id.AsLong()
                        && md.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID);

            if (mittDestEntity != null)
            {
                if (mittDestEntity.CHA_TIPO_URP == "R")
                {
                    var notificaEntitiesToRemove = await _dbContext.ModelloDestConNotificaEntities
                        .Where(n => n.ID_MODELLO_MITT_DEST == mittDestEntity.SYSTEM_ID)
                        .Select(n => n)
                        .ToListAsync();

                    _dbContext.ModelloDestConNotificaEntities.RemoveRange(notificaEntitiesToRemove);
                }

                _dbContext.ModelloMittDestEntities.Remove(mittDestEntity);
            }
        }

        protected virtual async Task Handle(UtenteDestinatarioAddedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
             .Where(cg => cg.ID_PEOPLE == @event.IdUtente.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
             .Select(cg => new
             {
                 cg.SYSTEM_ID,
                 cg.ID_PEOPLE
             })
             .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new UtenteNotFoundPi3Exception(@event.IdUtente);

            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            if (!await _dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                .AnyAsync(r => r.SYSTEM_ID == @event.IdRagioneTrasmissione.AsLong() && r.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new RagioneTrasmissioneNotFoundPi3Exception(@event.IdRagioneTrasmissione);
            }

            var mittDestEntity = new ModelloMittDestEntity();

            mittDestEntity.ID_MODELLO = modelloTrasmEntity.SYSTEM_ID;
            mittDestEntity.CHA_TIPO_MITT_DEST = "D";
            mittDestEntity.ID_CORR_GLOBALI = corrGlobaliEntity.SYSTEM_ID;
            mittDestEntity.ID_RAGIONE = @event.IdRagioneTrasmissione.AsLong();
            mittDestEntity.CHA_TIPO_TRASM = "S";
            mittDestEntity.VAR_NOTE_SING = @event.NoteTrasmissioneSingola != null ? @event.NoteTrasmissioneSingola.ToString() : null;
            mittDestEntity.CHA_TIPO_URP = "P";
            mittDestEntity.SCADENZA = @event.GiorniScadenza.HasValue ? @event.GiorniScadenza : 0;
            mittDestEntity.HIDE_DOC_VERSIONS = @event.NascondiVersioniPrecedenti.HasValue ? @event.NascondiVersioniPrecedenti.Value ? "1" : "0" : null;

            await _dbContext.ModelloMittDestEntities.AddAsync(mittDestEntity);

            await _dbContext.ModelloDestConNotificaEntities.AddAsync(new ModelloDestConNotificaEntity()
            {
                ID_PEOPLE = @event.IdUtente.AsLong(),
                ID_MODELLO_MITT_DEST = mittDestEntity.SYSTEM_ID,
                ID_MODELLO = aggregate.Id.AsLong()
            });
        }

        protected virtual async Task Handle(UtenteDestinatarioRemoved @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
             .Where(cg => cg.ID_PEOPLE == @event.IdUtente.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
             .Select(cg => new
             {
                 cg.SYSTEM_ID,
                 cg.ID_PEOPLE
             })
             .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new UtenteNotFoundPi3Exception(@event.IdUtente);

            var mittDestEntity = await _dbContext.ModelloMittDestEntities
                .FirstOrDefaultAsync(md => md.ID_MODELLO == aggregate.Id.AsLong()
                        && md.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID);

            if (mittDestEntity != null)
            {
                _dbContext.ModelloMittDestEntities.Remove(mittDestEntity);
            }
        }

        protected virtual async Task Handle(UtentiNotificatiChangedEvent @event, ModelloTrasmissione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                 .Where(cg => cg.ID_GRUPPO == @event.IdGruppoDestinatario.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong())
                 .Select(cg => new
                 {
                     cg.SYSTEM_ID,
                     cg.ID_PEOPLE
                 })
                 .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new UtenteNotFoundPi3Exception(@event.IdGruppoDestinatario);

            var idModelloMittDest = await _dbContext.ModelloMittDestEntities.AsNoTracking()
                    .Where(md => md.ID_MODELLO == aggregate.Id.AsLong() && md.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID)
                    .Select(md => md.SYSTEM_ID)
                    .FirstOrDefaultAsync();

            var notificheToDeleteEntities = await _dbContext.ModelloDestConNotificaEntities.AsNoTracking()
                    .Where(n => n.ID_MODELLO_MITT_DEST == idModelloMittDest)
                    .Select(n => n)
                    .ToListAsync();

            if (notificheToDeleteEntities.Any())
                _dbContext.ModelloDestConNotificaEntities.RemoveRange(notificheToDeleteEntities);

            foreach (var n in @event.NewUtentiNotificati)
            {
                if (!await _dbContext.PeopleEntities.AnyAsync(p => p.SYSTEM_ID == n.IdUtente.AsLong() && p.ID_AMM == aggregate.IdTenant.AsLong()))
                    throw new UtenteNotFoundPi3Exception(n.IdUtente);

                await _dbContext.ModelloDestConNotificaEntities.AddAsync(new ModelloDestConNotificaEntity()
                {
                    ID_MODELLO = aggregate.Id.AsLong(),
                    ID_MODELLO_MITT_DEST = idModelloMittDest,
                    ID_PEOPLE = n.IdUtente.AsLong()
                });
            }
        }

        protected virtual async Task Handle(UfficioDestinatarioAdded @event, ModelloTrasmissione aggregate)
        {
            if (!await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .AnyAsync(cg => cg.SYSTEM_ID == @event.IdUfficio.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong() && !cg.DTA_FINE.HasValue))
            {
                throw new UfficioNotFoundPi3Exception(@event.IdUfficio);
            }

            var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

            if (modelloTrasmEntity == null)
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            if (!await _dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                .AnyAsync(r => r.SYSTEM_ID == @event.IdRagioneTrasmissione.AsLong() && r.ID_AMM == aggregate.IdTenant.AsLong()))
            {
                throw new RagioneTrasmissioneNotFoundPi3Exception(@event.IdRagioneTrasmissione);
            }

            var mittDestEntity = new ModelloMittDestEntity();

            mittDestEntity.ID_MODELLO = modelloTrasmEntity.SYSTEM_ID;
            mittDestEntity.CHA_TIPO_MITT_DEST = "D";
            mittDestEntity.ID_CORR_GLOBALI = @event.IdUfficio.AsLong();
            mittDestEntity.ID_RAGIONE = @event.IdRagioneTrasmissione.AsLong();
            mittDestEntity.CHA_TIPO_TRASM = @event.TipoTrasmissioneSingola == TipiTrasmissioneSingolaEnum.Uno ? "S" : "T";
            mittDestEntity.VAR_NOTE_SING = @event.NoteTrasmissioneSingola != null ? @event.NoteTrasmissioneSingola.ToString() : null;
            mittDestEntity.CHA_TIPO_URP = "U";
            mittDestEntity.HIDE_DOC_VERSIONS = @event.NascondiVersioniPrecedenti.HasValue ? @event.NascondiVersioniPrecedenti.Value ? "1" : "0" : null;

            await _dbContext.ModelloMittDestEntities.AddAsync(mittDestEntity);
        }

        protected virtual async Task Handle(UfficioDestinatarioRemoved @event, ModelloTrasmissione aggregate)
        {
            if (!await _dbContext.CorrGlobaliEntities.AsNoTracking()
                        .AnyAsync(cg => cg.SYSTEM_ID == @event.IdUfficio.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong() && !cg.DTA_FINE.HasValue))
            {
                throw new UfficioNotFoundPi3Exception(@event.IdUfficio);
            }

            var mittDestEntity = await _dbContext.ModelloMittDestEntities
                .FirstOrDefaultAsync(md => md.ID_MODELLO == aggregate.Id.AsLong()
                        && md.ID_CORR_GLOBALI == @event.IdUfficio.AsLong());

            if (mittDestEntity != null)
            {
                _dbContext.ModelloMittDestEntities.Remove(mittDestEntity);
            }
        }

        protected virtual async Task Handle(OpzioniGruppoDestinatarioChangedEvent @event, ModelloTrasmissione aggregate)
        {
            if (!await HandleExists(aggregate.IdTenant, aggregate.Id))
                throw new ModelloTrasmissioneNotFoundPi3Exception(aggregate.Id);

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(cg => cg.ID_GRUPPO == @event.IdGruppoDestinatario.AsLong() && cg.ID_AMM == aggregate.IdTenant.AsLong() && !cg.DTA_FINE.HasValue)
                    .Select(cg => new { cg.SYSTEM_ID })
                    .FirstOrDefaultAsync();

            if (corrGlobaliEntity == null)
                throw new GruppoNotFoundPi3Exception(@event.IdGruppoDestinatario);

            var mittDestEntity = await _dbContext.ModelloMittDestEntities.FirstAsync(mt => mt.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID);

            mittDestEntity.SCADENZA = @event.GiorniScadenza.HasValue ? @event.GiorniScadenza.Value : null;
            mittDestEntity.HIDE_DOC_VERSIONS = @event.NascondiVersioniPrecedenti.HasValue ? @event.NascondiVersioniPrecedenti.Value ? "1" : "0" : null;
            mittDestEntity.VAR_NOTE_SING = @event.NoteTrasmissioneSingola != null ? @event.NoteTrasmissioneSingola.ToString() : null;

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected virtual async Task Handle(OpzioniUtenteDestinatarioChangedEvent @event, ModelloTrasmissione aggregate)
        {
        }

        protected virtual async Task Handle(OpzioniUfficioDestinatarioChangedEvent @event, ModelloTrasmissione aggregate)
        {
        }

        protected virtual async Task Handle(CessioneDirittiEffettuataEvent @event, ModelloTrasmissione aggregate)
        {
            var destinatario = aggregate.Destinatari.First(d => d.Id == @event.CessioneDiritti.IdDestinatario);

            var ragioneEntity = await _dbContext.RagioneTrasmissioneEntities.AsNoTracking()
                    .Where(r => r.SYSTEM_ID == destinatario.RagioneTrasmissione.Id.AsLong())
                    .Select(r => new
                    {
                        r.CHA_CEDE_DIRITTI,
                        r.CHA_MANTIENI_LETT,
                        r.CHA_MANTIENI_SCRITT
                    })
                    .FirstAsync();

            if (ragioneEntity.CHA_CEDE_DIRITTI != "N")
            {
                var modelloTrasmEntity = await _dbContext.ModelloTrasmEntities.FindAsync(aggregate.Id.AsLong());

                modelloTrasmEntity.ID_GROUP_NEW_OWNER = @event.CessioneDiritti.IdDestinatario.AsLong();

                if (!string.IsNullOrWhiteSpace(@event.CessioneDiritti.IdUtente))
                    modelloTrasmEntity.ID_PEOPLE_NEW_OWNER = @event.CessioneDiritti.IdUtente.AsLong();

                modelloTrasmEntity.CHA_CEDE_DIRITTI = "1";
                modelloTrasmEntity.CHA_MANTIENI_LETTURA = ragioneEntity.CHA_MANTIENI_LETT;
                modelloTrasmEntity.CHA_MANTIENI_SCRITTURA = ragioneEntity.CHA_MANTIENI_SCRITT;

                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            else
            {
                // La ragione trasmissione non prevede la cessione dei diritti
                throw new NotSupportedPi3Exception(ErrorDescriptions.RagioneTrasmissioneNonPrevedeCessioneDiritti, ErrorDescriptions.ResourceManager);
            }
        }

        #endregion
    }

}
