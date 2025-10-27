// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Entities;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Events;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.Repositories;
using Pi3.App.RubricaComune.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions;
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Resources;
using Pi3.App.RubricaComune.Infrastructure.EF.Entities;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Repositories
{
    public class CorrispondenteEFRepository : Repository<Corrispondente, string>, ICorrispondenteRepository
    {
        #region Public Members

        public CorrispondenteEFRepository(ILogger<CorrispondenteEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IRubricaComuneDbContext dbContext)
            : base(logger, claimsPrincipalService, eventPublisher)
        {
            _dbContext = dbContext;
        }

        public async virtual Task<bool> ExistByCodice(string codice)
        {
            return await this._dbContext.ElementiRubricaEntities.AsNoTracking().AnyAsync(e => e.CODICE == codice);
            
        }

        #endregion

        #region Private Members

        protected readonly IRubricaComuneDbContext _dbContext;

        protected static void AssertPubblicato(Corrispondente aggregate)
        {
            if (aggregate.Pubblicato)
                throw new NotSupportedPi3Exception(ErrorDescriptions.ElementoPubblicato, ErrorDescriptions.ResourceManager);
        }


        protected override async Task<bool> HandleExists(string id)
        {
            return await _dbContext.ElementiRubricaEntities.AnyAsync(e => e.ID == id.AsLong());
        }

        protected override async Task<Corrispondente> HandleGet(Corrispondente newAggregate, string id, params ILoadBehavior[] loadBehaviors)
        {
            if (!await HandleExists(id))
                throw new CorrispondenteNotFoundPi3Exception(id);

            var entity = await _dbContext.ElementiRubricaEntities.AsNoTracking().FirstAsync(e => e.ID == id.AsLong());
            var emailEntities = await this._dbContext.EmailEntities.AsNoTracking().Where(e => e.IDELEMENTORUBRICA == entity.ID).ToListAsync();

            var events = new List<IEvent>();

            if ((entity.CHA_PUBBLICATO ?? "0") == "1")
            {
                events.Add(new CorrispondentePubblicatoEvent()
                {
                    Id = entity.ID.ToString(),
                    DatiPubblicazione = new Core.AggregateModels.CorrispondenteAggregate.ValueObjects.DatiPubblicazione()
                    {
                        Codice = entity.CODICE,
                        Denominazione = entity.DESCRIZIONE,
                        Tipo = (entity.TIPOCORRISPONDENTE == "RF" ? Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.RaggruppamentoFunzionale : Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.UnitaOrganizzativa),
                        CodiceFiscale = entity.VAR_COD_FISC,
                        PartitaIva = entity.VAR_COD_PI,
                        Indirizzo = new Core.AggregateModels.CorrispondenteAggregate.ValueObjects.Indirizzo()
                        {
                             CAP = entity.CAP,
                             Citta = entity.CITTA,
                             Fax = entity.FAX,
                             Nazione = entity.NAZIONE,
                             Provincia = entity.PROVINCIA,
                             Recapito = entity.INDIRIZZO,
                             Telefono = entity.TELEFONO
                        }, 
                        UrlApiInteroperabilita = entity.URL,
                        Amministrazione = entity.AMMINISTRAZIONE,
                        AOO = entity.AOO,
                        Emails = emailEntities
                            .Select(e => 
                                new Core.AggregateModels.CorrispondenteAggregate.ValueObjects.EmailDaAggiornare()
                                {
                                     Email = e.EMAIL,
                                     Note = e.NOTE,
                                     Preferita = e.PREFERITA > 0
                                })
                            .ToList()
                            .AsReadOnly()
                    }
                });
            }
            else
            { 
                events.Add(new CorrispondenteCreatoEvent()
                {
                    Id = entity.ID.ToString(),
                    Codice = entity.CODICE,
                    Denominazione = entity.DESCRIZIONE,
                    DataCreazione = entity.DATACREAZIONE,
                    DataUltimaModifica = entity.DATAULTIMAMODIFICA,
                    Tipo = (entity.TIPOCORRISPONDENTE == "RF" ? Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.RaggruppamentoFunzionale: Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.UnitaOrganizzativa)
                });

                if (!string.IsNullOrWhiteSpace(entity.AMMINISTRAZIONE))
                {
                    events.Add(new AmministrazioneChangedEvent()
                    {
                        NewAmministrazione = entity.AMMINISTRAZIONE
                    });
                }

                if (!string.IsNullOrWhiteSpace(entity.AOO))
                {
                    events.Add(new AOOChangedEvent()
                    {
                        NewAOO = entity.AOO
                    });
                }

                if (!string.IsNullOrWhiteSpace(entity.VAR_COD_FISC))
                {
                    events.Add(new CodiceFiscaleChangedEvent()
                    {
                        NewCodiceFiscale = entity.VAR_COD_FISC
                    });
                }

                if (!string.IsNullOrWhiteSpace(entity.VAR_COD_PI))
                {
                    events.Add(new PartitaIvaChangedEvent()
                    {
                        NewPartitaIva = entity.VAR_COD_PI
                    });
                }

                emailEntities.ForEach(email =>
                {
                    events.Add(new EmailAddedEvent()
                    {
                        IdEmail = email.EMAIL,
                        Note = email.NOTE,
                        Preferita = email.PREFERITA > 0
                    });
                });

                events.Add(new IndirizzoChangedEvent()
                {
                    NewIndirizzo = new Core.AggregateModels.CorrispondenteAggregate.ValueObjects.Indirizzo()
                    {
                        CAP = entity.CAP,
                        Citta = entity.CITTA,
                        Fax = entity.FAX,
                        Nazione = entity.NAZIONE,
                        Provincia = entity.PROVINCIA,
                        Recapito = entity.INDIRIZZO,
                        Telefono = entity.TELEFONO
                    }
                });

                events.Add(new UrlApiInteroperabilitaChangedEvent()
                {
                    NewUrlApiInteroperabilita = entity.URL
                });
            }

            this.LoadAggregateFromHistory(newAggregate, events.ToArray());

            return newAggregate;
        }

        protected override async Task HandleAdd(Corrispondente aggregate)
        {
            await this.HandleChanges(aggregate);
        }
       
        protected override async Task HandleDelete(Corrispondente aggregate)
        {
            AssertPubblicato(aggregate);

            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            this._dbContext.ElementiRubricaEntities.Remove(entity);
            this._dbContext.EmailEntities.RemoveRange(this._dbContext.EmailEntities.Where(e => e.IDELEMENTORUBRICA == entity.ID));

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleUpdate(Corrispondente aggregate)
        {
            await this.HandleChanges(aggregate);
        }

        protected virtual async Task HandleChanges(Corrispondente aggregate)
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

        protected virtual async Task Handle(CorrispondenteCreatoEvent @event, Corrispondente aggregate)
        {
            var idUtenteCreatore = await _dbContext.UtentiEntities.AsNoTracking()
               .Where(u => u.NOME.ToLower() == "sa")
               .Select(u => u.ID)
               .FirstOrDefaultAsync();

            var entity = new ElementoRubricaEntity()
            {
                CODICE = @event.Codice,
                DESCRIZIONE = @event.Denominazione.ToString(),
                DATACREAZIONE = @event.DataCreazione ?? DateTime.Now,
                DATAULTIMAMODIFICA = @event.DataUltimaModifica ?? DateTime.Now,
                IDUTENTECREATORE = idUtenteCreatore,
                TIPOCORRISPONDENTE = @event.Tipo == Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.RaggruppamentoFunzionale ? "RF" : "UO"
            };

            await this._dbContext.ElementiRubricaEntities.AddAsync(entity);

            this.LoadAggregateFromHistory(aggregate, new IdAssignedEvent()
            {
                NewId = entity.ID.ToString()
            });
        }

        protected virtual async Task Handle(AggiornamentoPubblicatoEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.DESCRIZIONE = @event.DatiAggiornamento.Denominazione;
            entity.VAR_COD_FISC = @event.DatiAggiornamento.CodiceFiscale;
            entity.VAR_COD_PI = @event.DatiAggiornamento.PartitaIva;
            entity.AOO = @event.DatiAggiornamento.AOO?.ToString();
            entity.AMMINISTRAZIONE = @event.DatiAggiornamento.Amministrazione?.ToString();
            entity.INDIRIZZO = @event.DatiAggiornamento.Indirizzo?.Recapito?.ToString();
            entity.NAZIONE = @event.DatiAggiornamento.Indirizzo?.Nazione;
            entity.CAP = @event.DatiAggiornamento.Indirizzo?.CAP;
            entity.CITTA = @event.DatiAggiornamento.Indirizzo?.Citta;
            entity.PROVINCIA = @event.DatiAggiornamento.Indirizzo?.Provincia;
            entity.TELEFONO = @event.DatiAggiornamento.Indirizzo?.Telefono;
            entity.FAX = @event.DatiAggiornamento.Indirizzo?.Fax;

            (await this._dbContext.EmailEntities
                .Where(e => e.IDELEMENTORUBRICA == aggregate.Id.AsLong())
                .ToListAsync())
                .ForEach(e => this._dbContext.EmailEntities.Remove(e));

            foreach (var email in @event?.DatiAggiornamento?.Emails ?? Array.Empty<EmailDaAggiornare>())
            {
                await this._dbContext.EmailEntities.AddAsync(new EmailEntity()
                {
                    IDELEMENTORUBRICA = aggregate.Id.AsLong(),
                    EMAIL = email.Email,
                    NOTE = email.Note?.ToString(),
                    PREFERITA = ((email.Preferita ?? false) ? 1 : 0)
                });
            }            
        }

        protected virtual async Task Handle(DenominazioneChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.DESCRIZIONE = @event.NewDenominazione.ToString();            
        }

        protected virtual async Task Handle(TipoChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.TIPOCORRISPONDENTE = @event.NewTipo == TipiEnum.RaggruppamentoFunzionale ? "RF" : "UO";
        }

        protected virtual async Task Handle(AmministrazioneChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.AMMINISTRAZIONE = @event.NewAmministrazione?.ToString();
        }

        protected virtual async Task Handle(AOOChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.AOO = @event.NewAOO?.ToString();
        }

        protected virtual async Task Handle(CodiceFiscaleChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.VAR_COD_FISC = @event.NewCodiceFiscale?.ToString();
        }

        protected virtual async Task Handle(PartitaIvaChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.VAR_COD_PI = @event.NewPartitaIva?.ToString();
        }

        protected virtual async Task Handle(CorrispondentePubblicatoEvent @event, Corrispondente aggregate)
        {
            var idUtenteCreatore = await _dbContext.UtentiEntities.AsNoTracking()
                .Where(u => u.NOME.ToLower() == "sa")
                .Select(u => u.ID)
                .FirstOrDefaultAsync();

            var entity = new ElementoRubricaEntity()
            {
                CODICE = @event.DatiPubblicazione.Codice,
                DESCRIZIONE = @event.DatiPubblicazione.Denominazione.ToString(),
                DATACREAZIONE = DateTime.Now,
                DATAULTIMAMODIFICA = DateTime.Now,
                IDUTENTECREATORE = idUtenteCreatore,
                TIPOCORRISPONDENTE = @event.DatiPubblicazione.Tipo == Core.AggregateModels.CorrispondenteAggregate.ValueObjects.TipiEnum.RaggruppamentoFunzionale ? "RF" : "UO",
                VAR_COD_FISC = @event.DatiPubblicazione.CodiceFiscale,
                VAR_COD_PI = @event.DatiPubblicazione.PartitaIva,
                INDIRIZZO = @event.DatiPubblicazione?.Indirizzo?.Recapito?.ToString(),
                CAP = @event.DatiPubblicazione?.Indirizzo?.CAP,
                CITTA = @event.DatiPubblicazione?.Indirizzo?.Citta,
                PROVINCIA = @event.DatiPubblicazione?.Indirizzo?.Provincia,
                NAZIONE = @event.DatiPubblicazione?.Indirizzo?.Nazione,
                TELEFONO = @event.DatiPubblicazione?.Indirizzo?.Telefono,
                FAX= @event.DatiPubblicazione?.Indirizzo?.Fax,
                URL = @event.DatiPubblicazione?.UrlApiInteroperabilita,
                CHA_PUBBLICATO = "1",
                AMMINISTRAZIONE = @event.DatiPubblicazione?.Amministrazione?.ToString(),
                AOO = @event.DatiPubblicazione?.AOO?.ToString()
            };

            await this._dbContext.ElementiRubricaEntities.AddAsync(entity);

            this.LoadAggregateFromHistory(aggregate, new IdAssignedEvent()
            {
                NewId = entity.ID.ToString()
            });

            foreach (var email in @event?.DatiPubblicazione?.Emails ?? Array.Empty<EmailDaAggiornare>())
            {
                await this._dbContext.EmailEntities.AddAsync(new EmailEntity()
                {
                    IDELEMENTORUBRICA = entity.ID,
                    EMAIL = email.Email,
                    NOTE = email.Note?.ToString(),
                    PREFERITA = ((email.Preferita ?? false) ? 1 : 0)
                });
            }
        }

        protected virtual async Task Handle(EmailAddedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            if (await this._dbContext.EmailEntities.AnyAsync(e => e.IDELEMENTORUBRICA == aggregate.Id.AsLong() 
                && e.EMAIL == @event.IdEmail))
            {
                throw new EmailEntityPi3Exception(@event.IdEmail, ErrorDescriptions.EmailEntityGiaPresente);
            }

            entity.DATAULTIMAMODIFICA = DateTime.Now;

            await this._dbContext.EmailEntities.AddAsync(new EmailEntity()
            {
                IDELEMENTORUBRICA = aggregate.Id.AsLong(),
                EMAIL = @event.IdEmail,
                NOTE = @event.Note?.ToString(),
                PREFERITA = ((@event.Preferita ?? false) ? 1 : 0)
            });

            if (@event.Preferita ?? false)
            {
                this._dbContext.EmailEntities
                    .Where(e => e.EMAIL != @event.IdEmail)
                    .ToList()
                    .ForEach(e => e.PREFERITA = 0);
            }
        }

        protected virtual async Task Handle(EmailNoteChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            var emailEntity = await this._dbContext.EmailEntities
                .FirstOrDefaultAsync(e => e.IDELEMENTORUBRICA == aggregate.Id.AsLong() && e.EMAIL == @event.IdEmail) ?? throw new EmailEntityNotFoundPi3Exception(@event.IdEmail);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            emailEntity.NOTE = @event?.Note?.ToString();
        }

        protected virtual async Task Handle(EmailPreferitaChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            var emailEntities = await this._dbContext.EmailEntities
                            .Where(e => e.IDELEMENTORUBRICA == aggregate.Id.AsLong())
                            .Select(e => e)
                            .ToListAsync();

            var emailEntity = emailEntities.FirstOrDefault(e => e.EMAIL == @event.IdEmail) ?? throw new EmailEntityNotFoundPi3Exception(@event.IdEmail);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            emailEntity.PREFERITA = ((@event?.Preferita ?? false) ? 1 : 0);

            if (emailEntity.PREFERITA == 1)
            {
                emailEntities
                    .Where(e => e.EMAIL != @event?.IdEmail)
                    .ToList()
                    .ForEach(e => e.PREFERITA = 0);
            }
        }

        protected virtual async Task Handle(EmailRemovedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            var emailEntity = await this._dbContext.EmailEntities
                .FirstOrDefaultAsync(e => e.IDELEMENTORUBRICA == aggregate.Id.AsLong() && e.EMAIL == @event.IdEmail) ?? throw new EmailEntityNotFoundPi3Exception(@event.IdEmail);
            entity.DATAULTIMAMODIFICA = DateTime.Now;

            this._dbContext.EmailEntities.Remove(emailEntity);
        }

        protected virtual async Task Handle(IndirizzoChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.INDIRIZZO = @event.NewIndirizzo?.Recapito?.ToString();
            entity.NAZIONE = @event.NewIndirizzo?.Nazione;
            entity.CAP = @event.NewIndirizzo?.CAP;
            entity.CITTA = @event.NewIndirizzo?.Citta;
            entity.PROVINCIA = @event.NewIndirizzo?.Provincia;
            entity.TELEFONO = @event.NewIndirizzo?.Telefono;
            entity.FAX = @event.NewIndirizzo?.Fax;
        }

        protected virtual async Task Handle(UrlApiInteroperabilitaChangedEvent @event, Corrispondente aggregate)
        {
            var entity = await this._dbContext.ElementiRubricaEntities.FindAsync(aggregate.Id.AsLong()) ?? throw new ElementoRubricaEntityNotFoundPi3Exception(aggregate.Id);
            entity.DATAULTIMAMODIFICA = DateTime.Now;
            entity.URL = @event.NewUrlApiInteroperabilita;
        }

        #endregion
    }
}