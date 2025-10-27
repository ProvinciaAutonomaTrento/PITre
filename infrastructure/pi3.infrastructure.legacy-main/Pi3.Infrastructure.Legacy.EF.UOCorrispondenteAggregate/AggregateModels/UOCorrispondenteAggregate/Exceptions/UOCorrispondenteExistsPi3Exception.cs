// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.UOCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.UOCorrispondenteAggregate.Exceptions
{
    public class UOCorrispondenteExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public UOCorrispondenteExistsPi3Exception(string codiceUOCorrispondente)
            : base(ErrorDescriptions.UOCorrispondenteExists, null, ErrorDescriptions.ResourceManager, codiceUOCorrispondente)
        {
            CodiceUOCorrispondente = codiceUOCorrispondente;
        }

        public string CodiceUOCorrispondente { get; init; }

        #endregion
    }
}
