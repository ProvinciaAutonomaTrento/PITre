// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.AggregateModels.NotaAggregate;
using Pi3.Core.AggregateModels.NotaAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.NotaAggregate.Repository
{
    public class NotaEFRepository : ElementRepository<Nota>, INotaRepository
    {
        public NotaEFRepository(ILogger<NotaEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;
            InitializeMapper();
        }
        protected readonly IPi3DbContext _dbContext;
        protected IMapper _mapper = null;
        protected override async Task HandleAdd(Nota element)
        {
            var notaEntity = new NotaEntity()
            {
                DATACREAZIONE = element.CreationDate,
                IDOGGETTOASSOCIATO = !string.IsNullOrEmpty(element.OggettoAssociato.Id)? element.OggettoAssociato.Id.AsLong() : 0,
                IDPEOPLEDELEGATO = !string.IsNullOrEmpty(element.Autore.IdUtenteDelegato)? element.Autore.IdUtenteDelegato.AsLong() : 0,
                IDRFASSOCIATO = !string.IsNullOrEmpty(element.IdAccessoRF)? element.IdAccessoRF.AsLong() : 0,
                IDRUOLOCREATORE = !string.IsNullOrEmpty(element.Autore.IdRuolo)? element.Autore.IdRuolo.AsLong() : 0,
                IDUTENTECREATORE = !string.IsNullOrEmpty(element.Autore.IdUtente)? element.Autore.IdUtente.AsLong(): 0,
                //SYSTEM_ID = !string.IsNullOrEmpty(element.Id) ? element.Id.AsLong() : 0,
                TESTO = !string.IsNullOrEmpty(element.Name.ToString())? element.Name.ToString() : string.Empty,
                TIPOOGGETTOASSOCIATO = element.OggettoAssociato.TipoOggetto == TipiOggettoEnum.Documento ? "D" : "F",
                TIPOVISIBILITA = GetTipoVisibilita(element.TipoAccesso)
            }; 


            notaEntity.TIPOVISIBILITA = GetTipoVisibilita(element.TipoAccesso);
            if (notaEntity.TIPOVISIBILITA != "F" && notaEntity.IDRFASSOCIATO == 0)
                notaEntity.IDRFASSOCIATO = null;


            await _dbContext.NoteEntities.AddAsync(notaEntity);
                
            await ((DbContext)_dbContext).SaveChangesAsync();
            element.AssignId(notaEntity.SYSTEM_ID.ToString());
        }
        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var exists = false;
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();
            var notaEntity = await _dbContext
                        .NoteEntities
                        .FirstOrDefaultAsync(p =>
                                p.SYSTEM_ID == idAsLong);

            if (notaEntity != null)
            {
                switch (notaEntity.TIPOVISIBILITA)
                {
                    case "P":
                        exists = notaEntity.IDUTENTECREATORE == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                        break;
                    case "R":
                        exists = notaEntity.IDRUOLOCREATORE == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                        break;
                    case "F":
                        exists =(from rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                                where rr.ID_RUOLO_IN_UO == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true) &&
                                rr.ID_REGISTRO == notaEntity.IDRFASSOCIATO
                                select rr.SYSTEM_ID).Any();
                        break;
                    case "T":
                        exists = true;
                        break;
                    default:
                        exists = false;
                        break;
                }
            }

            return exists;
        }
        protected async override Task HandleUpdate(Nota element)
        {
            var notaEntity = await _dbContext.NoteEntities.FindAsync(element.Id.Trim().AsLong());
            if (notaEntity != null)
            {
                if (GetTipoAccesso(notaEntity.TIPOVISIBILITA) > element.TipoAccesso)
                {
                    throw new NoteNotSupportedPi3Exception(ErrorDescriptions.CambioVisibilitaNonConsentito);
                }
                notaEntity.TIPOVISIBILITA = GetTipoVisibilita(element.TipoAccesso);
                notaEntity.IDRFASSOCIATO = !string.IsNullOrWhiteSpace(element.IdAccessoRF) ? element.IdAccessoRF.AsLong() : null;
                notaEntity.TESTO = element.Description != null ? element.Description.ToString() : null;
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
            else
            {
                throw new NotaNotFoundPi3Exception(element.Id);
            }
        }
        protected async override Task HandleDelete(Nota element)
        {
            bool auth = false;
            switch (element.TipoAccesso)
            {
                case TipoAccessoNotaEnum.Personale:
                    auth = element.Autore.IdUtente == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true).ToString();
                    break;
                case TipoAccessoNotaEnum.Ruolo:
                    auth = element.Autore.IdRuolo == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true).ToString();
                    break;
                case TipoAccessoNotaEnum.RF:
                    auth = (from rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                            where rr.ID_RUOLO_IN_UO == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true) &&
                            rr.ID_REGISTRO == element.IdAccessoRF.AsLong()
                            select rr.SYSTEM_ID).Any();
                    break;
                case TipoAccessoNotaEnum.Pubblica:
                    auth = true;
                    break;
                default:
                    auth = false;
                    break;
            }
            if (auth)
            {
                var notaEntity = await _dbContext
                       .NoteEntities
                .FirstOrDefaultAsync(p =>
                               p.SYSTEM_ID == element.Id.AsLong());
                ((DbContext)_dbContext).Entry<NotaEntity>(notaEntity).State = EntityState.Deleted;
                await ((DbContext)_dbContext).SaveChangesAsync();
            }
        }

        protected override async Task<Nota> HandleGet(Nota newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        { 
            var authenticated = false;

            var idTenantAsNumber = Convert.ToInt64(idTenant);
            var idAsNumber = Convert.ToInt64(id);
            var notaEntity = await _dbContext.NoteEntities.FirstOrDefaultAsync(r => r.SYSTEM_ID == idAsNumber);

            switch (notaEntity.TIPOVISIBILITA)
            {
                case "P":
                    authenticated = notaEntity.IDUTENTECREATORE == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
                    break;
                case "R":
                    authenticated = notaEntity.IDRUOLOCREATORE == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);
                    break;
                case "F":
                    var uo = _dbContext.CorrGlobaliEntities.Where(w => w.ID_GRUPPO == _claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true)).
                        Select(s => s.SYSTEM_ID).FirstOrDefault();
                    authenticated = (from rr in _dbContext.RuoloRegistroEntities.AsNoTracking()
                            where rr.ID_RUOLO_IN_UO == uo &&
                            rr.ID_REGISTRO == notaEntity.IDRFASSOCIATO
                            select rr.SYSTEM_ID).Any();
                    break;
                case "T":
                    authenticated = true;
                    break;
                default:
                    authenticated = false;
                    break;
            }

            if (authenticated)
                return _mapper.Map<Nota>(notaEntity);
            else
                return null;
        }

        protected virtual void InitializeMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NotaEntity, Nota>()
                    .ConstructUsing(src => new Nota(
                    src.SYSTEM_ID.ToString(),
                    _claimsPrincipal.Current.GetPi3ClaimValue<string>(Pi3ClaimTypes.IdTenant, true),
                    src.DATACREAZIONE,
                    new TextValue(string.Format(Descriptions.NoteNameFormat, src.DATACREAZIONE.AsDateTimeFormat())),
                    new TextValue(src.TESTO),
                    new AutoreNota() { IdRuolo = src.IDRUOLOCREATORE.ToString(), IdUtente = src.IDUTENTECREATORE.ToString(), IdUtenteDelegato = src.IDPEOPLEDELEGATO.HasValue ? src.IDPEOPLEDELEGATO.ToString() : "0" },
                    src.IDOGGETTOASSOCIATO.ToString(),
                    src.TIPOOGGETTOASSOCIATO.Equals("D") ? TipiOggettoEnum.Documento : TipiOggettoEnum.Fascicolo,
                    GetTipoAccesso(src.TIPOVISIBILITA),
                    src.IDRFASSOCIATO.HasValue ? src.IDRFASSOCIATO.ToString() : null))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });

                //cfg.CreateMap<Nota, NotaEntity>()
                //   .ForMember(src => src.SYSTEM_ID, opt => opt.MapFrom(dest => dest.Id.AsLong()))
                //   .ForMember(src => src.IDRUOLOCREATORE, opt => opt.MapFrom(dest => dest.Autore.IdRuolo.AsLong()))
                //   .ForMember(src => src.IDUTENTECREATORE, opt => opt.MapFrom(dest => dest.Autore.IdUtente.AsLong()))
                //   .ForMember(src => src.IDPEOPLEDELEGATO, opt => opt.MapFrom(dest => dest.Autore.IdUtenteDelegato.AsLong()))
                //   .ForMember(src => src.TESTO, opt => opt.MapFrom(dest => dest.Name))
                //   .ForMember(src => src.DATACREAZIONE, opt => opt.MapFrom(dest => dest.CreationDate))
                //   .ForMember(src => src.IDOGGETTOASSOCIATO, opt => opt.MapFrom(dest => dest.OggettoAssociato.Id.AsLong()))
                //   .ForMember(src => src.IDRFASSOCIATO, opt => opt.MapFrom(dest => dest.IdAccessoRF.AsLong()))
                //   .ForMember(src => src.TIPOVISIBILITA, opt => opt.MapFrom(dest => GetTipoVisibilita(dest.TipoAccesso)))
                //   .ForMember(src => src.TIPOOGGETTOASSOCIATO, opt => opt.MapFrom(dest => dest.OggettoAssociato.TipoOggetto == TipiOggettoEnum.Documento ? "D" : "F"));

                //aggiugnere anche 
            });
            _mapper = configuration.CreateMapper();
        }

        protected TipoAccessoNotaEnum GetTipoAccesso(string tipoAccesso)
        {
            switch (tipoAccesso)
            {
                case "P":
                    return TipoAccessoNotaEnum.Personale;
                case "R":
                    return TipoAccessoNotaEnum.Ruolo;
                case "F":
                    return TipoAccessoNotaEnum.RF;
                case "T":
                    return TipoAccessoNotaEnum.Pubblica;
                default:
                    return TipoAccessoNotaEnum.Personale;
            }
        }
        protected string GetTipoVisibilita(TipoAccessoNotaEnum TipoAccesso)
        {
            switch (TipoAccesso)
            {
                case TipoAccessoNotaEnum.Personale:
                    return "P";
                case TipoAccessoNotaEnum.Ruolo:
                    return "R";
                case TipoAccessoNotaEnum.RF:
                    return "F";
                case TipoAccessoNotaEnum.Pubblica:
                    return "T";
                default:
                    return null;
            }
        }
    }
}
