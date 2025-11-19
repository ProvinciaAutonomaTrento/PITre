// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate
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
            this.UtenteDestinatario = utenteDestinatario;
        }

        public UtenteDestinatario UtenteDestinatario { get; protected set; }

        internal override void AddTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            if (this._trasmissioniUtente.Count == 0)
                this._trasmissioniUtente.Add(trasmissioneUtente);
        }

        internal override void RemoveTrasmissioneUtente(TrasmissioneUtente trasmissioneUtente)
        {
            this._trasmissioniUtente.Clear();
        }

        #endregion

        #region Private Members

        #endregion
    }
}
