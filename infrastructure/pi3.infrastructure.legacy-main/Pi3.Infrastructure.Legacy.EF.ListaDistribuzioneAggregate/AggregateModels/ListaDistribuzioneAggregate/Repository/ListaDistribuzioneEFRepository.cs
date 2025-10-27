// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Events;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.Repositories;
using Pi3.Core.AggregateModels.ListaDistribuzioneAggregate.ValueObjects;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.ListaDistribuzioneAggregate.Repository
{
    public class ListaDistribuzioneEFRepository : ElementRepository<ListaDistribuzione>, IListaDistribuzioneRepository
    {
        #region Public Members

        public ListaDistribuzioneEFRepository(ILogger<ListaDistribuzioneEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;
            _claimPrincipalService = claimsPrincipalService;

            InitializeMapper();
        }

        #endregion

        #region Private Members

        protected readonly IPi3DbContext _dbContext;
        protected readonly IClaimsPrincipalService _claimPrincipalService;
        protected IMapper _mapper = null;

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idLong = id.AsLong();

            var cgEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(w => w.SYSTEM_ID == idLong
                            && w.ID_AMM == idTenantAsLong
                            && w.CHA_TIPO_URP == "L")
                    .Select(s => new
                    {
                        s.ID_GRUPPO_LISTE,
                        s.ID_PEOPLE_LISTE,
                    }).FirstOrDefaultAsync();

            return cgEntity != null;
        }

        protected override async Task<ListaDistribuzione> HandleGet(ListaDistribuzione aggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            if (!await HandleExists(idTenant, id))
                throw new ListaDistribuzioneNotFoundPi3Exception(id);

            long idLong = id.AsLong();
            var idTenantAsLong = idTenant.AsLong();

            var cgEntity = await _dbContext.CorrGlobaliEntities.AsNoTracking()
                    .Where(w => w.SYSTEM_ID == idLong
                        && w.ID_AMM == idTenantAsLong
                        && w.CHA_TIPO_URP == "L")
                    .Select(s => new
                    {
                        s.ID_GRUPPO_LISTE,
                        s.ID_PEOPLE_LISTE,
                        s.SYSTEM_ID,
                        s.VAR_COD_RUBRICA,
                        s.VAR_DESC_CORR,
                        s.VAR_COGNOME,
                        s.VAR_NOME
                    }).FirstAsync();

            aggregate = new ListaDistribuzione(id, idTenant, DateTime.Now, new TextValue(cgEntity.VAR_COD_RUBRICA), new TextValue(cgEntity.VAR_DESC_CORR));

            if (cgEntity.ID_PEOPLE_LISTE.HasValue)
            {
                var author = await _dbContext.PeopleEntities.FirstAsync(x => x.SYSTEM_ID == cgEntity.ID_PEOPLE_LISTE);
                aggregate.AssignAutorePersona(cgEntity.ID_PEOPLE_LISTE.Value.ToString(), author.USER_ID, author.VAR_NOME, author.VAR_COGNOME);
            }
                
            else if (cgEntity.ID_GRUPPO_LISTE.HasValue)
            {
                var author = await _dbContext.GroupEntities.FirstAsync(x => x.SYSTEM_ID == cgEntity.ID_GRUPPO_LISTE);
                aggregate.AssignAutoreRuolo(cgEntity.ID_GRUPPO_LISTE.Value.ToString(), author.GROUP_ID, author.GROUP_NAME is not null ? new(author.GROUP_NAME) : default);
            }

            var listCor = await _dbContext.ListeDistrEntities
                .Join(_dbContext.CorrGlobaliEntities, l => l.ID_DPA_CORR, c => c.SYSTEM_ID, (l, c) => new { l, c }).
                Where(w => w.l.ID_LISTA_DPA_CORR == idLong).Select(s => new
                {
                    IdCorr = s.c.SYSTEM_ID,
                    IdPeople = s.c.ID_PEOPLE,
                    IdGruppo = s.c.ID_GRUPPO,
                    Descrizione = s.c.VAR_DESC_CORR,
                    TipoRuolo = s.c.CHA_TIPO_URP,
                    EstOrInt = s.c.CHA_TIPO_IE,

                }).ToListAsync();

            foreach (var corr in listCor)
            {
                TipiDestinatariEnum tipoDestinatario = TipiDestinatariEnum.Persona;

                switch (corr.TipoRuolo)
                {
                    case "R":
                        tipoDestinatario = TipiDestinatariEnum.Gruppo;
                        break;
                    case "U":
                        tipoDestinatario = TipiDestinatariEnum.Ufficio;
                        break;
                }

                aggregate.AddDestinatario(corr.IdCorr.ToString(),
                    tipoDestinatario,
                    new TextValue(corr.Descrizione),
                    corr.EstOrInt == "E" ? true : false);
            }

            aggregate.MarkChangesAsCommitted();

            return aggregate;
        }

        protected override async Task HandleAdd(ListaDistribuzione element)
        {
            await HandleChanges(element);
        }

        protected override async Task HandleDelete(ListaDistribuzione element)
        {
            if (!await HandleExists(element.IdTenant, element.Id))
                throw new ListaDistribuzioneNotFoundPi3Exception(element.Id);

            var corrEntity = await _dbContext.CorrGlobaliEntities.FindAsync(element.Id.AsLong());

            _dbContext.CorrGlobaliEntities.Remove(corrEntity);

            var listEntity = await _dbContext.ListeDistrEntities.AsNoTracking()
                             .Where(w => w.ID_LISTA_DPA_CORR == element.Id.AsLong())
                             .Select(s => s).ToListAsync();

            _dbContext.ListeDistrEntities.RemoveRange(listEntity);

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleUpdate(ListaDistribuzione element)
        {
            await HandleChanges(element);
        }

        protected virtual async Task HandleChanges(ListaDistribuzione aggregate)
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

        protected virtual async Task Handle(ElementCreatedEvent @event, ListaDistribuzione aggregate)
        {
            if (await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(w => w.VAR_COD_RUBRICA == @event.Name.ToString()
                        && w.ID_AMM == aggregate.IdTenant.AsLong())
                .AnyAsync())
            {
                throw new ListaDistribuzioneAlreadyExistsPi3Exception(@event.Name);
            }

            var corrGlobaliEntity = new CorrGlobaliEntity()
            {
                VAR_COD_RUBRICA = @event.Name.ToString(),
                VAR_DESC_CORR = @event.Description == null ? @event.Name.ToString() : @event.Description.ToString(),
                CHA_TIPO_URP = "L",
                ID_AMM = @event.IdTenant.AsLong()
            };

            await _dbContext.CorrGlobaliEntities.AddAsync(corrGlobaliEntity);

            this.LoadAggregateFromHistory(aggregate,
                new ElementIdAssignedEvent()
                {
                    Id = corrGlobaliEntity.SYSTEM_ID.ToString()
                });
        }

        protected virtual async Task Handle(ElementNameChangedEvent @event, ListaDistribuzione aggregate)
        {
            if (await _dbContext.CorrGlobaliEntities.AsNoTracking()
                .Where(w => w.VAR_COD_RUBRICA == @event.NewName.ToString()
                        && w.ID_AMM == aggregate.IdTenant.AsLong())
                .AnyAsync())
            {
                throw new ListaDistribuzioneAlreadyExistsPi3Exception(@event.NewName);
            }

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            corrGlobaliEntity.VAR_COD_RUBRICA = @event.NewName.ToString();
        }

        protected virtual async Task Handle(ElementDescriptionChangedEvent @event, ListaDistribuzione aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            corrGlobaliEntity.VAR_DESC_CORR = @event.NewDescription == null ?
                        corrGlobaliEntity.VAR_COD_RUBRICA : @event.NewDescription.ToString();
        }

        protected virtual async Task Handle(AutoreRuoloAssignedEvent @event, ListaDistribuzione aggregate)
        {
            if (!await _dbContext.GroupEntities.AnyAsync(g => g.SYSTEM_ID == @event.IdGruppo.AsLong()))
                throw new GroupNotFoundPi3Exception(@event.IdGruppo);

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            corrGlobaliEntity.ID_PEOPLE_LISTE = null;
            corrGlobaliEntity.ID_GRUPPO_LISTE = @event.IdGruppo.AsLong();
        }

        protected virtual async Task Handle(AutorePersonaAssignedEvent @event, ListaDistribuzione aggregate)
        {
            if (!await _dbContext.PeopleEntities.AnyAsync(g => g.SYSTEM_ID == @event.IdUtente.AsLong()
                            && g.ID_AMM == aggregate.IdTenant.AsLong()))
                throw new UserNotFoundPi3Exception(@event.IdUtente);

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            corrGlobaliEntity.ID_PEOPLE_LISTE = @event.IdUtente.AsLong();
            corrGlobaliEntity.ID_GRUPPO_LISTE = null;
        }

        protected virtual async Task Handle(DestinatarioAddedEvent @event, ListaDistribuzione aggregate)
        {
            var id = @event.IdDestinatario.AsLong();

            //if (!@event.Esterno.GetValueOrDefault() && @event.TipoDestinatario == TipiDestinatariEnum.Persona)
            //{
            //    if (!await _dbContext.PeopleEntities.AnyAsync(g => g.SYSTEM_ID == @event.IdDestinatario.AsLong()
            //            && g.ID_AMM == aggregate.IdTenant.AsLong()))
            //        throw new UserNotFoundPi3Exception(@event.IdDestinatario);

            //    id = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.ID_PEOPLE == @event.IdDestinatario.AsLong()).Select(s => s.SYSTEM_ID).FirstAsync();

            //}
            //else if (!@event.Esterno.GetValueOrDefault() && @event.TipoDestinatario == TipiDestinatariEnum.Gruppo)
            //{
            //    if (!await _dbContext.GroupEntities.AnyAsync(g => g.SYSTEM_ID == @event.IdDestinatario.AsLong()))
            //        throw new GroupNotFoundPi3Exception(@event.IdDestinatario);

            //    id = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.ID_GRUPPO == @event.IdDestinatario.AsLong()).Select(s => s.SYSTEM_ID).FirstAsync();
            //}
            //else
            //{
            //    id = @event.IdDestinatario.AsLong();
            //}

            if (!await _dbContext.CorrGlobaliEntities.AnyAsync(g => g.SYSTEM_ID == id))
                throw new CorrispondentedNotFoundException(@event.IdDestinatario);

            if (!await _dbContext.ListeDistrEntities.AsNoTracking()
                .Where(w => w.ID_LISTA_DPA_CORR == aggregate.Id.AsLong()
                            && w.ID_DPA_CORR == id).AnyAsync())
            {
                var destinatarioEntity = new ListeDistrEntity()
                {
                    ID_LISTA_DPA_CORR = aggregate.Id.AsLong(),
                    ID_DPA_CORR = id
                };

                await _dbContext.ListeDistrEntities.AddAsync(destinatarioEntity);
            }
        }

        protected virtual async Task Handle(DestinatarioRemovedEvent @event, ListaDistribuzione aggregate)
        {
            //var destinatario = aggregate.Destinatari.Where(w => w.Id == @event.IdDestinatario).Select(s => s).First();

            //long id = 0;

            //if (!destinatario.Esterno.GetValueOrDefault() && destinatario.TipoDestinataro == TipiDestinatariEnum.Persona)
            //{
            //    id = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.ID_PEOPLE == destinatario.Id.AsLong()).Select(s => s.SYSTEM_ID).FirstAsync();
            //}
            //else if (!destinatario.Esterno.GetValueOrDefault() && destinatario.TipoDestinataro == TipiDestinatariEnum.Gruppo)
            //{
            //    id = await _dbContext.CorrGlobaliEntities.AsNoTracking().Where(w => w.ID_GRUPPO == destinatario.Id.AsLong()).Select(s => s.SYSTEM_ID).FirstAsync();
            //}
            //else
            //{
            //    id = @event.IdDestinatario.AsLong();
            //}

            var destinatario = await _dbContext.CorrGlobaliEntities.FirstOrDefaultAsync(x => x.SYSTEM_ID == @event.IdDestinatario.AsLong());

            if (destinatario is not null && await _dbContext.ListeDistrEntities.AsNoTracking()
                            .Where(w => w.ID_LISTA_DPA_CORR == aggregate.Id.AsLong()
                                        && w.ID_DPA_CORR == destinatario.SYSTEM_ID).AnyAsync())
            {
                //var destinatarioEntity = await _dbContext.ListeDistrEntities.FindAsync(destinatario.SYSTEM_ID);
                var destinatarioEntity = await _dbContext.ListeDistrEntities.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ID_LISTA_DPA_CORR == aggregate.Id.AsLong() && x.ID_DPA_CORR == destinatario.SYSTEM_ID);

                _dbContext.ListeDistrEntities.Remove(destinatarioEntity);
            }
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {

            });

            _mapper = configuration.CreateMapper();
        }

        #endregion
    }

}
