// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.DocumentoGetDettaglioDocumentoNoSecurity
{
    public class DocumentoNotFoundPi3Exception : NotFoundPi3Exception
    {
        #region Public Members

        public DocumentoNotFoundPi3Exception(long idProfile)
            : base(ErrorDescriptions.DocumentoNonTrovato, null, ErrorDescriptions.ResourceManager, idProfile)
        {
            this.IdProfile = idProfile;
        }

        public long IdProfile { get; init; }

        #endregion
    }
}
