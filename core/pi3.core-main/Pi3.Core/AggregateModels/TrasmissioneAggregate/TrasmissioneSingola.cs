// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.RagioneTrasmissioneAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
{
    public enum TipiTrasmissioneSingolaEnum
    {
        Uno,
        Tutti
    }

    public abstract class TrasmissioneSingola : Entity<string>
    {
        #region Public Members

        internal TrasmissioneSingola(            
            string id,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipo,
            TextValue? note = null,
            DateTime? dataScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
        {            
            this.Id = id;
            this.RagioneTrasmissione = ragioneTrasmissione;
            this.Tipo = tipo;
            this.Note = note;
            this.DataScadenza = dataScadenza;
            this.NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
            this._trasmissioniUtente = new List<TrasmissioneUtente>();
        }

        public RagioneTrasmissione RagioneTrasmissione { get; protected set; }

        public TipiTrasmissioneSingolaEnum Tipo { get; protected set; }

        public TextValue? Note { get; protected set; } = null;

        public DateTime? DataScadenza { get; protected set; } = null;

        public bool? NascondiVersioniPrecedenti { get; protected set; } = false;

        internal virtual void ChangeTipo(TipiTrasmissioneSingolaEnum newTipo)
        {
            this.Tipo = Tipo;
        }

        internal virtual void ChangeNote(TextValue? newNote)
        {
            this.Note = newNote;
        }

        internal virtual void ChangeDataScadenza(DateTime? newDataScadenza)
        {
            this.DataScadenza = newDataScadenza;
        }

        internal virtual void ChangeNascondiVersioniPrecedenti(bool? newNascondiVersioniPrecedenti)
        {
            this.NascondiVersioniPrecedenti = newNascondiVersioniPrecedenti;
        }

        public IReadOnlyList<TrasmissioneUtente> TrasmissioniUtente
        {
            get
            {
                return this._trasmissioniUtente.AsReadOnly();
            }
        }

        internal virtual void AddTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            this._trasmissioniUtente.Add(trasmissioneUtente);
        }

        internal virtual void RemoveTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            this._trasmissioniUtente.Remove(trasmissioneUtente);
        }

        #endregion

        #region Private Members

        protected readonly List<TrasmissioneUtente> _trasmissioniUtente;

        #endregion
    }
}
