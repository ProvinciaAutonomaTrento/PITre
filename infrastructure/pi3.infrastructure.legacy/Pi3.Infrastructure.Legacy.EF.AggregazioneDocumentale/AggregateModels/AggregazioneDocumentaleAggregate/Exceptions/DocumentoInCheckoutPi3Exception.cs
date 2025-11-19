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

    public class DocumentoInCheckoutPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoInCheckoutPi3Exception(string idDoc, string idFolder)
            : base(ErrorDescriptions.DocumentoInCheckoutException, null, ErrorDescriptions.ResourceManager, idDoc, idFolder)
        {
            IdDoc = idDoc;
            IdFolder = idFolder;
        }

        public string IdDoc { get; init; }
        public string IdFolder { get; init; }

        #endregion
    }
}
