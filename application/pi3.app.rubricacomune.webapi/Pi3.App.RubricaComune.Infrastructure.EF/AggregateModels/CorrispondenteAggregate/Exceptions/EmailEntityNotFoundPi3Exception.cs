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
    public class EmailEntityNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public EmailEntityNotFoundPi3Exception(string idEmail)
            : base(ErrorDescriptions.EmailEntityNonTrovata, null, ErrorDescriptions.ResourceManager, idEmail)
        {
            this.IdEmail = idEmail;
        }

        public string IdEmail { get; init; }

        #endregion
    }
}
