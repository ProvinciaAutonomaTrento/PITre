// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.PersonaCorrispondenteAggregate.Exceptions
{
    public class PersonaCorrispondenteCanNotBeModifiedPi3Exception : Pi3Exception
    {
        #region Public Members

        public PersonaCorrispondenteCanNotBeModifiedPi3Exception(string idCorrispondente)
            : base(ErrorDescriptions.PersonaCorrispondenteCanNotBeModified, null, ErrorDescriptions.ResourceManager, idCorrispondente)
        {
            IdCorrispondente = idCorrispondente;
        }

        public string IdCorrispondente { get; init; }

        #endregion
    }
}
