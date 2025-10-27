// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.Extensions.Logging;
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities
{
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

            Id = id;
            RagioneTrasmissione = ragioneTrasmissione;
            NoteTrasmissioneSingola = noteTrasmissioneSingola;
            GiorniScadenza = giorniScadenza;
            NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
        }

        public RagioneTrasmissione RagioneTrasmissione { get; protected set; }

        public TextValue? NoteTrasmissioneSingola { get; protected set; }

        public virtual void ChangeNoteTrasmissioneSingola(TextValue? noteTrasmissioneSingola)
        {
            NoteTrasmissioneSingola = noteTrasmissioneSingola;
        }

        public int? GiorniScadenza { get; protected set; }

        public bool? NascondiVersioniPrecedenti { get; protected set; }

        public virtual void ChangeGiorniScadenza(int? giorniScadenza)
        {
            GiorniScadenza = giorniScadenza;
        }

        public virtual void ChangeNascondiVersioniPrecedenti(bool? nascondiVersioniPrecedenti)
        {
            NascondiVersioniPrecedenti = nascondiVersioniPrecedenti;
        }
    }
}
