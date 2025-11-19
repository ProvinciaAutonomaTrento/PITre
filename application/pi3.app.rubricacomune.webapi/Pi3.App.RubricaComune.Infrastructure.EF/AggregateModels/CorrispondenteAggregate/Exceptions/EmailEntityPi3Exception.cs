// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.RubricaComune.Infrastructure.EF.AggregateModels.CorrispondenteAggregate.Exceptions
{
    public class EmailEntityPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public EmailEntityPi3Exception(string idEmail, string errorMessage)
            : base(errorMessage, null, ErrorDescriptions.ResourceManager, idEmail)
        {
            this.IdEmail = idEmail;
        }

        public string IdEmail { get; init; }

        #endregion
    }
}
