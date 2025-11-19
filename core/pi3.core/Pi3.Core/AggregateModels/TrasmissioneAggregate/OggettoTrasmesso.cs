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
    public class OggettoTrasmesso : Entity<string>
    {
        internal OggettoTrasmesso(string id, TipiOggettiTrasmessiEnum tipoOggetto)
        {
            this.Id = id;
            this.TipoOggetto = tipoOggetto;
        }

        public TipiOggettiTrasmessiEnum TipoOggetto { get; protected set; }
    }
}
