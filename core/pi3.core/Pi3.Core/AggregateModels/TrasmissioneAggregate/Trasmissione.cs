// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public class Trasmissione : Element
    {
        #region Public Members

        public Trasmissione(string idTenant,
                  DateTime creationDate,                  
                  string idOggettoTrasmesso,
                  TipiOggettiTrasmessiEnum tipoOggettoTrasmesso,
                  Autore? autore = null,
                  TextValue? noteGenerali = null)
        {
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            idOggettoTrasmesso = idOggettoTrasmesso ?? throw new ArgumentNullException(nameof(idOggettoTrasmesso));

            if (autore != null)
                Validator.ValidateObject(autore, new ValidationContext(autore), true);

            this.ApplyChange(new TrasmissioneCreataEvent()
            {
                Id = null,
                TypeName = "Trasmissione",
                IdTenant = idTenant,
                CreationDate = creationDate,
                Name = new TextValue(string.Format(Resources.Nome, tipoOggettoTrasmesso, idOggettoTrasmesso)),
                Autore = autore,
                IdOggettoTrasmesso = idOggettoTrasmesso,
                TipoOggettoTrasmesso = tipoOggettoTrasmesso,
                NoteGenerali = noteGenerali
            });
        }

        public Trasmissione(string id, 
                    string idTenant,
                    DateTime creationDate,                    
                    string idOggettoTrasmesso, 
                    TipiOggettiTrasmessiEnum tipoOggettoTrasmesso,
                    Autore? autore = null,
                    TextValue? noteGenerali = null) 
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            idTenant = idTenant ?? throw new ArgumentNullException(nameof(idTenant));
            idOggettoTrasmesso = idOggettoTrasmesso ?? throw new ArgumentNullException(nameof(idOggettoTrasmesso));

            if (autore != null)
                Validator.ValidateObject(autore, new ValidationContext(autore), true);

            this.ApplyChange(new TrasmissioneCreataEvent()
            {
                Id = id,
                TypeName = "Trasmissione",
                IdTenant = idTenant,
                CreationDate = creationDate,
                Name = new TextValue(string.Format(Resources.Nome, tipoOggettoTrasmesso, idOggettoTrasmesso)),
                Autore = autore,
                IdOggettoTrasmesso = idOggettoTrasmesso,
                TipoOggettoTrasmesso = tipoOggettoTrasmesso,
                NoteGenerali = noteGenerali
            });
        }

        public Autore? Autore { get; protected set; }

        public OggettoTrasmesso OggettoTrasmesso { get; protected set; }

        public virtual void ChangeNoteGenerali(TextValue? newNoteGenerali)
        {
            this.ApplyChange(new NoteGeneraliChangedEvent()
            {
                Id = this.Id,
                NewNoteGenerali = newNoteGenerali
            });
        }

        public TextValue? NoteGenerali { get; protected set; }

        public bool? CediDiritti { get; protected set; }

        public virtual void ChangeCediDiritti(bool newCediDiritti)
        {
            this.AssertTrasmissioneInviata();

            this.ApplyChange(new CediDirittiChangedEvent()
            {
                Id = this.Id,
                NewCediDiritti = newCediDiritti
            });
        }

        public virtual void Invia(DateTime? dataInvio = null)
        {
            this.AssertTrasmissioneInviata();

            this.ApplyChange(new TrasmissioneInviataEvent()
            {
                Id = this.Id,
                DataInvio = dataInvio ?? DateTime.Now
            });
        }

        public DateTime? DataInvio { get; set; } = null;

        public virtual void PrepareTrasmissioneSingolaGruppo(DatiTrasmissioneSingolaGruppo datiTrasmissioneSingola)
        {
            this.AssertTrasmissioneInviata();

            datiTrasmissioneSingola = datiTrasmissioneSingola ?? throw new ArgumentNullException(nameof(datiTrasmissioneSingola));

            Validator.ValidateObject(datiTrasmissioneSingola, new ValidationContext(datiTrasmissioneSingola), true);

            this.ApplyChange(new TrasmissioneSingolaGruppoPreparedEvent()
            {
                Id = this.Id,
                DatiTrasmissioneSingola = datiTrasmissioneSingola
            });
        }

        public virtual void PrepareTrasmissioneSingolaUtente(DatiTrasmissioneSingolaUtente datiTrasmissioneSingola)
        {
            this.AssertTrasmissioneInviata();

            datiTrasmissioneSingola = datiTrasmissioneSingola ?? throw new ArgumentNullException(nameof(datiTrasmissioneSingola));

            Validator.ValidateObject(datiTrasmissioneSingola, new ValidationContext(datiTrasmissioneSingola), true);

            this.ApplyChange(new TrasmissioneSingolaUtentePreparedEvent()
            {
                Id = this.Id,
                DatiTrasmissioneSingola = datiTrasmissioneSingola
            });
        }

        public virtual void AddTrasmissioneSingolaGruppo(
            string id, 
            string idRagioneTrasmissione, string? nomeRagioneTrasmissione, bool? ragioneConWorkflow,
            TipiTrasmissioneSingolaEnum tipo, 
            string idGruppoDestinatario, string? codiceGruppoDestinatario, TextValue? descrizioneGruppoDestinatario,
            TextValue? note = null,
            DateTime? dataScadenza = null,
            bool? nascondiVersioniPrecedenti = null, 
            CessioneDirittiRagione? cessioneDirittiRagione = null)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));
            idRagioneTrasmissione = idRagioneTrasmissione ?? throw new ArgumentNullException(nameof(idRagioneTrasmissione));
            idGruppoDestinatario = idGruppoDestinatario ?? throw new ArgumentNullException(nameof(idGruppoDestinatario));

            this.ApplyChange(new TrasmissioneSingolaGruppoAddedEvent()
            {
                Id = this.Id,
                IdTrasmissioneSingola = id,
                IdRagioneTrasmissione = idRagioneTrasmissione,
                NomeRagioneTrasmissione = nomeRagioneTrasmissione,
                RagioneConWorkflow = ragioneConWorkflow,
                CessioneDirittiRagione = cessioneDirittiRagione,
                Tipo = tipo,
                IdGruppoDestinatario = idGruppoDestinatario,
                CodiceGruppoDestinatario = codiceGruppoDestinatario,
                DescrizioneGruppoDestinatario = descrizioneGruppoDestinatario,
                Note = note,
                DataScadenza = dataScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public virtual void AddTrasmissioneSingolaUtente(
        string id,
            string idRagioneTrasmissione, string? nomeRagioneTrasmissione, bool? ragioneConWorkflow,
            string idUtenteDestinatario, string? userIdDestinatario, string? cognomeDestinatario, string? nomeDestinatario,
            TextValue? note = null,
            DateTime? dataScadenza = null,
            bool? nascondiVersioniPrecedenti = null, 
            CessioneDirittiRagione? cessioneDirittiRagione = null)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));
            idRagioneTrasmissione = idRagioneTrasmissione ?? throw new ArgumentNullException(nameof(idRagioneTrasmissione));
            idUtenteDestinatario = idUtenteDestinatario ?? throw new ArgumentNullException(nameof(idUtenteDestinatario));

            this.ApplyChange(new TrasmissioneSingolaUtenteAddedEvent()
            {
                Id = this.Id,
                IdTrasmissioneSingola = id,
                IdRagioneTrasmissione = idRagioneTrasmissione,
                NomeRagioneTrasmissione = nomeRagioneTrasmissione,
                RagioneConWorkflow = ragioneConWorkflow,
                CessioneDirittiRagione = cessioneDirittiRagione,
                IdUtenteDestinatario = idUtenteDestinatario,
                UserIdDestinatario = userIdDestinatario,
                CognomeDestinatario = cognomeDestinatario,
                NomeDestinatario = nomeDestinatario,
                Note = note,
                DataScadenza = dataScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public IReadOnlyList<TrasmissioneSingola> TrasmissioniSingole
        {
            get
            {
                return _trasmissioniSingole.AsReadOnly();
            }
        }

        public virtual void RemoveTrasmissioneSingola(string id)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._trasmissioniSingole.Any(ts => ts.Id == id))
                throw new TrasmissioneSingolaNotFoundPi3Exception(id);

            this.ApplyChange(new TrasmissioneSingolaRemovedEvent()
            {
                Id = this.Id,
                IdTrasmissioneSingola = id
            });
        }

        public virtual void RemoveTrasmissioneUtente(string id)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._trasmissioniSingole.Any(ts => ts.TrasmissioniUtente.Any(tu => tu.Id == id)))
                throw new TrasmissioneUtenteNotFoundPi3Exception(id);

            this.ApplyChange(new TrasmissioneUtenteRemovedEvent()
            {
                Id = this.Id,
                IdTrasmissioneUtente = id
            });
        }

        public virtual void AddTrasmissioneUtente(
            string idTrasmissioneSingola,
            string idTrasmissioneUtente,
            string idUtente,
            string? userId,
            string? cognome,
            string? nome,
            DateTime? dataRimozioneCentroNotifiche)
        {
            idTrasmissioneSingola = idTrasmissioneSingola ?? throw new ArgumentNullException(nameof(idTrasmissioneSingola));
            idTrasmissioneUtente = idTrasmissioneUtente ?? throw new ArgumentNullException(nameof(idTrasmissioneUtente));

            if (!this._trasmissioniSingole.Any(ts => ts.Id == idTrasmissioneSingola))
                throw new TrasmissioneSingolaNotFoundPi3Exception(idTrasmissioneSingola);
            
            if (this._trasmissioniSingole.Any(ts => ts.TrasmissioniUtente.Any(tu => tu.Id == idTrasmissioneUtente)))
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaPresente, ErrorDescriptions.ResourceManager, idTrasmissioneUtente);

            this.ApplyChange(new TrasmissioneUtenteAddedEvent()
            {
                 Id = this.Id,
                 IdTrasmissioneSingola = idTrasmissioneSingola,
                 IdTrasmissioneUtente = idTrasmissioneUtente,
                 IdUtente = idUtente,
                 UserId = userId,
                 Cognome = cognome,
                 Nome = nome,
                 DataRimozioneCentroNotifiche = dataRimozioneCentroNotifiche
            });
        }

        public virtual void Accetta(string idUtente, Accetta accetta)
        {
            idUtente = idUtente ?? throw new ArgumentNullException(nameof(idUtente));
            accetta = accetta ?? throw new ArgumentNullException(nameof(accetta));

            foreach (var ts in this._trasmissioniSingole.Where(ts => ts.RagioneTrasmissione.ConWorkflow ?? false))
            {
                foreach (var tu in ts.TrasmissioniUtente.Where(tu => tu.UtenteDestinatario.Id == idUtente && !tu.DataAccettazione.HasValue && !tu.DataRifiuto.HasValue))
                {
                    this.ApplyChange(new TrasmissioneUtenteAccettataEvent()
                    {
                        Id = this.Id,
                        IdTrasmissioneUtente = tu.Id,
                        Accetta = accetta
                    });
                }
            }
        }

        public virtual void Rifiuta(string idUtente, Rifiuta rifiuta)
        {
            idUtente = idUtente ?? throw new ArgumentNullException(nameof(idUtente));
            rifiuta = rifiuta ?? throw new ArgumentNullException(nameof(rifiuta));

            foreach (var ts in this._trasmissioniSingole.Where(ts => ts.RagioneTrasmissione.ConWorkflow ?? false))
            {
                foreach (var tu in ts.TrasmissioniUtente.Where(tu => tu.UtenteDestinatario.Id == idUtente && !tu.DataAccettazione.HasValue && !tu.DataRifiuto.HasValue))
                {
                    this.ApplyChange(new TrasmissioneUtenteRifiutataEvent()
                    {
                        Id = this.Id,
                        IdTrasmissioneUtente = tu.Id,
                        Rifiuta = rifiuta
                    });
                }
            }
        }

        public virtual void RifiutaTrasmissioneUtente(string id, Rifiuta rifiuta)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            rifiuta = rifiuta ?? throw new ArgumentNullException(nameof(rifiuta));

            if (!this._trasmissioniSingole.Any(ts => ts.TrasmissioniUtente.Any(tu => tu.Id == id)))
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteNonTrovata, ErrorDescriptions.ResourceManager, id);

            this.ApplyChange(new TrasmissioneUtenteRifiutataEvent()
            {
                Id = this.Id,
                IdTrasmissioneUtente = id,
                Rifiuta = rifiuta
            });
        }

        public virtual void Visto(string idUtente, Visto visto)
        {
            idUtente = idUtente ?? throw new ArgumentNullException(nameof(idUtente));
            visto = visto ?? throw new ArgumentNullException(nameof(visto));

            foreach (var ts in this._trasmissioniSingole.Where(ts => !(ts.RagioneTrasmissione.ConWorkflow ?? false)))
            {
                foreach (var tu in ts.TrasmissioniUtente.Where(tu => tu.UtenteDestinatario.Id == idUtente))
                {
                    this.ApplyChange(new TrasmissioneUtenteVistaEvent()
                    {
                        Id = this.Id,
                        IdTrasmissioneUtente = tu.Id,
                        Visto = visto
                    });
                }
            }
        }

        public virtual void ChangeTipoTrasmissioneSingola(string id, TipiTrasmissioneSingolaEnum newTipo)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));
            
            if (!this._trasmissioniSingole.Any(ts => ts.Id == id))
                throw new TrasmissioneSingolaNotFoundPi3Exception(id);

            this.ApplyChange(new TipoTrasmissioneSingolaChangedEvent()
            {
                Id = this.Id,
                NewTipo = newTipo
            });
        }

        public virtual void ChangeNoteTrasmissioneSingola(string id, TextValue? newNote)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._trasmissioniSingole.Any(ts => ts.Id == id))
                throw new TrasmissioneSingolaNotFoundPi3Exception(id);

            this.ApplyChange(new NoteTrasmissioneSingolaChangedEvent()
            {
                Id = this.Id,
                NewNote = newNote
            });
        }

        public virtual void ChangeDataScadenzaTrasmissioneSingola(string id, DateTime? newDataScadenza)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._trasmissioniSingole.Any(ts => ts.Id == id))
                throw new TrasmissioneSingolaNotFoundPi3Exception(id);

            this.ApplyChange(new DataScadenzaTrasmissioneSingolaChangedEvent()
            {
                Id = this.Id,
                NewDataScadenza = newDataScadenza
            });
        }

        public virtual void ChangeNascondiVersioniPrecedentiTrasmissioneSingola(string id, bool? newNascondiVersioniPrecedenti)
        {
            this.AssertTrasmissioneInviata();

            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._trasmissioniSingole.Any(ts => ts.Id == id))
                throw new TrasmissioneSingolaNotFoundPi3Exception(id);

            this.ApplyChange(new TrasmissioneSingolaNascondiVersioniPrecedentiChangedEvent()
            {
                Id = this.Id,
                NewNascondiVersioniPrecedenti = newNascondiVersioniPrecedenti
            });
        }

        public override bool IsValid()
        {
            return !GetErrors().Any();
        }

        public virtual IEnumerable<Pi3Exception> GetErrors()
        {
            List<Pi3Exception> pi3Exceptions = new List<Pi3Exception>();

            if (this.TrasmissioniSingole.Count == 0
                && !this.GetUncommittedChanges().Any(c => c.GetType() == typeof(TrasmissioneSingolaGruppoPreparedEvent))
                && !this.GetUncommittedChanges().Any(c => c.GetType() == typeof(TrasmissioneSingolaUtentePreparedEvent))
                )
            {
                pi3Exceptions.Add(new TrasmissioneSingolaNonAssegnataPi3Exception());
            }                

            if(this.TrasmissioniSingole.Count > 0 && this.TrasmissioniSingole.Any(s => !s.TrasmissioniUtente.Any()))
            {
                pi3Exceptions.Add(new TrasmissioneUtenteNonAssegnataPi3Exception());
            }

            if (this.GetUncommittedChanges().Any(c => c.GetType() == typeof(TrasmissioneSingolaGruppoPreparedEvent) 
                && (c as TrasmissioneSingolaGruppoPreparedEvent).DatiTrasmissioneSingola.UtentiNotificati.Count == 0))
            {
                pi3Exceptions.Add(new TrasmissioneUtenteNonAssegnataPi3Exception());
            }

            return pi3Exceptions;
        }

        #endregion

        #region Private Members

        protected List<TrasmissioneSingola> _trasmissioniSingole;

        protected virtual void AssertTrasmissioneInviata()
        {
            if (this.DataInvio.HasValue)
                throw new TrasmissionePi3Exception(ErrorDescriptions.TrasmissioneGiaInviata, ErrorDescriptions.ResourceManager, this.Id);
        }

        protected TrasmissioneUtente FindTrasmissioneUtente(string id)
        {
            TrasmissioneUtente tu = null;

            foreach (var ts in this._trasmissioniSingole)
            {
                tu = ts.TrasmissioniUtente.FirstOrDefault(tu => tu.Id == id);

                if (tu != null)
                    break;
            }

            if (tu == null)
                throw new TrasmissioneUtenteNotFoundPi3Exception(id);

            return tu;
        }

        protected virtual void Handle(TrasmissioneCreataEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this.Autore = @event.Autore;
            this.OggettoTrasmesso = new OggettoTrasmesso(@event.IdOggettoTrasmesso, @event.TipoOggettoTrasmesso);
            this._trasmissioniSingole = new List<TrasmissioneSingola>();
            this.NoteGenerali = @event.NoteGenerali;            
        }

        protected virtual void Handle(NoteGeneraliChangedEvent @event)
        {
            this.NoteGenerali = @event.NewNoteGenerali;
        }

        protected virtual void Handle(CediDirittiChangedEvent @event)
        {
            this.CediDiritti = @event.NewCediDiritti;
        }

        protected virtual void Handle(TrasmissioneInviataEvent @event)
        {
            this.DataInvio = @event.DataInvio;
        }

        protected virtual void Handle(TrasmissioneSingolaGruppoPreparedEvent @event)
        {
        }

        protected virtual void Handle(TrasmissioneSingolaUtentePreparedEvent @event)
        {
        }

        protected virtual void Handle(TrasmissioneSingolaGruppoAddedEvent @event)
        {
            if(!this.DataInvio.HasValue && @event.CessioneDirittiRagione != null && !@event.CessioneDirittiRagione.ConSceltaUtente)
                this.CediDiritti = true;

            this._trasmissioniSingole.Add(new TrasmissioneSingolaGruppo(@event.IdTrasmissioneSingola,                    
                    new RagioneTrasmissione(@event.IdRagioneTrasmissione, @event.NomeRagioneTrasmissione, @event.RagioneConWorkflow, @event.CessioneDirittiRagione),
                    @event.Tipo,
                    new GruppoDestinatario(@event.IdGruppoDestinatario, @event.CodiceGruppoDestinatario, @event.DescrizioneGruppoDestinatario),
                    @event.Note,
                    @event.DataScadenza,
                    @event.NascondiVersioniPrecedenti));
        }

        protected virtual void Handle(TrasmissioneSingolaUtenteAddedEvent @event)
        {
            if (!this.DataInvio.HasValue && @event.CessioneDirittiRagione != null && !@event.CessioneDirittiRagione.ConSceltaUtente)
                this.CediDiritti = true;

            this._trasmissioniSingole.Add(new TrasmissioneSingolaUtente(@event.IdTrasmissioneSingola,
                    new RagioneTrasmissione(@event.IdRagioneTrasmissione, @event.NomeRagioneTrasmissione, @event.RagioneConWorkflow, @event.CessioneDirittiRagione),
                    new UtenteDestinatario(@event.IdUtenteDestinatario, @event.UserIdDestinatario, @event.CognomeDestinatario, @event.NomeDestinatario),
                    @event.Note,
                    @event.DataScadenza,
                    @event.NascondiVersioniPrecedenti));
        }

        protected virtual void Handle(TrasmissioneSingolaRemovedEvent @event)
        {
            this._trasmissioniSingole.Remove(this._trasmissioniSingole.First(ts => ts.Id == @event.IdTrasmissioneSingola));
        }

        protected virtual void Handle(TrasmissioneUtenteRemovedEvent @event)
        {
            var tu = this.FindTrasmissioneUtente(@event.IdTrasmissioneUtente);

            tu.Parent.RemoveTrasmissioneUtente(tu);
        }

        protected virtual void Handle(TrasmissioneUtenteAddedEvent @event)
        {
            var ts = this._trasmissioniSingole
                .First(ts => ts.Id == @event.IdTrasmissioneSingola);

            ts.AddTrasmissioneUtente(
                new TrasmissioneUtente(
                    ts, @event.IdTrasmissioneUtente, 
                    new UtenteDestinatario(@event.IdUtente, @event.UserId, @event.Cognome, @event.Nome)));
        }

        protected virtual void Handle(TrasmissioneUtenteAccettataEvent @event)
        {
            var tu = this.FindTrasmissioneUtente(@event.IdTrasmissioneUtente);

            tu.Do(@event.Accetta);
        }

        protected virtual void Handle(TrasmissioneUtenteRifiutataEvent @event)
        {
            var tu = this.FindTrasmissioneUtente(@event.IdTrasmissioneUtente);

            tu.Do(@event.Rifiuta);
        }

        protected virtual void Handle(TrasmissioneUtenteVistaEvent @event)
        {
            var tu = this.FindTrasmissioneUtente(@event.IdTrasmissioneUtente);

            //tu.Do(@event.Visto);
        }

        protected virtual void Handle(TipoTrasmissioneSingolaChangedEvent @event)
        {
            var ts = this._trasmissioniSingole.First(ts => ts.Id == @event.IdTrasmissioneSingola);

            ts.ChangeTipo(@event.NewTipo);
        }

        protected virtual void Handle(NoteTrasmissioneSingolaChangedEvent @event)
        {
            var ts = this._trasmissioniSingole.First(ts => ts.Id == @event.IdTrasmissioneSingola);

            ts.ChangeNote(@event.NewNote);
        }

        protected virtual void Handle(DataScadenzaTrasmissioneSingolaChangedEvent @event)
        {
            var ts = this._trasmissioniSingole.First(ts => ts.Id == @event.IdTrasmissioneSingola);

            ts.ChangeDataScadenza(@event.NewDataScadenza);
        }

        protected virtual void Handle(TrasmissioneSingolaNascondiVersioniPrecedentiChangedEvent @event)
        {
            var ts = this._trasmissioniSingole.First(ts => ts.Id == @event.IdTrasmissioneSingola);

            ts.ChangeNascondiVersioniPrecedenti(@event.NewNascondiVersioniPrecedenti);
        }

        #endregion
    }
}
