// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.DocumentoScambiaAllegato
{
    public class DocumentoConsolidatoPi3Exception : Pi3Exception
    {
        #region Public Members

        public DocumentoConsolidatoPi3Exception(string id)
            : base(ErrorDescriptions.DocumentoConsolidato, ErrorDescriptions.ResourceManager, id)
        {
        }

        #endregion
    }
}
