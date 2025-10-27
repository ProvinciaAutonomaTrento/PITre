// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities
{

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
            Id = id;
            RagioneTrasmissione = ragioneTrasmissione;
            Tipo = tipo;
            Note = note;
            DataScadenza = dataScadenza;
            NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
            _trasmissioniUtente = new List<TrasmissioneUtente>();
        }

        public RagioneTrasmissione RagioneTrasmissione { get; protected set; }

        public TipiTrasmissioneSingolaEnum Tipo { get; protected set; }

        public TextValue? Note { get; protected set; } = null;

        public DateTime? DataScadenza { get; protected set; } = null;

        public bool? NascondiVersioniPrecedenti { get; protected set; } = false;

        internal virtual void ChangeTipo(TipiTrasmissioneSingolaEnum newTipo)
        {
            Tipo = newTipo;
        }

        internal virtual void ChangeNote(TextValue? newNote)
        {
            Note = newNote;
        }

        internal virtual void ChangeDataScadenza(DateTime? newDataScadenza)
        {
            DataScadenza = newDataScadenza;
        }

        internal virtual void ChangeNascondiVersioniPrecedenti(bool? newNascondiVersioniPrecedenti)
        {
            NascondiVersioniPrecedenti = newNascondiVersioniPrecedenti;
        }

        public IReadOnlyList<TrasmissioneUtente> TrasmissioniUtente
        {
            get
            {
                return _trasmissioniUtente.AsReadOnly();
            }
        }

        internal virtual void AddTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            _trasmissioniUtente.Add(trasmissioneUtente);
        }

        internal virtual void RemoveTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            _trasmissioniUtente.Remove(trasmissioneUtente);
        }

        #endregion

        #region Private Members

        protected readonly List<TrasmissioneUtente> _trasmissioniUtente;

        #endregion
    }
}
