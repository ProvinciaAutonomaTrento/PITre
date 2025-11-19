// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentoAmministrativoAggregate.Exceptions
{
    public class CampoContatoreRepertorioNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public CampoContatoreRepertorioNotFoundPi3Exception(string idField)
            : base(ErrorDescriptions.CampoContatoreRepertorioNotFound, ErrorDescriptions.ResourceManager, idField)
        {
            IdField = idField;
        }

        public string IdField { get; init; }

        #endregion
    }
}
