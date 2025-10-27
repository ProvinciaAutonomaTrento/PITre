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
    public class TrasmissioneSingolaUtente : TrasmissioneSingola
    {
        #region Public Members

        internal TrasmissioneSingolaUtente(
            string id,
            RagioneTrasmissione ragioneTrasmissione,
            UtenteDestinatario utenteDestinatario,
            TextValue? note = null,
            DateTime? dataScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
            : base(id, ragioneTrasmissione, TipiTrasmissioneSingolaEnum.Uno, note, dataScadenza, nascondiVersioniPrecedenti)
        {
            UtenteDestinatario = utenteDestinatario;
        }

        public UtenteDestinatario UtenteDestinatario { get; protected set; }

        internal override void AddTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            if (_trasmissioniUtente.Count == 0)
                _trasmissioniUtente.Add(trasmissioneUtente);
        }

        internal override void RemoveTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            _trasmissioniUtente.Clear();
        }

        #endregion

        #region Private Members

        #endregion
    }
}
