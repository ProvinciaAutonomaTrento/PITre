// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.ConsolidateDocumentById
{
    public class DocInFinalStateException : Pi3Exception
    {
        public DocInFinalStateException(string idDocument)
            : base(Resources.FinalStateException, Resources.ResourceManager, null, idDocument)
        {
                this.IdDocument = idDocument;
        }

        public string IdDocument { get; init; }
    }

    public class DocumentoAmministrativoPredispostoException : Pi3Exception
    {
        public DocumentoAmministrativoPredispostoException(string idDocument)
            :base(Resources.DocPredispostoException, Resources.ResourceManager, null, idDocument)
        {
            this.IdDocument = idDocument;
        }

        public string IdDocument { get; init; }

    }
}
