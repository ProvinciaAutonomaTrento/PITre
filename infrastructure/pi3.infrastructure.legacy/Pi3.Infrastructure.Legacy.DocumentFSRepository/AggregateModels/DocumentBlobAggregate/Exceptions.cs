// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.DocumentFSRepository.AggregateModels.DocumentBlobAggregate
{
    public class DocumentBlobAlreadyExistsPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentBlobAlreadyExistsPi3Exception(string idDocumentBlob)
            : base(ErrorDescriptions.DocumentBlobAlreadyExists, ErrorDescriptions.ResourceManager, idDocumentBlob)
        {
            this.IdDocumentBlob = idDocumentBlob;
        }

        public string IdDocumentBlob { get; init; }

        #endregion
    }

    public class DocumentBlobNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DocumentBlobNotFoundPi3Exception(string idDocumentBlob)
            : base(ErrorDescriptions.DocumentBlobNotFound, ErrorDescriptions.ResourceManager, idDocumentBlob)
        {
            this.IdDocumentBlob = idDocumentBlob;
        }

        public string IdDocumentBlob { get; init; }

        #endregion
    }
}
