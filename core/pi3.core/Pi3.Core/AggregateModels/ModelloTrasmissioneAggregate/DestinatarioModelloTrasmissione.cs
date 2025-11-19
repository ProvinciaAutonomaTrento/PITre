// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Microsoft.Extensions.Logging;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate
{
    public class UtenteDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public UtenteDestinatarioModelloTrasmissione(string id,
            string? userId,
            string? cognome,
            string? nome,
            RagioneTrasmissione ragioneTrasmissione,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            this.UserId = userId;
            this.Cognome = cognome;
            this.Nome = nome;
        }

        public string? UserId { get; protected set; }

        public string? Cognome { get; protected set; }

        public string? Nome { get; protected set; }
    }

    public class GruppoDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public GruppoDestinatarioModelloTrasmissione(string id,
            string? codiceGruppo,
            TextValue? descrizioneGruppo,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
            IReadOnlyList<DatiUtenteNotificato> utentiNotificati,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            utentiNotificati = utentiNotificati ?? throw new ArgumentNullException(nameof(utentiNotificati));

            this.CodiceGruppo = codiceGruppo;
            this.DescrizioneGruppo = descrizioneGruppo;
            this.TipoTrasmissioneSingola = tipoTrasmissioneSingola;
            this.UtentiNotificati = utentiNotificati;
        }

        public string? CodiceGruppo { get; protected set; }

        public TextValue? DescrizioneGruppo { get; protected set; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; protected set; }

        public virtual void ChangeTipoTrasmissioneSingola(TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola)
        {
            this.TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }

        public IReadOnlyList<DatiUtenteNotificato> UtentiNotificati { get; protected set; }

        public virtual void ChangeUtentiNotificati(IReadOnlyList<DatiUtenteNotificato> utentiNotificati)
        {
            this.UtentiNotificati = utentiNotificati;
        }
    }

    public class UfficioDestinatarioModelloTrasmissione : DestinatarioModelloTrasmissione
    {
        public UfficioDestinatarioModelloTrasmissione(string id,
            string? codiceUfficio,
            TextValue? descrizioneUfficio,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
            : base(id, ragioneTrasmissione, noteTrasmissioneSingola, giorniScadenza, nascondiVersioniPrecedenti)
        {
            this.CodiceUfficio = codiceUfficio;
            this.DescrizioneUfficio = descrizioneUfficio;
            this.TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }

        public string? CodiceUfficio { get; protected set; }

        public TextValue? DescrizioneUfficio { get; protected set; }

        public TipiTrasmissioneSingolaEnum TipoTrasmissioneSingola { get; protected set; }

        public virtual void ChangeTipoTrasmissioneSingola(TipiTrasmissioneSingolaEnum tipoTrasmissioneSingola)
        {
            this.TipoTrasmissioneSingola = tipoTrasmissioneSingola;
        }
    }

    public abstract class DestinatarioModelloTrasmissione : Entity<string>
    {
        protected DestinatarioModelloTrasmissione(
            string id, 
            RagioneTrasmissione ragioneTrasmissione,
            TextValue? noteTrasmissioneSingola,
            int? giorniScadenza,
            bool? nascondiVersioniPrecedenti)
        {
            id = id ?? throw new ArgumentNullException(nameof(id));
            ragioneTrasmissione = ragioneTrasmissione ?? throw new ArgumentNullException(nameof(ragioneTrasmissione));

            this.Id = id;
            this.RagioneTrasmissione = ragioneTrasmissione;
            this.NoteTrasmissioneSingola = noteTrasmissioneSingola;
            this.GiorniScadenza = giorniScadenza;
            this.NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
        }

        public RagioneTrasmissione RagioneTrasmissione { get; protected set; }

        public TextValue? NoteTrasmissioneSingola { get; protected set; }

        public virtual void ChangeNoteTrasmissioneSingola(TextValue? noteTrasmissioneSingola)
        {
            this.NoteTrasmissioneSingola = noteTrasmissioneSingola;
        }

        public int? GiorniScadenza { get; protected set; }

        public bool? NascondiVersioniPrecedenti { get; protected set; }

        public virtual void ChangeGiorniScadenza(int? giorniScadenza)
        {
            this.GiorniScadenza = giorniScadenza;
        }

        public virtual void ChangeNascondiVersioniPrecedenti(bool? nascondiVersioniPrecedenti)
        {
            this.NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
        }
    }
}
