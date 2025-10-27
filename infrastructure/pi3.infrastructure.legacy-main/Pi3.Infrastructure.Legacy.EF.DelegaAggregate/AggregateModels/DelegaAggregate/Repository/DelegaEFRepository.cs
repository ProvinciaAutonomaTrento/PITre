// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.DelegaAggregate;
using Pi3.Core.AggregateModels.DelegaAggregate.Events;
using Pi3.Core.AggregateModels.DelegaAggregate.Repositories;
using Pi3.Core.AggregateModels.DelegaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DelegaAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DelegaAggregate.Repository
{
    public class DelegaEFRepository : ElementRepository<Delega>, IDelegaRepository
    {
        #region Public Members

        public DelegaEFRepository(ILogger<DelegaEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
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
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            return await _dbContext
                .DelegheEntities
                .AnyAsync(c => c.SYSTEM_ID == idAsLong);
        }

        protected override async Task<Delega> HandleGet(Delega aggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(id);

            var events = new List<IEvent>();

            events.Add(new ElementCreatedEvent()
            {
                Id = delegaEntity.SYSTEM_ID.ToString(),
                IdTenant = idTenant,
                CreationDate = DateTime.Now
            });

            var idGruppoDelegante = "0";
            var codiceDelegante = "TUTTI";
            var descrizioneDelegante = string.Empty;

            if (delegaEntity.ID_RUOLO_DELEGANTE != 0)
            {
                var gruppoDelegante = await _dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == delegaEntity.ID_RUOLO_DELEGANTE).Select(c => new { c.ID_GRUPPO, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstAsync();
                idGruppoDelegante = gruppoDelegante.ID_GRUPPO.ToString();
                codiceDelegante = gruppoDelegante.VAR_COD_RUBRICA;
                descrizioneDelegante = gruppoDelegante.VAR_DESC_CORR;
            }

            events.Add(new GruppoDeleganteAssignedEvent()
            {
                Id = aggregate.Id,
                IdGruppo = idGruppoDelegante,
                Codice = codiceDelegante,
                Descrizione = descrizioneDelegante
            });

            var utenteDelegante = await _dbContext.PeopleEntities.Where(d => d.SYSTEM_ID == delegaEntity.ID_PEOPLE_DELEGANTE).Select(d => new { d.SYSTEM_ID, d.USER_ID, d.VAR_COGNOME, d.VAR_NOME }).FirstAsync();
            events.Add(new UtenteDeleganteAssignedEvent()
            {
                Id = aggregate.Id,
                IdUtente = utenteDelegante.SYSTEM_ID.ToString(),
                UserId = utenteDelegante.USER_ID,
                Cognome = utenteDelegante.VAR_COGNOME,
                Nome = utenteDelegante.VAR_NOME
            });

            var utenteDelegato = await _dbContext.PeopleEntities.Where(d => d.SYSTEM_ID == delegaEntity.ID_PEOPLE_DELEGATO).Select(d => new { d.SYSTEM_ID, d.USER_ID, d.VAR_COGNOME, d.VAR_NOME }).FirstAsync();
            events.Add(new UtenteDelegatoAssignedEvent()
            {
                Id = aggregate.Id,
                IdUtente = utenteDelegato.SYSTEM_ID.ToString(),
                UserId = utenteDelegato.USER_ID,
                Cognome = utenteDelegato.VAR_COGNOME,
                Nome = utenteDelegato.VAR_NOME
            });

            var gruppoDelegato = await _dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == delegaEntity.ID_RUOLO_DELEGATO).Select(c => new { c.ID_GRUPPO, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstAsync();
            events.Add(new GruppoDelegatoAssignedEvent()
            {
                Id = aggregate.Id,
                IdGruppo = gruppoDelegato.ID_GRUPPO.ToString(),
                Codice = gruppoDelegato.VAR_COD_RUBRICA,
                Descrizione = gruppoDelegato.VAR_DESC_CORR
            });

            events.Add(new DataDecorrenzaChangeEvent()
            {
                Id = aggregate.Id,
                NewData = (DateTime)delegaEntity.DATA_DECORRENZA
            });

            events.Add(new DataScadenzaChangeEvent()
            {
                Id = aggregate.Id,
                NewData = delegaEntity.DATA_SCADENZA
            });

            if (delegaEntity.CHA_IN_ESERCIZIO == "1")
            {
                events.Add(new EsercitataEvent()
                {
                    Id = aggregate.Id,
                    InEsercizio = true
                });
            }

            this.LoadAggregateFromHistory(aggregate, events.ToArray());

            return aggregate;
        }

        protected override async Task HandleAdd(Delega aggregate)
        {
            var idUtenteDelegante = aggregate.UtenteDelegante.Id.AsLong();
            var peopleDelegante = await _dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == idUtenteDelegante  && p.DISABLED == "N").Select(p => new { p.SYSTEM_ID, p.USER_ID }).FirstAsync();
            if (peopleDelegante == null)
                throw new UtenteDeleganteNotFoundPi3Exception(aggregate.UtenteDelegante.Id);

            if (aggregate.DataDecorrenza == null || aggregate.DataDecorrenza.Date < DateTime.Now.Date)
                throw new DataDecorrenzaNotValidPi3Exception();

            var peopleDelegato = await _dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == aggregate.UtenteDelegato.Id.AsLong() && p.DISABLED == "N").Select(p => new { p.SYSTEM_ID, p.USER_ID }).FirstAsync();
            if (peopleDelegato == null)
                throw new UtenteDelegatoNotFoundPi3Exception(aggregate.UtenteDelegato.Id);

            var idRuoloDelegante = aggregate.GruppoDelegante.Id;
            var codRuoloDelegante = aggregate.GruppoDelegante.Codice;
            if (idRuoloDelegante != "0")
            {
                var gruppoDelegante = await _dbContext.CorrGlobaliEntities
                                                .Where(c => c.ID_GRUPPO == aggregate.GruppoDelegante.Id.AsLong() && c.DTA_FINE == null && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I")
                                                .Select(c => new { c.SYSTEM_ID, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstOrDefaultAsync();
                if (gruppoDelegante == null)
                    throw new GruppoDeleganteNotFoundPi3Exception(aggregate.GruppoDelegante.Id);

                idRuoloDelegante = gruppoDelegante.SYSTEM_ID.ToString();
                codRuoloDelegante = gruppoDelegante.VAR_DESC_CORR;
            }
            else
            {
                codRuoloDelegante = "TUTTI";
            }

            var gruppoDelegato = await _dbContext.CorrGlobaliEntities
                                                .Where(c => c.ID_GRUPPO == aggregate.GruppoDelegato.Id.AsLong() && c.DTA_FINE == null && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I")
                                                .Select(c => new { c.SYSTEM_ID, c.ID_UO, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstOrDefaultAsync();
            if (gruppoDelegato == null)
                throw new GruppoDelegatoNotFoundPi3Exception(aggregate.GruppoDelegato.Id);

            var delegaEntity = new DelegaEntity();
            await _dbContext.DelegheEntities.AddAsync(delegaEntity);

            this.LoadAggregateFromHistory(aggregate,
                    new ElementIdAssignedEvent()
                    {
                        Id = delegaEntity.SYSTEM_ID.ToString()
                    });

            await AssertUnicaDelega(aggregate);

            delegaEntity.ID_PEOPLE_DELEGANTE = peopleDelegante.SYSTEM_ID;
            delegaEntity.ID_RUOLO_DELEGANTE = idRuoloDelegante.AsLong();
            delegaEntity.ID_PEOPLE_DELEGATO = peopleDelegato.SYSTEM_ID;
            delegaEntity.ID_RUOLO_DELEGATO = gruppoDelegato.SYSTEM_ID;
            delegaEntity.ID_UO_DELEGATO = gruppoDelegato.ID_UO;
            delegaEntity.COD_PEOPLE_DELEGANTE = peopleDelegante.USER_ID;
            delegaEntity.COD_RUOLO_DELEGANTE = codRuoloDelegante;
            delegaEntity.COD_PEOPLE_DELEGATO = peopleDelegato.USER_ID;
            delegaEntity.COD_RUOLO_DELEGATO = gruppoDelegato.VAR_COD_RUBRICA;
            delegaEntity.DATA_DECORRENZA = aggregate.DataDecorrenza;
            delegaEntity.DATA_SCADENZA = aggregate.DataScadenza;
            delegaEntity.CHA_IN_ESERCIZIO = "0";

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleDelete(Delega aggregate)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleUpdate(Delega aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected virtual async Task HandleChanges(Delega aggregate)
        {
            var idTenant = _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant);

            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            var stato = StatoDelegaEnum.Impostata;
            if (delegaEntity.DATA_DECORRENZA < DateTime.Now && (delegaEntity.DATA_SCADENZA == new DateTime() || delegaEntity.DATA_SCADENZA > DateTime.Now))
                stato = StatoDelegaEnum.Attiva;

            if (delegaEntity.DATA_SCADENZA != new DateTime() && delegaEntity.DATA_SCADENZA < DateTime.Now)
                stato = StatoDelegaEnum.Scaduta;

            await AssertUnicaDelega(aggregate);

            if (aggregate.GetUncommittedChanges().Any(e => e.GetType() == typeof(DataDecorrenzaChangeEvent)))
            {
                if (stato == StatoDelegaEnum.Attiva)
                    throw new DataDecorrenzaNonModificabilePi3Exception();

                if (aggregate.DataDecorrenza == null || aggregate.DataDecorrenza.Date < DateTime.Now.Date)
                    throw new DataDecorrenzaNotValidPi3Exception();
            }

            if (stato != StatoDelegaEnum.Impostata &&
                (aggregate.GetUncommittedChanges().Any(e => e.GetType() == typeof(UtenteDelegatoAssignedEvent))
                || aggregate.GetUncommittedChanges().Any(e => e.GetType() == typeof(GruppoDeleganteAssignedEvent))
                || aggregate.GetUncommittedChanges().Any(e => e.GetType() == typeof(GruppoDelegatoAssignedEvent))))
            {

                if (stato == StatoDelegaEnum.Attiva)
                    delegaEntity.DATA_SCADENZA = DateTime.Now;

                var aggregateNew = new Delega(idTenant, DateTime.Now, null, null);

                var delegaEntityNew = new DelegaEntity();
                delegaEntityNew.CHA_IN_ESERCIZIO = "0";
                await _dbContext.DelegheEntities.AddAsync(delegaEntityNew);

                this.LoadAggregateFromHistory(aggregate,
                        new ElementIdAssignedEvent()
                        {
                            Id = delegaEntityNew.SYSTEM_ID.ToString()
                        });

                aggregateNew.AssignGruppoDelegante(aggregate.GruppoDelegante.Id, aggregate.GruppoDelegante.Codice, aggregate.GruppoDelegante.Descrizione);
                aggregateNew.AssignGruppoDelegato(aggregate.GruppoDelegato.Id, aggregate.GruppoDelegato.Codice, aggregate.GruppoDelegato.Descrizione);
                aggregateNew.AssignUtenteDelegante(aggregate.UtenteDelegante.Id, aggregate.UtenteDelegante.UserId, null, null);
                aggregateNew.AssignUtenteDelegato(aggregate.UtenteDelegato.Id, aggregate.UtenteDelegato.UserId, null, null);
                aggregateNew.ChangeDataDecorrenza(aggregate.DataDecorrenza);
                aggregateNew.ChangeDataScadenza(aggregate.DataScadenza);

                aggregate = aggregateNew;
            }

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

        protected virtual async Task Handle(UtenteDeleganteAssignedEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            var idUtenteAsLong = @event.IdUtente.AsLong();
            var peopleDelegante = await _dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == idUtenteAsLong && p.DISABLED == "N").Select(p => new { p.SYSTEM_ID, p.USER_ID }).FirstAsync();
            if (peopleDelegante == null)
                throw new UtenteDeleganteNotFoundPi3Exception(aggregate.UtenteDelegante.Id);

            delegaEntity.ID_PEOPLE_DELEGANTE = peopleDelegante.SYSTEM_ID;
            delegaEntity.COD_PEOPLE_DELEGANTE = peopleDelegante.USER_ID;
        }

        protected virtual async Task Handle(GruppoDelegatoAssignedEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            var idGruppoAsLong = @event.IdGruppo.AsLong();
            var gruppoDelegato = await _dbContext.CorrGlobaliEntities
                              .Where(c => c.ID_GRUPPO == idGruppoAsLong && c.DTA_FINE == null && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I")
                              .Select(c => new { c.SYSTEM_ID, c.ID_UO, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstOrDefaultAsync();

            if (gruppoDelegato == null)
                throw new GruppoDeleganteNotFoundPi3Exception(@event.Id);

            delegaEntity.ID_RUOLO_DELEGATO = gruppoDelegato.SYSTEM_ID;
            delegaEntity.COD_RUOLO_DELEGATO = gruppoDelegato.VAR_COD_RUBRICA;
            delegaEntity.ID_UO_DELEGATO = gruppoDelegato.ID_UO;
        }

        protected virtual async Task Handle(GruppoDeleganteAssignedEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            var idRuoloDelegante = @event.IdGruppo;
            var codRuoloDelegante = @event.Codice;
            if (idRuoloDelegante != "0")
            {
                var idGruppoAsLong = @event.IdGruppo.AsLong();
                var gruppoDelegante = await _dbContext.CorrGlobaliEntities
                                .Where(c => c.ID_GRUPPO == idGruppoAsLong && c.DTA_FINE == null && c.CHA_TIPO_URP == "R" && c.CHA_TIPO_IE == "I")
                                .Select(c => new { c.SYSTEM_ID, c.VAR_COD_RUBRICA, c.VAR_DESC_CORR }).FirstOrDefaultAsync();

                if (gruppoDelegante == null)
                    throw new GruppoDeleganteNotFoundPi3Exception(@event.Id);

                idRuoloDelegante = gruppoDelegante.SYSTEM_ID.ToString();
                codRuoloDelegante = gruppoDelegante.VAR_DESC_CORR;
            }
            else
            {
                codRuoloDelegante = "TUTTI";
            }

            delegaEntity.ID_RUOLO_DELEGANTE = idRuoloDelegante.AsLong();
            delegaEntity.COD_RUOLO_DELEGANTE = codRuoloDelegante;
        }

        protected virtual async Task Handle(UtenteDelegatoAssignedEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            var idUtenteAsLong = @event.IdUtente.AsLong();
            var peopleDelegato = await _dbContext.PeopleEntities.Where(p => p.SYSTEM_ID == idUtenteAsLong && p.DISABLED == "N").Select(p => new { p.SYSTEM_ID, p.USER_ID }).FirstAsync();
            if (peopleDelegato == null)
                throw new UtenteDelegatoNotFoundPi3Exception(aggregate.UtenteDelegato.Id);

            delegaEntity.ID_PEOPLE_DELEGATO = peopleDelegato.SYSTEM_ID;
            delegaEntity.COD_PEOPLE_DELEGATO = peopleDelegato.USER_ID;
        }

        protected virtual async Task Handle(DataDecorrenzaChangeEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            delegaEntity.DATA_DECORRENZA = @event.NewData;
        }

        protected virtual async Task Handle(DataScadenzaChangeEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            delegaEntity.DATA_SCADENZA = @event.NewData;
        }

        protected virtual async Task Handle(RevocataEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            if (delegaEntity.DATA_DECORRENZA > DateTime.Now)
                _dbContext.DelegheEntities.Remove(delegaEntity);

            if (delegaEntity.DATA_DECORRENZA < DateTime.Now && (delegaEntity.DATA_SCADENZA == null || delegaEntity.DATA_SCADENZA > DateTime.Now))
                delegaEntity.DATA_SCADENZA = DateTime.Now;
        }

        protected virtual async Task Handle(EsercitataEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            if (delegaEntity.DATA_DECORRENZA > DateTime.Now || delegaEntity.DATA_SCADENZA != null && delegaEntity.DATA_SCADENZA < DateTime.Now)
                throw new DelegaNonAttivaPi3Exception();

            delegaEntity.CHA_IN_ESERCIZIO = "1";

            var delegheEntity = await _dbContext.DelegheEntities.Where(d => d.ID_PEOPLE_DELEGATO == delegaEntity.ID_PEOPLE_DELEGATO && d.CHA_IN_ESERCIZIO == "1").ToListAsync();
            foreach (var d in delegheEntity)
                d.CHA_IN_ESERCIZIO = "0";
        }

        protected virtual async Task Handle(DismessaEvent @event, Delega aggregate)
        {
            var delegaEntity = await _dbContext.DelegheEntities.FindAsync(aggregate.Id.AsLong());

            if (delegaEntity == null)
                throw new DelegaNotFoundPi3Exception(@event.Id);

            delegaEntity.CHA_IN_ESERCIZIO = "0";
        }

        protected virtual async Task AssertUnicaDelega(Delega aggregate)
        {
            var delegheEntity = await _dbContext.DelegheEntities
                .Where(d => d.SYSTEM_ID != aggregate.Id.AsLong() && d.ID_PEOPLE_DELEGANTE == aggregate.UtenteDelegante.Id.AsLong() && (d.DATA_SCADENZA == null || d.DATA_SCADENZA >= DateTime.Now))
                .ToListAsync();

            foreach (var d in delegheEntity)
            {
                var idGruppoDelegante = d.ID_RUOLO_DELEGANTE;
                if (d.ID_RUOLO_DELEGANTE != 0)
                {
                    var idGruppoEntity = await _dbContext.CorrGlobaliEntities.Where(c => c.SYSTEM_ID == d.ID_RUOLO_DELEGANTE).Select(c => new { c.ID_GRUPPO }).FirstAsync();
                    idGruppoDelegante = idGruppoEntity.ID_GRUPPO;
                }

                if (aggregate.GruppoDelegante.Id == "0" || idGruppoDelegante == 0 || aggregate.GruppoDelegante.Id.AsLong() == idGruppoDelegante)
                {
                    if (aggregate.DataDecorrenza >= d.DATA_DECORRENZA && (d.DATA_SCADENZA == null || aggregate.DataDecorrenza <= d.DATA_SCADENZA))
                        throw new DelegaUnicaPi3Exception();

                    if (aggregate.DataDecorrenza <= d.DATA_DECORRENZA && (aggregate.DataScadenza == null || aggregate.DataScadenza >= d.DATA_DECORRENZA))
                        throw new DelegaUnicaPi3Exception();
                }
            }
        }

        #endregion
    }
}
