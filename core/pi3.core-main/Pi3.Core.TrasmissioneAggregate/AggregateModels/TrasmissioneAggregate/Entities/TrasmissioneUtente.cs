// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Exceptions;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.Resources;
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities
{
    public class TrasmissioneUtente : Entity<string>
    {
        #region Public Members

        internal TrasmissioneUtente(
            TrasmissioneSingola parent,
            string id, UtenteDestinatario utenteDestinatario)
        {
            Parent = parent;
            Id = id;
            UtenteDestinatario = utenteDestinatario;
        }

        public TrasmissioneSingola Parent { get; protected set; }

        public UtenteDestinatario UtenteDestinatario { get; protected set; }

        public DateTime? DataVista { get; protected set; } = null;

        public DateTime? DataAccettazione { get; protected set; } = null;

        public TextValue? NoteAccettazione { get; protected set; } = null;

        public DateTime? DataRifiuto { get; protected set; } = null;

        public TextValue? NoteRifiuto { get; protected set; } = null;

        public DateTime? DataRimozioneCentroNotifiche { get; protected set; } = null;

        internal void SetDataRimozioneCentroNotifiche(DateTime? dataRimozione)
        {
            DataRimozioneCentroNotifiche = dataRimozione;
        }

        public UtenteDestinatario? UtenteDelegato { get; protected set; } = null;

        internal void Do(Accetta accetta)
        {
            if (!Parent.RagioneTrasmissione.ConWorkflow ?? false)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.RagioneTrasmissioneSenzaWorkflow, ErrorDescriptions.ResourceManager, Id);

            if (Parent.Tipo == TipiTrasmissioneSingolaEnum.Uno &&
                Parent.TrasmissioniUtente.Any(tu => tu.DataAccettazione.HasValue))
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettata, ErrorDescriptions.ResourceManager, Id);

            else if (DataAccettazione.HasValue)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaAccettata, ErrorDescriptions.ResourceManager, Id);

            DataAccettazione = accetta.Data;
            NoteAccettazione = accetta.Note;

            if (!string.IsNullOrWhiteSpace(accetta.IdDelegato))
                UtenteDelegato = new UtenteDestinatario(accetta.IdDelegato, accetta.UserIdDelegato, accetta.CognomeDelegato, accetta.NomeDelegato);
        }

        internal void Do(Rifiuta rifiuta)
        {
            if (!Parent.RagioneTrasmissione.ConWorkflow ?? false)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.RagioneTrasmissioneSenzaWorkflow, ErrorDescriptions.ResourceManager, Id);

            if (Parent.Tipo == TipiTrasmissioneSingolaEnum.Uno &&
                            Parent.TrasmissioniUtente.Any(tu => tu.DataRifiuto.HasValue))
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaRifiutata, ErrorDescriptions.ResourceManager, Id);

            else if (DataRifiuto.HasValue)
                throw new TrasmissioneUtentePi3Exception(ErrorDescriptions.TrasmissioneUtenteGiaRifiutata, ErrorDescriptions.ResourceManager, Id);

            DataRifiuto = rifiuta.Data;
            NoteRifiuto = rifiuta.Note;

            if (!string.IsNullOrWhiteSpace(rifiuta.IdDelegato))
                UtenteDelegato = new UtenteDestinatario(rifiuta.IdDelegato, rifiuta.UserIdDelegato, rifiuta.CognomeDelegato, rifiuta.NomeDelegato);
        }

        #endregion

        #region Private Members

        #endregion
    }
}
