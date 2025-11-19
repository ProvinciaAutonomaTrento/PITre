// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate
{

    public class ModelloTrasmissione : Element
    {
        #region Public Members

        protected ModelloTrasmissione() : base()
        { }

        public ModelloTrasmissione(
            string idTenant, DateTime creationDate, 
            TextValue name, TextValue? noteGenerali,
            string idRegistro, string? codiceRegistro, TextValue? descrizioneRegistro,
            TipiOggettiTrasmessiEnum tipoOggettoTrasmesso)
            : this(null, idTenant, creationDate, name, noteGenerali, idRegistro, codiceRegistro, descrizioneRegistro, tipoOggettoTrasmesso)
        {
        }

        public ModelloTrasmissione(
                string id,
                string idTenant, DateTime creationDate,
                TextValue name, TextValue? noteGenerali,
                string idRegistro, string? codiceRegistro, TextValue? descrizioneRegistro,
                TipiOggettiTrasmessiEnum tipoOggettoTrasmesso)
        {
            this.ApplyChange(new ModelloTrasmissioneCreatoEvent()
            {
                Id = id,
                TypeName = "ModelloTrasmissione",
                IdTenant = idTenant,
                CreationDate = creationDate,
                Name = name,
                NoteGenerali = noteGenerali,
                IdRegistro = idRegistro,
                CodiceRegistro = codiceRegistro,
                DescrizioneRegistro = descrizioneRegistro,
                TipoOggettoTrasmesso = tipoOggettoTrasmesso
            });
        }

        public Autore Autore { get; protected set; }

        public void AssignAutorePersona(string id, string? userId = null, string? cognome = null, string? nome = null)
        {
            this.ApplyChange(new AutorePersonaAssignedEvent()
            {
                Id = this.Id,
                IdUser = id,
                UserId = userId,
                Cognome = cognome,
                Nome = nome,
            });
        }

        public void AssignAutoreGruppo(string id, string? codice = null, TextValue? descrizione = null)
        {
            this.ApplyChange(new AutoreGruppoAssignedEvent()
            {
                Id = this.Id,
                IdGruppo = id,
                Codice = codice,
                Descrizione = descrizione
            });
        }

        public TextValue? NoteGenerali { get; protected set; }

        public void ChangeNoteGenerali(TextValue? newNoteGenerali)
        {
            this.ApplyChange(new NoteGeneraliChangedEvent()
            {
                Id = this.Id,
                NewNoteGenerali = newNoteGenerali
            });
        }

        public Registro Registro { get; protected set; }

        public void ChangeRegistro(string id, string? codice, TextValue? descrizione)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            this.ApplyChange(new RegistroChangedEvent()
            {
                Id = this.Id,
                CodiceRegistro = codice,
                DescrizioneRegistro = descrizione
            });
        }

        public TipiOggettiTrasmessiEnum TipoOggettoTrasmesso { get; protected set; }

        public virtual void ChangeTipoOggettoTrasmesso(TipiOggettiTrasmessiEnum newTipoOggettoTrasmesso)
        {
            this.ApplyChange(new TipoOggettoTrasmessoChangedEvent()
            {
                Id = this.Id,
                NewTipoOggettoTrasmesso = newTipoOggettoTrasmesso
            });
        }

        public IReadOnlyList<DestinatarioModelloTrasmissione> Destinatari
        {
            get
            {
                var destinatari = new List<DestinatarioModelloTrasmissione>();
                destinatari.AddRange(this._gruppiDestinatari);
                destinatari.AddRange(this._utentiDestinatari);
                destinatari.AddRange(this._ufficiDestinatari);
                return destinatari.AsReadOnly();
            }
        }

        public override IEnumerable<Pi3Exception> GetErrors()
        {
            var errors = new List<Pi3Exception>();

            if (this.Autore == null)
                errors.Add(new NessunAutoreAssegnatoPi3Exception());

            if (!this._ufficiDestinatari.Any() && !this._gruppiDestinatari.Any() && !this._utentiDestinatari.Any())
                errors.Add(new NessunDestinatarioAssegnatoPi3Exception());

            if (this.Destinatari.Any(d => d.RagioneTrasmissione.CessioneDiritti != null) && this.CessioneDiritti == null)
                errors.Add(new NessunaCessioneDirittiAssegnataPi3Exception());

            return errors;
        }

        public CessioneDiritti CessioneDiritti { get; protected set; }

        public virtual void CediDiritti(CessioneDiritti cessioneDiritti)
        {
            var destinatario = this.Destinatari.FirstOrDefault(d => d.Id == cessioneDiritti.IdDestinatario);

            if (destinatario == null)
                throw new DestinatarioModelloTrasmissioneNotFoundPi3Exception(cessioneDiritti.IdDestinatario);

            if (destinatario.RagioneTrasmissione.CessioneDiritti == null)
                throw new RagioneTrasmissionePi3Exception(destinatario.RagioneTrasmissione.Id, ErrorDescriptions.RagioneTrasmissioneSenzaCessioneDiritti, ErrorDescriptions.ResourceManager);

            if (destinatario.GetType() == typeof(UfficioDestinatarioModelloTrasmissione))
            {
                throw new NotSupportedPi3Exception(ErrorDescriptions.CessioneDirittiNonSupportata, ErrorDescriptions.ResourceManager);
            }
            else if (destinatario.GetType() == typeof(GruppoDestinatarioModelloTrasmissione))
            {
                var gruppoDestinatario = ((GruppoDestinatarioModelloTrasmissione)destinatario);

                if (!gruppoDestinatario.UtentiNotificati.Any(u => u.IdUtente == cessioneDiritti.IdUtente))
                    throw new UtenteNotificatoNotFoundPi3Exception(cessioneDiritti.IdUtente);
            }

            this.ApplyChange(new CessioneDirittiEffettuataEvent()
            {
                Id = this.Id,
                CessioneDiritti = cessioneDiritti
            });
        }

        public virtual void AddGruppoDestinatario(
                    string id,
                    string? codiceGruppo,
                    TextValue? descrizioneGruppo,
                    string idRagioneTrasmissione,
                    string? nomeRagioneTrasmissione,
                    CessioneDirittiRagioneTrasmissione? cessioneDirittiRagioneTrasmissione,
                    TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
                    List<DatiUtenteNotificato> utentiNotificati,
                    TextValue? noteTrasmissioneSingola,
                    int? giorniScadenza,
                    bool? nascondiVersioniPrecedenti)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            idRagioneTrasmissione = idRagioneTrasmissione ?? throw new ArgumentNullException(nameof(idRagioneTrasmissione));
            utentiNotificati = utentiNotificati ?? throw new ArgumentNullException(nameof(utentiNotificati));

            if (this._gruppiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneAlreadyExistsPi3Exception(id);

            this.ApplyChange(new GruppoDestinatarioAdded()
            {
                Id = this.Id,
                IdGruppo = id,
                CodiceGruppo = codiceGruppo,
                DescrizioneGruppo = descrizioneGruppo,
                IdRagioneTrasmissione = idRagioneTrasmissione,
                NomeRagioneTrasmissione = nomeRagioneTrasmissione,
                CessioneDirittiRagioneTrasmissione = cessioneDirittiRagioneTrasmissione,
                TipoTrasmissioneSingola = tipoTrasmissioneSingola,
                UtentiNotificati = utentiNotificati,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public virtual void ChangeUtentiNotificati(string idGruppoDesinatario, List<DatiUtenteNotificato> utentiNotificati)
        {
            idGruppoDesinatario = idGruppoDesinatario ?? throw new ArgumentNullException(nameof(idGruppoDesinatario));
            utentiNotificati = utentiNotificati ?? throw new ArgumentNullException(nameof(utentiNotificati));

            if (!this._gruppiDestinatari.Any(d => d.Id == idGruppoDesinatario))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(idGruppoDesinatario);

            this.ApplyChange(new UtentiNotificatiChangedEvent()
            {
                Id = this.Id,
                IdGruppoDestinatario = idGruppoDesinatario,
                NewUtentiNotificati = utentiNotificati
            });
        }

        public virtual void RemoveGruppoDestinatario(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._gruppiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new GruppoDestinatarioRemoved()
            {
                Id = this.Id,
                IdGruppo = id
            });
        }

        public virtual void AddUtenteDestinatario(
            string id,
            string? userId,
            string? cognome,
            string? nome,
            string idRagioneTrasmissione,
            string? nomeRagioneTrasmissione,
            CessioneDirittiRagioneTrasmissione? cessioneDirittiRagioneTrasmissione,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            idRagioneTrasmissione = idRagioneTrasmissione ?? throw new ArgumentNullException(nameof(idRagioneTrasmissione));

            if (this._utentiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneAlreadyExistsPi3Exception(id);

            this.ApplyChange(new UtenteDestinatarioAdded()
            {
                Id = this.Id,
                IdUtente = id,
                UserId = userId,
                Cognome = cognome,
                Nome = nome,
                IdRagioneTrasmissione = idRagioneTrasmissione,
                NomeRagioneTrasmissione = nomeRagioneTrasmissione,
                CessioneDirittiRagioneTrasmissione = cessioneDirittiRagioneTrasmissione,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public virtual void RemoveUtenteDestinatario(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._utentiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new UtenteDestinatarioRemoved()
            {
                Id = this.Id,
                IdUtente = id
            });
        }

        public virtual void AddUfficioDestinatario(
                   string id,
                   string? codiceUfficio,
                   TextValue? descrizioneUfficio,
                   string idRagioneTrasmissione,
                   string? nomeRagioneTrasmissione,
                   TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
                   TextValue? noteTrasmissioneSingola,
                   int? giorniScadenza,
                   bool? nascondiVersioniPrecedenti)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            idRagioneTrasmissione = idRagioneTrasmissione ?? throw new ArgumentNullException(nameof(idRagioneTrasmissione));

            if (this._ufficiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneAlreadyExistsPi3Exception(id);

            this.ApplyChange(new UfficioDestinatarioAdded()
            {
                Id = this.Id,
                IdUfficio = id,
                CodiceUfficio = codiceUfficio,
                DescrizioneUfficio= descrizioneUfficio,
                IdRagioneTrasmissione = idRagioneTrasmissione,
                NomeRagioneTrasmissione = nomeRagioneTrasmissione,
                TipoTrasmissioneSingola = tipoTrasmissioneSingola,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public virtual void RemoveUfficioDestinatario(string id)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._ufficiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new UfficioDestinatarioRemoved()
            {
                Id = this.Id,
                IdUfficio = id
            });
        }

        public void ChangeOpzioniGruppoDestinatario(string id,
            TipiTrasmissioneSingolaEnum? tipoTrasmissioneSingola = null,
            TextValue? noteTrasmissioneSingola = null,
            int? giorniScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._gruppiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new OpzioniGruppoDestinatarioChangedEvent()
            {
                Id = this.Id,
                IdGruppoDestinatario = id,
                TipoTrasmissioneSingola = tipoTrasmissioneSingola,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public void ChangeOpzioniUtenteDestinatario(string id,
            TextValue? noteTrasmissioneSingola = null,
            int? giorniScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._utentiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new OpzioniUtenteDestinatarioChangedEvent()
            {
                Id = this.Id,
                IdUtenteDestinatario = id,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }

        public void ChangeOpzioniUfficioDestinatario(string id,
            TipiTrasmissioneSingolaEnum? tipoTrasmissioneSingola = null,
            TextValue? noteTrasmissioneSingola = null,
            int? giorniScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));

            if (!this._ufficiDestinatari.Any(d => d.Id == id))
                throw new DestintarioModelloTrasmissioneNotFoundPi3Exception(id);

            this.ApplyChange(new OpzioniUfficioDestinatarioChangedEvent()
            {
                Id = this.Id,
                IdUfficioDestinatario = id,
                TipoTrasmissioneSingola = tipoTrasmissioneSingola,
                NoteTrasmissioneSingola = noteTrasmissioneSingola,
                GiorniScadenza = giorniScadenza,
                NascondiVersioniPrecedenti = nascondiVersioniPrecedenti
            });
        }


        #endregion

        #region Private Members

        protected List<GruppoDestinatarioModelloTrasmissione> _gruppiDestinatari;
        protected List<UtenteDestinatarioModelloTrasmissione> _utentiDestinatari;
        protected List<UfficioDestinatarioModelloTrasmissione> _ufficiDestinatari;

        protected void Handle(ModelloTrasmissioneCreatoEvent @event)
        {
            base.Handle((ElementCreatedEvent)@event);

            this.Description = @event.Description;
            this.NoteGenerali = @event.NoteGenerali;
            this.Registro = new Registro(@event.Id, @event.CodiceRegistro, @event.DescrizioneRegistro);
            this.TipoOggettoTrasmesso = @event.TipoOggettoTrasmesso;
            this._gruppiDestinatari = new List<GruppoDestinatarioModelloTrasmissione>();
            this._utentiDestinatari = new List<UtenteDestinatarioModelloTrasmissione>();
            this._ufficiDestinatari = new List<UfficioDestinatarioModelloTrasmissione>();
        }

        protected override void Handle(ElementDescriptionChangedEvent @event)
        {
            base.Handle(@event);

            // Note generali come descrizione del documento
            this.NoteGenerali = @event.NewDescription;
        }

        protected virtual void Handle(NoteGeneraliChangedEvent @event)
        {
            this.Description = @event.NewNoteGenerali;
            this.NoteGenerali = @event.NewNoteGenerali;
        }

        protected virtual void Handle(RegistroChangedEvent @event)
        {
            this.Registro = new Registro(@event.Id, @event.CodiceRegistro, @event.DescrizioneRegistro);
        }

        protected virtual void Handle(GruppoDestinatarioAdded @event)
        {
            this._gruppiDestinatari.Add(new GruppoDestinatarioModelloTrasmissione(
                    @event.IdGruppo,
                    @event.CodiceGruppo,
                    @event.DescrizioneGruppo,
                    new RagioneTrasmissione(@event.IdRagioneTrasmissione, @event.NomeRagioneTrasmissione, @event.CessioneDirittiRagioneTrasmissione),
                    @event.TipoTrasmissioneSingola,
                    @event.UtentiNotificati,
                    @event.NoteTrasmissioneSingola,
                    @event.GiorniScadenza,
                    @event.NascondiVersioniPrecedenti));
        }

        protected virtual void Handle(GruppoDestinatarioRemoved @event)
        {
            this._gruppiDestinatari.RemoveAll(d => d.Id == @event.IdGruppo);

            if (this.CessioneDiritti != null && this.CessioneDiritti.IdDestinatario == @event.IdGruppo)
                this.CessioneDiritti = null;
        }

        protected virtual void Handle(UtenteDestinatarioAdded @event)
        {
            this._utentiDestinatari.Add(new UtenteDestinatarioModelloTrasmissione(
                @event.IdUtente,
                @event.UserId,
                @event.Cognome,
                @event.Nome,
                new RagioneTrasmissione(@event.IdRagioneTrasmissione, @event.NomeRagioneTrasmissione, @event.CessioneDirittiRagioneTrasmissione),
                @event.NoteTrasmissioneSingola,
                @event.GiorniScadenza,
                @event.NascondiVersioniPrecedenti));
        }

        protected virtual void Handle(UtenteDestinatarioRemoved @event)
        {
            this._utentiDestinatari.RemoveAll(d => d.Id == @event.IdUtente);

            if (this.CessioneDiritti != null && this.CessioneDiritti.IdDestinatario == @event.IdUtente)
                this.CessioneDiritti = null;
        }

        protected virtual void Handle(UfficioDestinatarioAdded @event)
        {
            this._ufficiDestinatari.Add(new UfficioDestinatarioModelloTrasmissione(
                @event.IdUfficio,
                @event.CodiceUfficio,
                @event.DescrizioneUfficio,                
                new RagioneTrasmissione(@event.IdRagioneTrasmissione, @event.NomeRagioneTrasmissione),
                @event.TipoTrasmissioneSingola,
                @event.NoteTrasmissioneSingola,
                @event.GiorniScadenza,
                @event.NascondiVersioniPrecedenti));
        }

        protected virtual void Handle(UfficioDestinatarioRemoved @event)
        {
            this._ufficiDestinatari.RemoveAll(d => d.Id == @event.IdUfficio);
        }

        protected virtual void Handle(TipoOggettoTrasmessoChangedEvent @event)
        {
            this.TipoOggettoTrasmesso = @event.NewTipoOggettoTrasmesso;
        }

        protected virtual void Handle(OpzioniGruppoDestinatarioChangedEvent @event)
        {
            var destinatario = this._gruppiDestinatari.First(d => d.Id == @event.IdGruppoDestinatario);

            if (@event.TipoTrasmissioneSingola.HasValue)
                destinatario.ChangeTipoTrasmissioneSingola(@event.TipoTrasmissioneSingola.Value);

            if (@event.NoteTrasmissioneSingola != null)
                destinatario.ChangeNoteTrasmissioneSingola(@event.NoteTrasmissioneSingola);

            if (@event.GiorniScadenza.HasValue)
                destinatario.ChangeGiorniScadenza(@event.GiorniScadenza);

            if (@event.NascondiVersioniPrecedenti.HasValue)
                destinatario.ChangeNascondiVersioniPrecedenti(@event.NascondiVersioniPrecedenti);
        }

        protected virtual void Handle(OpzioniUtenteDestinatarioChangedEvent @event)
        {
            var destinatario = this._utentiDestinatari.First(d => d.Id == @event.IdUtenteDestinatario);

            if (@event.NoteTrasmissioneSingola != null)
                destinatario.ChangeNoteTrasmissioneSingola(@event.NoteTrasmissioneSingola);

            if (@event.GiorniScadenza.HasValue)
                destinatario.ChangeGiorniScadenza(@event.GiorniScadenza);

            if (@event.NascondiVersioniPrecedenti.HasValue)
                destinatario.ChangeNascondiVersioniPrecedenti(@event.NascondiVersioniPrecedenti);
        }

        protected virtual void Handle(OpzioniUfficioDestinatarioChangedEvent @event)
        {
            var destinatario = this._ufficiDestinatari.First(d => d.Id == @event.IdUfficioDestinatario);

            if (@event.TipoTrasmissioneSingola.HasValue)
                destinatario.ChangeTipoTrasmissioneSingola(@event.TipoTrasmissioneSingola.Value);

            if (@event.NoteTrasmissioneSingola != null)
                destinatario.ChangeNoteTrasmissioneSingola(@event.NoteTrasmissioneSingola);

            if (@event.GiorniScadenza.HasValue)
                destinatario.ChangeGiorniScadenza(@event.GiorniScadenza);

            if (@event.NascondiVersioniPrecedenti.HasValue)
                destinatario.ChangeNascondiVersioniPrecedenti(@event.NascondiVersioniPrecedenti);
        }

        protected virtual void Handle(UtentiNotificatiChangedEvent @event)
        {
            var destinatario = this._gruppiDestinatari.First(d => d.Id == @event.IdGruppoDestinatario);

            destinatario.ChangeUtentiNotificati(@event.NewUtentiNotificati);
        }

        protected virtual void Handle(AutorePersonaAssignedEvent @event)
        {
            this.Autore = new AutorePersona(@event.IdUser, @event.UserId, @event.Cognome, @event.Nome);
        }

        protected virtual void Handle(AutoreGruppoAssignedEvent @event)
        {
            this.Autore = new AutoreGruppo(@event.IdGruppo, @event.Codice, @event.Descrizione);
        }

        protected virtual void Handle(CessioneDirittiEffettuataEvent @event)
        {
            this.CessioneDiritti = @event.CessioneDiritti;       
        }

        #endregion
    }
}