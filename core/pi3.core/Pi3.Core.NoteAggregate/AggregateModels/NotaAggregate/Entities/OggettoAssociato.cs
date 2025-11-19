// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.NotaAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.NotaAggregate.Entities
{
    public class OggettoAssociato : Entity<string>
    {
        #region Public Members

        internal OggettoAssociato(string idOggetto, TipiOggettoEnum tipoOggetto)
        {
            this.Id = idOggetto;
            this.TipoOggetto = tipoOggetto;
        }

        public TipiOggettoEnum TipoOggetto { get; protected set; }

        #endregion

        #region Private Members

        #endregion
    }

}
