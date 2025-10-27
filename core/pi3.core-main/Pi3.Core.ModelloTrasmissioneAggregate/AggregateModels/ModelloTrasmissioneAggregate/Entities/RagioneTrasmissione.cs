// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.ValueObjects;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ModelloTrasmissioneAggregate.Entities
{

    public class RagioneTrasmissione : Entity<string>
    {
        #region Public Members

        internal RagioneTrasmissione(string id, string? nome = null, CessioneDirittiRagioneTrasmissione? cessioneDiritti = null)
        {
            Id = id;
            Nome = nome;
            CessioneDiritti = cessioneDiritti;
        }

        public string? Nome { get; protected set; } = null;

        public CessioneDirittiRagioneTrasmissione? CessioneDiritti { get; protected set; } = null;

        #endregion

        #region Private Members

        #endregion
    }
}
