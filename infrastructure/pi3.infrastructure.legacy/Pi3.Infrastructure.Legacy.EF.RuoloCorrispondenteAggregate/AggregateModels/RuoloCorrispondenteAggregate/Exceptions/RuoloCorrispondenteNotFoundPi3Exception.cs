// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.RuoloCorrispondenteAggregate.Exceptions
{
    public class RuoloCorrispondenteNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public RuoloCorrispondenteNotFoundPi3Exception(string idRuoloCorrispondente)
            : base(ErrorDescriptions.RuoloCorrispondenteNotFound, null, ErrorDescriptions.ResourceManager, idRuoloCorrispondente)
        {
            IdRuoloCorrispondente = idRuoloCorrispondente;
        }

        public string IdRuoloCorrispondente { get; init; }

        #endregion
    }

}
