// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions
{

    public class CorrispondenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CorrispondenteNotFoundPi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.CorrispondenteNonTrovato, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            this.IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }
}
