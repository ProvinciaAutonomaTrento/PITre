// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.CorrispondenteAggregate;
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate;
using Pi3.Core.Extensions;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Infrastructure.Legacy.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Exceptions;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Resources;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Events;
using Pi3.Core.AggregateModels.CorrispondenteAggregate.ValueObjects;
using Pi3.Core.AggregateModels.ElementAggregate.Events;
using Pi3.Core.AggregateModels.ElementAggregate.Repositories;
using Pi3.Core.AggregateModels.PersonaCorrispondenteAggregate.Repositories;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Repository
{
    public class PersonaCorrispondenteEFRepository : ElementRepository<PersonaCorrispondente>, IPersonaCorrispondenteRepository
    {
        #region Public Members

        public PersonaCorrispondenteEFRepository(ILogger<PersonaCorrispondenteEFRepository> logger, IClaimsPrincipalService claimsPrincipalService, IEventPublisher eventPublisher, IPi3DbContext dbContext)
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
                cfg.CreateMap<CorrGlobaliEntity, PersonaCorrispondente>()
                    .ConstructUsing(src => new PersonaCorrispondente(
                        src.SYSTEM_ID.ToString(),
                        src.ID_AMM.GetValueOrDefault().ToString(),
                        (DateTime)src.DTA_INIZIO,
                        new TextValue(src.VAR_COD_RUBRICA),
                        new TextValue(src.VAR_DESC_CORR),
                        src.VAR_NOME,
                        src.VAR_COGNOME,
                        src.ID_REGISTRO.ToString()))
                    .AfterMap((src, dest) =>
                    {
                        dest.MarkChangesAsCommitted();
                    });
            });

            _mapper = configuration.CreateMapper();
        }

        protected override async Task<bool> HandleExists(string idTenant, string id)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();

            return await _dbContext
                .CorrGlobaliEntities
                .AnyAsync(c => c.SYSTEM_ID == idAsLong && (c.ID_AMM == idTenantAsLong || c.ID_AMM == null) && c.CHA_TIPO_IE == "E" && c.CHA_TIPO_URP == "P");
        }

        protected override async Task<PersonaCorrispondente> HandleGet(PersonaCorrispondente newAggregate, string idTenant, string id, ILoadBehavior[]? loadBehaviors = null)
        {
            var idTenantAsLong = idTenant.AsLong();
            var idAsLong = id.AsLong();
            var events = new List<IEvent>();

            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FirstOrDefaultAsync(c => c.SYSTEM_ID == idAsLong && (c.ID_AMM == idTenantAsLong || c.ID_AMM == null) && c.CHA_TIPO_IE == "E");
            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(id);

            if (corrGlobaliEntity.CHA_TIPO_URP != "P")
                throw new PersonaCorrispondenteNotValidPi3Exception(id);

            var aggregate = _mapper.Map<PersonaCorrispondente>(corrGlobaliEntity);
            events.Add(new DataFineSettedEvent()
            {
                Id = aggregate.Id,
                NewDataFine = corrGlobaliEntity.DTA_FINE
            });
            events.Add(new CodiceAmministrazioneSettedEvent()
            {
                Id = aggregate.Id,
                NewCodiceAmministrazione = corrGlobaliEntity.VAR_CODICE_AMM
            });
            events.Add(new CodiceAOOSettedEvent()
            {
                Id = aggregate.Id,
                NewCodiceAOO = corrGlobaliEntity.VAR_CODICE_AOO
            });
            events.Add(new RubricaComuneSettedEvent()
            {
                Id = aggregate.Id,
                NewRubricaComune = corrGlobaliEntity.CHA_TIPO_CORR == "C"
            });
            events.Add(new RubricaEsternaSettedEvent()
            {
                Id = aggregate.Id,
                NewRubricaEsterna = corrGlobaliEntity.RUBRICA_ESTERNA
            });
            events.Add(new CodiceSettedEvent()
            {
                Id = aggregate.Id,
                Codice = corrGlobaliEntity.VAR_CODICE
            });
            events.Add(new IdOldSettedEvent()
            {
                Id = aggregate.Id,
                IdOld = corrGlobaliEntity.ID_OLD.ToString()
            });
            events.Add(new DescriptionOldSettedEvent()
            {
                Id = aggregate.Id,
                DescriptionOld = corrGlobaliEntity.VAR_DESC_CORR_OLD
            });
            events.Add(new InteropUrlSettedEvent()
            {
                Id = aggregate.Id,
                InteropUrl = corrGlobaliEntity.INTEROPURL
            });

            var mailCorrEsterniEntity = await _dbContext.MailCorrEsterniEntities.Where(e => e.ID_CORR == idAsLong).ToListAsync();
            foreach (var m in mailCorrEsterniEntity)
            {
                events.Add(new EmailAddedEvent()
                {
                    Id = aggregate.Id,
                    IdEmail = m.SYSTEM_ID.ToString(),
                    IndirizzoEmail = m.VAR_EMAIL,
                    Principale = m.VAR_PRINCIPALE == "1",
                    Note = new TextValue(m.VAR_NOTE)
                });
            }

            var canaleCorrEntity = await _dbContext.CanaleCorrEntities.FirstOrDefaultAsync(c => c.ID_CORR_GLOBALE == idAsLong);
            events.Add(new CanalePreferenzialeSettedEvent()
            {
                Id = aggregate.Id,
                IdCanalePreferenziale = canaleCorrEntity.ID_DOCUMENTTYPE.ToString()
            });

            events.Add(new NomeChangedEvent()
            {
                Id = aggregate.Id,
                NewNome = corrGlobaliEntity.VAR_NOME
            });

            events.Add(new CognomeChangedEvent()
            {
                Id = aggregate.Id,
                NewCognome = corrGlobaliEntity.VAR_COGNOME
            });

            if (corrGlobaliEntity.CHA_DETTAGLI == "1")
            {
                var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == corrGlobaliEntity.SYSTEM_ID);

                var indirizzo = new IndirizzoCorrispondente()
                {
                    Indirizzo = dettGlobaliEntity.VAR_INDIRIZZO,
                    Provincia = dettGlobaliEntity.VAR_PROVINCIA,
                    Cap = dettGlobaliEntity.VAR_CAP,
                    Nazione = dettGlobaliEntity.VAR_NAZIONE,
                    Citta = dettGlobaliEntity.VAR_CITTA,
                    Localita = dettGlobaliEntity.VAR_LOCALITA,
                    Fax = dettGlobaliEntity.VAR_FAX,
                    TelefonoPrincipale = dettGlobaliEntity.VAR_TELEFONO,
                    TelefonoSecondario = dettGlobaliEntity.VAR_TELEFONO2
                };
                events.Add(new IndirizzoChangedEvent()
                {
                    Id = aggregate.Id,
                    NewIndirizzo = indirizzo
                });

                events.Add(new CodiceFiscaleSettedEvent()
                {
                    Id = aggregate.Id,
                    NewCodiceFiscale = dettGlobaliEntity.VAR_COD_FISC
                });

                events.Add(new PartitaIvaSettedEvent()
                {
                    Id = aggregate.Id,
                    NewPartitaIva = dettGlobaliEntity.VAR_COD_PI
                });

                events.Add(new DataNascitaSettedEvent()
                {
                    Id = aggregate.Id,
                    NewDataNascita = dettGlobaliEntity.DTA_NASCITA?.AsDateTime()
                });

                events.Add(new LuogoNascitaSettedEvent()
                {
                    Id = aggregate.Id,
                    NewLuogoNascita = dettGlobaliEntity.VAR_LUOGO_NASCITA
                });

                events.Add(new NoteSettedEvent()
                {
                    Id = aggregate.Id,
                    NewNote = dettGlobaliEntity.VAR_NOTE
                });

                events.Add(new TitoloSettedEvent()
                {
                    Id = aggregate.Id,
                    Titolo = dettGlobaliEntity.VAR_TITOLO
                });
            }

            if (events.Count > 0)
                this.LoadAggregateFromHistory(aggregate, events.ToArray());

            return aggregate;
        }

        protected override async Task HandleAdd(PersonaCorrispondente aggregate)
        {
            await HandleChanges(aggregate);
        }

        protected override async Task HandleDelete(PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

            if (corrGlobaliEntity.DTA_FINE != null && corrGlobaliEntity.DTA_FINE > DateTime.MinValue)
                throw new PersonaCorrispondenteCanNotBeRemovedPi3Exception(aggregate.Id);

            var authorization = "DO_MOD_CORR_TUTTI";
            if (!string.IsNullOrEmpty(aggregate.IdRegistro))
            {
                var chaRF = await _dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == aggregate.IdRegistro.AsLong()).Select(r => r.CHA_RF).FirstAsync();
                if (chaRF == "1")
                    authorization = "DO_MOD_CORR_RF";
                if (chaRF == "0")
                    authorization = "DO_MOD_CORR_REG";
            }
            this._claimsPrincipal.Current.AssertPi3Authorization(authorization);

            var aggregateNew = await Storicizza(aggregate);
            if (!aggregateNew.Item2)
            {
                var canaleCorrEntity = await _dbContext.CanaleCorrEntities.FirstAsync(c => c.ID_CORR_GLOBALE == aggregate.Id.AsLong());
                _dbContext.CanaleCorrEntities.Remove(canaleCorrEntity);

                _dbContext.CorrGlobaliEntities.Remove(corrGlobaliEntity);
            }

            await ((DbContext)_dbContext).SaveChangesAsync();
        }

        protected override async Task HandleUpdate(PersonaCorrispondente aggregate)
        {
            await HandleChanges(aggregate);
        }

        public async Task<(PersonaCorrispondente, bool)> Storicizza(PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            var idUserAsLong = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdUser, true);
            var idGroup = this._claimsPrincipal.Current.GetPi3ClaimValue<long>(Pi3ClaimTypes.IdGroup, true);

            var inDocArrivoPar = await _dbContext.DocArrivoParEntities.AnyAsync(d => d.ID_MITT_DEST == aggregate.Id.AsLong());
            var inCorrSto = await _dbContext.CorrStoEntities.AnyAsync(d => d.ID_MITT_DEST == aggregate.Id.AsLong());
            var inAssociazioneTemplates = await _dbContext.AssociazioneTemplatesEntities.AnyAsync(a => a.VALORE_OGGETTO_DB == aggregate.Id);
            var inAssTemplatesFasc = await _dbContext.AssTemplatesFascEntities.AnyAsync(a => a.VALORE_OGGETTO_DB == aggregate.Id);

            if (inDocArrivoPar || inCorrSto || inAssociazioneTemplates || inAssTemplatesFasc)
            {
                var idRuoloInUO = await _dbContext.CorrGlobaliEntities
                        .Where(c => c.ID_GRUPPO == idGroup)
                        .Select(c => c.SYSTEM_ID).FirstAsync();

                var codice = corrGlobaliEntity.VAR_COD_RUBRICA + "_" + aggregate.Id;
                corrGlobaliEntity.DTA_FINE = DateTime.Now;
                corrGlobaliEntity.VAR_COD_RUBRICA = codice;
                corrGlobaliEntity.VAR_CODICE = codice;
                corrGlobaliEntity.ID_PARENT = null;
                if (inAssociazioneTemplates)
                {
                    var associazioneTemplateEntities = await (from a in _dbContext.AssociazioneTemplatesEntities
                                                              join o in _dbContext.OggettiCustomEntities on a.ID_OGGETTO equals o.SYSTEM_ID
                                                              join t in _dbContext.TipoOggettoEntities on o.ID_TIPO_OGGETTO equals t.SYSTEM_ID
                                                              where a.VALORE_OGGETTO_DB == aggregate.Id && t.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                              select new
                                                              {
                                                                  a.DOC_NUMBER,
                                                                  a.ID_TEMPLATE,
                                                                  a.ID_OGGETTO
                                                              })
                                                                .ToListAsync();
                    foreach (var a in associazioneTemplateEntities)
                    {
                        var profilStoEntity = new ProfilStoEntity();
                        profilStoEntity.ID_TEMPLATE = (long)a.ID_TEMPLATE;
                        profilStoEntity.DTA_MODIFICA = DateTime.Now;
                        profilStoEntity.ID_PROFILE = a.DOC_NUMBER.AsLong();
                        profilStoEntity.ID_OGG_CUSTOM = (long)a.ID_OGGETTO;
                        profilStoEntity.ID_PEOPLE = idUserAsLong;
                        profilStoEntity.ID_RUOLO_IN_UO = idRuoloInUO;
                        profilStoEntity.VAR_DESC_MODIFICA = Descriptions.CorrispondenteStoricizzato;
                    }
                }
                if (inAssTemplatesFasc)
                {
                    var associazioneTemplateFascEntities = await (from a in _dbContext.AssTemplatesFascEntities
                                                                  join o in _dbContext.OggettiCustomEntities on a.ID_OGGETTO equals o.SYSTEM_ID
                                                                  join t in _dbContext.TipoOggettoEntities on o.ID_TIPO_OGGETTO equals t.SYSTEM_ID
                                                                  where a.VALORE_OGGETTO_DB == aggregate.Id && t.DESCRIZIONE.ToUpper().Equals("CORRISPONDENTE")
                                                                  select new
                                                                  {
                                                                      a.ID_PROJECT,
                                                                      a.ID_TEMPLATE,
                                                                      a.ID_OGGETTO
                                                                  })
                                                                .ToListAsync();
                    foreach (var a in associazioneTemplateFascEntities)
                    {
                        var profilFascStoEntity = new ProfilFascStoEntity();
                        profilFascStoEntity.ID_TEMPLATE = (long)a.ID_TEMPLATE;
                        profilFascStoEntity.DTA_MODIFICA = DateTime.Now;
                        profilFascStoEntity.ID_PROJECT = a.ID_PROJECT.AsLong();
                        profilFascStoEntity.ID_OGG_CUSTOM = (long)a.ID_OGGETTO;
                        profilFascStoEntity.ID_PEOPLE = idUserAsLong;
                        profilFascStoEntity.ID_RUOLO_IN_UO = idRuoloInUO;
                        profilFascStoEntity.VAR_DESC_MODIFICA = Descriptions.CorrispondenteStoricizzato;
                    }
                }

                PersonaCorrispondente aggregateNew = new PersonaCorrispondente(aggregate.IdTenant, DateTime.Now, aggregate.Name, aggregate.Description, aggregate.Nome, aggregate.Cognome, aggregate.IdRegistro);
                aggregateNew.SetCanalePreferenziale(aggregate.CanalePreferenziale.Id);
                aggregateNew.SetCodice(aggregate.Codice);
                aggregateNew.SetCodiceAmministrazione(aggregate.CodiceAmministrazione);
                aggregateNew.SetCodiceAOO(aggregate.CodiceAOO);
                aggregateNew.SetDescriptionOld(aggregate.DescriptionOld);
                aggregateNew.SetIdOld(aggregate.Id);
                aggregateNew.SetInteropUrl(aggregate.InteropUrl);
                aggregateNew.SetRubricaComune((bool)aggregate.RubricaComune);
                aggregateNew.SetRubricaEsterna(aggregate.RubricaEsterna);

                if (aggregate.Indirizzo != null)
                    aggregateNew.ChangeIndirizzo(aggregate.Indirizzo);

                if (!string.IsNullOrEmpty(aggregate.CodiceFiscale))
                    aggregateNew.SetCodiceFiscale(aggregate.CodiceFiscale);

                if (!string.IsNullOrEmpty(aggregate.PartitaIva))
                    aggregateNew.SetPartitaIva(aggregate.PartitaIva);

                if (!string.IsNullOrEmpty(aggregate.Note))
                    aggregateNew.SetNote(aggregate.Note);

                if (aggregate.DataNascita != null && aggregate.DataNascita > DateTime.MinValue)
                    aggregateNew.SetDataNascita(aggregate.DataNascita);

                if (!string.IsNullOrEmpty(aggregate.LuogoNascita))
                    aggregateNew.SetLuogoNascita(aggregate.LuogoNascita);

                if (!string.IsNullOrEmpty(aggregate.Titolo))
                    aggregateNew.SetTitolo(aggregate.Titolo);

                foreach (var email in aggregate.Email)
                    aggregateNew.AddEmail(null, email.IndirizzoEmail, email.Note, email.Principale);

                return (aggregateNew, true);
            }

            return (aggregate, false);
        }

        protected virtual async Task HandleChanges(PersonaCorrispondente aggregate)
        {
            if (!string.IsNullOrEmpty(aggregate.Id))
            {
                var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

                if (corrGlobaliEntity == null)
                    throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

                if (corrGlobaliEntity.DTA_FINE != null && corrGlobaliEntity.DTA_FINE > DateTime.MinValue)
                    throw new PersonaCorrispondenteCanNotBeRemovedPi3Exception(aggregate.Id);

                var authorization = "DO_MOD_CORR_TUTTI";
                if (!string.IsNullOrEmpty(aggregate.IdRegistro))
                {
                    var chaRF = await _dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == aggregate.IdRegistro.AsLong()).Select(r => r.CHA_RF).FirstAsync();
                    if (chaRF == "1")
                        authorization = "DO_MOD_CORR_RF";
                    if (chaRF == "0")
                        authorization = "DO_MOD_CORR_REG";
                }
                this._claimsPrincipal.Current.AssertPi3Authorization(authorization);

                var aggregateNew = await Storicizza(aggregate);
                if (aggregateNew.Item2)
                    aggregate = aggregateNew.Item1;
            }
            else
            {
                var authorization = "DO_INS_CORR_TUTTI";
                if (!string.IsNullOrEmpty(aggregate.IdRegistro))
                {
                    var chaRF = await _dbContext.RegistroEntities.Where(r => r.SYSTEM_ID == aggregate.IdRegistro.AsLong()).Select(r => r.CHA_RF).FirstAsync();
                    if (chaRF == "1")
                        authorization = "DO_INS_CORR_RF";
                    if (chaRF == "0")
                        authorization = "DO_INS_CORR_REG";
                }
                this._claimsPrincipal.Current.AssertPi3Authorization(authorization);
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

        protected virtual async Task Handle(PersonaCorrispondenteCreatedEvent @event, PersonaCorrispondente aggregate)
        {
            var idTenantAsLong = aggregate.IdTenant.AsLong();

            var var_codice_rubrica = aggregate.Name.ToString().ToUpper();
            var corrispondenti = await _dbContext.CorrGlobaliEntities.Where(c => c.VAR_COD_RUBRICA.ToUpper().Equals(var_codice_rubrica) && c.DTA_FINE == null).ToListAsync();
            if (corrispondenti != null && corrispondenti.Count > 0)
            {
                if (corrispondenti.Any(c => c.DTA_FINE == null) &&
                    (!string.IsNullOrEmpty(aggregate.IdTenant) && corrispondenti.Any(c => c.ID_AMM == idTenantAsLong || c.ID_AMM == null) ||
                    !string.IsNullOrEmpty(aggregate.IdRegistro) && corrispondenti.Any(c => c.ID_REGISTRO == aggregate.IdRegistro.AsLong()) ||
                    string.IsNullOrEmpty(aggregate.IdRegistro) && corrispondenti.Any(c => c.ID_REGISTRO == null)))
                    throw new PersonaCorrispondenteExistsPi3Exception(var_codice_rubrica);
            }

            var corrGlobaliEntity = new CorrGlobaliEntity();

            await _dbContext.CorrGlobaliEntities.AddAsync(corrGlobaliEntity);

            this.LoadAggregateFromHistory(aggregate,
                    new ElementIdAssignedEvent()
                    {
                        Id = corrGlobaliEntity.SYSTEM_ID.ToString()
                    });

            corrGlobaliEntity.ID_AMM = !string.IsNullOrEmpty(aggregate.IdTenant) ? aggregate.IdTenant.AsLong() : null;
            corrGlobaliEntity.VAR_COD_RUBRICA = aggregate.Name.ToString();
            corrGlobaliEntity.VAR_CODICE = aggregate.Name.ToString();
            corrGlobaliEntity.VAR_DESC_CORR = !string.IsNullOrEmpty(aggregate.Titolo) ? string.Format("{0} {1} {2}", aggregate.Titolo, aggregate.Cognome, aggregate.Nome) : string.Format("{0} {1}", aggregate.Cognome, aggregate.Nome);
            corrGlobaliEntity.DTA_INIZIO = aggregate.CreationDate;
            corrGlobaliEntity.ID_REGISTRO = !string.IsNullOrEmpty(aggregate.IdRegistro) ? aggregate.IdRegistro.AsLong() : null;
            corrGlobaliEntity.VAR_COGNOME = aggregate.Cognome;
            corrGlobaliEntity.VAR_NOME = aggregate.Nome;
            corrGlobaliEntity.CHA_TIPO_IE = "E";
            corrGlobaliEntity.CHA_TIPO_URP = "P";
            corrGlobaliEntity.CHA_TIPO_CORR = aggregate.RubricaComune != null && (bool)aggregate.RubricaComune ? "C" : "S";
            corrGlobaliEntity.ID_OLD = 0;
            corrGlobaliEntity.ID_UO = 0;
            corrGlobaliEntity.CHA_DETTAGLI = "1";
            corrGlobaliEntity.VAR_EMAIL = string.Empty;
            corrGlobaliEntity.VAR_CODICE_AMM = aggregate.CodiceAmministrazione;
            corrGlobaliEntity.VAR_CODICE_AOO = aggregate.CodiceAOO;
            corrGlobaliEntity.CHA_PA = "1";
            corrGlobaliEntity.VAR_CHIAVE_AE = "0";
            corrGlobaliEntity.CHA_SYSTEM_ROLE = "0";

            var idCanalePreferenziale = string.Empty;
            if (aggregate.CanalePreferenziale != null && !string.IsNullOrEmpty(aggregate.CanalePreferenziale.Id))
            {
                idCanalePreferenziale = aggregate.CanalePreferenziale.Id;
                if (!await _dbContext.DocumentTypesEntities.AnyAsync(d => d.SYSTEM_ID == idCanalePreferenziale.AsLong()))
                    throw new CanalePreferenzialeNotFoundPi3Exception(@event.Id);
            }
            else
            {
                var canaleCorrLetteraEntity = await _dbContext.DocumentTypesEntities.FirstOrDefaultAsync(d => d.TYPE_ID == "LETTERA");
                idCanalePreferenziale = canaleCorrLetteraEntity.SYSTEM_ID.ToString();
            }

            var canaleCorrEntity = new CanaleCorrEntity();
            await _dbContext.CanaleCorrEntities.AddAsync(canaleCorrEntity);

            canaleCorrEntity.ID_CORR_GLOBALE = aggregate.Id.AsLong();
            canaleCorrEntity.CHA_PREFERITO = "1";
            canaleCorrEntity.ID_DOCUMENTTYPE = idCanalePreferenziale.AsLong();


            var dettGlobaliEntity = new DettGlobaliEntity();
            await _dbContext.DettGlobaliEntities.AddAsync(dettGlobaliEntity);
            dettGlobaliEntity.ID_CORR_GLOBALI = corrGlobaliEntity.SYSTEM_ID;
            if (aggregate.Indirizzo != null)
            {
                dettGlobaliEntity.VAR_INDIRIZZO = aggregate.Indirizzo.Indirizzo;
                dettGlobaliEntity.VAR_CAP = aggregate.Indirizzo.Cap;
                dettGlobaliEntity.VAR_CITTA = aggregate.Indirizzo.Citta;
                dettGlobaliEntity.VAR_PROVINCIA = aggregate.Indirizzo.Provincia;
                dettGlobaliEntity.VAR_NAZIONE = aggregate.Indirizzo.Nazione;
                dettGlobaliEntity.VAR_LOCALITA = aggregate.Indirizzo.Localita;
                dettGlobaliEntity.VAR_FAX = aggregate.Indirizzo.Fax;
                dettGlobaliEntity.VAR_TELEFONO = aggregate.Indirizzo.TelefonoPrincipale;
                dettGlobaliEntity.VAR_TELEFONO2 = aggregate.Indirizzo.TelefonoSecondario;
            }
            else
            {
                dettGlobaliEntity.VAR_INDIRIZZO = string.Empty;
                dettGlobaliEntity.VAR_CAP = string.Empty;
                dettGlobaliEntity.VAR_CITTA = string.Empty;
                dettGlobaliEntity.VAR_PROVINCIA = string.Empty;
                dettGlobaliEntity.VAR_LOCALITA = string.Empty;
                dettGlobaliEntity.VAR_NAZIONE = string.Empty;
                dettGlobaliEntity.VAR_FAX = string.Empty;
                dettGlobaliEntity.VAR_TELEFONO = string.Empty;
                dettGlobaliEntity.VAR_TELEFONO2 = string.Empty;
            }
            dettGlobaliEntity.VAR_NOTE = aggregate.Note;
            dettGlobaliEntity.VAR_LUOGO_NASCITA = aggregate.LuogoNascita;
            dettGlobaliEntity.DTA_NASCITA = aggregate.DataNascita.AsDateFormat();
            dettGlobaliEntity.VAR_TITOLO = aggregate.Titolo;
            dettGlobaliEntity.VAR_COD_FISC = aggregate.CodiceFiscale;
            dettGlobaliEntity.VAR_COD_PI = aggregate.PartitaIva;

            if (aggregate.IdOld != null)
            {
                var listeDistrEntity = await _dbContext.ListeDistrEntities.Where(l => l.ID_DPA_CORR == aggregate.IdOld.AsLong()).ToListAsync();
                foreach (var l in listeDistrEntity)
                    l.ID_DPA_CORR = corrGlobaliEntity.SYSTEM_ID;
            }
        }

        protected virtual async Task Handle(CodiceSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_CODICE = @event.Codice != null ? @event.Codice.ToString() : null;
        }

        protected virtual async Task Handle(CodiceAmministrazioneSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_CODICE_AMM = @event.NewCodiceAmministrazione != null ? @event.NewCodiceAmministrazione.ToString() : null;
        }

        protected virtual async Task Handle(CodiceAOOSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_CODICE_AOO = @event.NewCodiceAOO != null ? @event.NewCodiceAOO.ToString() : null;
        }

        protected virtual async Task Handle(EmailPrincipaleSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());
            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

            var mailCorrEsterniEntity = await _dbContext.MailCorrEsterniEntities.FirstOrDefaultAsync(m => m.VAR_EMAIL.Equals(@event.IndirizzoEmail) && m.ID_CORR == aggregate.Id.AsLong());
            var mailCorrEsterniPrincipaleOldEntity = await _dbContext.MailCorrEsterniEntities.FirstOrDefaultAsync(m => m.ID_CORR == aggregate.Id.AsLong() && m.VAR_PRINCIPALE == "1");

            if (mailCorrEsterniEntity == null)
                throw new MailCorrispondenteNotFoundPi3Exception(@event.IndirizzoEmail);

            corrGlobaliEntity.VAR_EMAIL = mailCorrEsterniEntity.VAR_EMAIL;

            mailCorrEsterniEntity.VAR_PRINCIPALE = "1";
            if (mailCorrEsterniPrincipaleOldEntity != null)
                mailCorrEsterniPrincipaleOldEntity.VAR_PRINCIPALE = "0";
        }

        protected virtual async Task Handle(EmailAddedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

            if (await _dbContext.MailCorrEsterniEntities.AnyAsync(m => m.ID_CORR == aggregate.Id.AsLong() && m.VAR_EMAIL.ToUpper() == @event.IndirizzoEmail.ToUpper()))
                throw new MailCorrispondentePresentePi3Exception(@event.IndirizzoEmail);

            var mailCorrEsterniEntity = new MailCorrEsterniEntity();
            await _dbContext.MailCorrEsterniEntities.AddAsync(mailCorrEsterniEntity);

            mailCorrEsterniEntity.VAR_EMAIL = @event.IndirizzoEmail;
            mailCorrEsterniEntity.ID_CORR = aggregate.Id.AsLong();
            mailCorrEsterniEntity.VAR_NOTE = @event.Note.ToString();
            mailCorrEsterniEntity.VAR_PRINCIPALE = @event.Principale ? "1" : "0";

            if (@event.Principale)
                corrGlobaliEntity.VAR_EMAIL = mailCorrEsterniEntity.VAR_EMAIL;
        }

        protected virtual async Task Handle(EmailModifiedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

            var mailCorrEsterniEntity = await _dbContext.MailCorrEsterniEntities.FirstOrDefaultAsync(m => m.SYSTEM_ID == @event.IdEmail.AsLong());
            if (mailCorrEsterniEntity == null)
                throw new MailCorrispondenteNotFoundPi3Exception(@event.IndirizzoEmail);

            mailCorrEsterniEntity.VAR_EMAIL = @event.IndirizzoEmail;
            mailCorrEsterniEntity.VAR_NOTE = @event.Note.ToString();
        }

        protected virtual async Task Handle(EmailRemovedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(aggregate.Id);

            var mailCorrEsterniEntity = await _dbContext.MailCorrEsterniEntities.FirstOrDefaultAsync(m => m.SYSTEM_ID == @event.IdEmail.AsLong());
            if (mailCorrEsterniEntity == null)
                throw new MailCorrispondenteNotFoundPi3Exception(@event.IdEmail);

            _dbContext.MailCorrEsterniEntities.Remove(mailCorrEsterniEntity);
        }

        protected virtual async Task Handle(CanalePreferenzialeSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            if (!await _dbContext.DocumentTypesEntities.AnyAsync(d => d.SYSTEM_ID == @event.IdCanalePreferenziale.AsLong()))
                throw new CanalePreferenzialeNotFoundPi3Exception(@event.Id);

            var canaleCorrEntity = await _dbContext.CanaleCorrEntities.FirstOrDefaultAsync(c => c.ID_CORR_GLOBALE == aggregate.Id.AsLong());
            if (canaleCorrEntity != null)
                canaleCorrEntity.ID_DOCUMENTTYPE = @event.IdCanalePreferenziale.AsLong();
        }

        protected virtual async Task Handle(DataFineSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);
            
            corrGlobaliEntity.DTA_FINE = @event.NewDataFine;
        }

        protected virtual async Task Handle(RubricaComuneSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.CHA_TIPO_CORR = aggregate.RubricaComune.HasValue && aggregate.RubricaComune.Value ? "C" : "S";
        }

        protected virtual async Task Handle(RubricaEsternaSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.RUBRICA_ESTERNA = aggregate.RubricaEsterna;
        }

        protected virtual async Task Handle(IdOldSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.ID_OLD = @event.IdOld != null ? @event.IdOld.AsLong() : null;
        }

        protected virtual async Task Handle(DescriptionOldSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_DESC_CORR_OLD = @event.DescriptionOld;
        }

        protected virtual async Task Handle(InteropUrlSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.INTEROPURL = @event.InteropUrl;
        }

        protected virtual async Task Handle(IndirizzoChangedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
            {
                dettGlobaliEntity.VAR_INDIRIZZO = @event.NewIndirizzo.Indirizzo;
                dettGlobaliEntity.VAR_PROVINCIA = @event.NewIndirizzo.Provincia;
                dettGlobaliEntity.VAR_CAP = @event.NewIndirizzo.Cap;
                dettGlobaliEntity.VAR_NAZIONE = @event.NewIndirizzo.Nazione;
                dettGlobaliEntity.VAR_CITTA = @event.NewIndirizzo.Citta;
                dettGlobaliEntity.VAR_LOCALITA = @event.NewIndirizzo.Localita;
                dettGlobaliEntity.VAR_TELEFONO = @event.NewIndirizzo.TelefonoPrincipale;
                dettGlobaliEntity.VAR_TELEFONO2 = @event.NewIndirizzo.TelefonoSecondario;
            }
        }

        protected virtual async Task Handle(CodiceFiscaleSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
                dettGlobaliEntity.VAR_COD_FISC = @event.NewCodiceFiscale;
        }

        protected virtual async Task Handle(PartitaIvaSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
                dettGlobaliEntity.VAR_COD_PI = @event.NewPartitaIva;
        }

        protected virtual async Task Handle(NoteSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
                dettGlobaliEntity.VAR_NOTE = @event.NewNote;
        }

        protected virtual async Task Handle(NomeChangedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_NOME = @event.NewNome;
        }

        protected virtual async Task Handle(CognomeChangedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            corrGlobaliEntity.VAR_COGNOME = @event.NewCognome;
        }

        protected virtual async Task Handle(LuogoNascitaSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
                dettGlobaliEntity.VAR_LUOGO_NASCITA = @event.NewLuogoNascita;
        }

        protected virtual async Task Handle(DataNascitaSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
                dettGlobaliEntity.DTA_NASCITA = @event.NewDataNascita != null && @event.NewDataNascita != DateTime.MinValue 
                                                ? @event.NewDataNascita.AsDateFormat() : null;
        }

        protected virtual async Task Handle(TitoloSettedEvent @event, PersonaCorrispondente aggregate)
        {
            var corrGlobaliEntity = await _dbContext.CorrGlobaliEntities.FindAsync(aggregate.Id.AsLong());

            if (corrGlobaliEntity == null)
                throw new PersonaCorrispondenteNotFoundPi3Exception(@event.Id);

            var dettGlobaliEntity = await _dbContext.DettGlobaliEntities.FirstOrDefaultAsync(d => d.ID_CORR_GLOBALI == aggregate.Id.AsLong());
            if (dettGlobaliEntity != null)
            {
                dettGlobaliEntity.VAR_TITOLO = @event.Titolo;
                corrGlobaliEntity.VAR_DESC_CORR = 
                    !string.IsNullOrEmpty(@event.Titolo) ? 
                        string.Format("{0} {1} {2}", @event.Titolo, aggregate.Cognome, aggregate.Nome) : 
                        string.Format("{0} {1}", aggregate.Cognome, aggregate.Nome);
            }
        }

        #endregion
    }
}
