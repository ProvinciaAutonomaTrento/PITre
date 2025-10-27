// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Exceptions
{
    public class CorrispondenteInListaDistribuzionePi3Exception : Pi3Exception
    {
        #region Public Members

        public CorrispondenteInListaDistribuzionePi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.CorrispondenteInListaDistribuzione, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }
}
