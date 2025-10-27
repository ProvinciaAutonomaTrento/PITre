// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Exceptions
{
    public class RuoloCorrispondenteCanNotBeRemovedPi3Exception : Pi3Exception
    {
        #region Public Members

        public RuoloCorrispondenteCanNotBeRemovedPi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.RuoloCorrispondenteCanNotBeRemoved, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }
}
