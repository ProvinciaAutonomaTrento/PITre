// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.AggregateModels.DocumentoAmministrativoAggregate.Exceptions
{
    public class DocumentTypeFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DocumentTypeFoundPi3Exception(string documentType)
            : base(ErrorDescriptions.DocumentTypeNotFound, null, ErrorDescriptions.ResourceManager, documentType)
        {
            DocumentType = documentType;
        }

        public string DocumentType { get; init; }

        #endregion
    }
}
