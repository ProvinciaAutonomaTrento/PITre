// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.TrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.TrasmissioneAggregate.Entities
{
    public class TrasmissioneSingolaGruppo : TrasmissioneSingola
    {
        #region Public Members

        internal TrasmissioneSingolaGruppo(string id,
            RagioneTrasmissione ragioneTrasmissione,
            TipiTrasmissioneSingolaEnum tipo,
            GruppoDestinatario gruppoDestinatario,
            TextValue? note = null,
            DateTime? dataScadenza = null,
            bool? nascondiVersioniPrecedenti = null)
            : base(id, ragioneTrasmissione, tipo, note, dataScadenza, nascondiVersioniPrecedenti)
        {
            GruppoDestinatario = gruppoDestinatario;
        }

        public GruppoDestinatario GruppoDestinatario { get; protected set; }


        #endregion

        #region Private Members

        #endregion
    }
}
