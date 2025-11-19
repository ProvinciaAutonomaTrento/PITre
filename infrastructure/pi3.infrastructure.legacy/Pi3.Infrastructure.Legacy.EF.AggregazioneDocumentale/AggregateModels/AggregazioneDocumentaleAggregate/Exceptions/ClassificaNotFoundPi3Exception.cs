// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.AggregazioneDocumentaleAggregate.Exceptions
{
    public class ClassificaNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public ClassificaNotFoundPi3Exception(string idClassifica)
            : base(ErrorDescriptions.ClassificaNotFound, ErrorDescriptions.ResourceManager, idClassifica)
        {
            IdClassifica = idClassifica;
        }

        public string IdClassifica { get; init; }

        #endregion
    }
}
